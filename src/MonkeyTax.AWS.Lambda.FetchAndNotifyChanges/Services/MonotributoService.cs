namespace MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Services
{
    using Amazon.DynamoDBv2;
    using Amazon.DynamoDBv2.Model;
    using Amazon.SimpleNotificationService;
    using Amazon.SimpleNotificationService.Model;
    using Configuration;
    using MonkeyTax.Application.Monotributo.Model;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using RestSharp;
    using System.Threading;

    internal class MonotributoService
    {
        private readonly RestClient _restClient;
        private readonly MonotributoServiceConfig _config;
        private readonly IAmazonDynamoDB _awsDynamoDbClient;
        private readonly IAmazonSimpleNotificationService _awsSnsClient;

        public MonotributoService(MonotributoServiceConfig config, IAmazonDynamoDB awsDynamoDbClient, IAmazonSimpleNotificationService awsSnsClient)
        {
            _config = config;
            _awsDynamoDbClient = awsDynamoDbClient;
            _awsSnsClient = awsSnsClient;
            RestClientOptions options = new()
            {
                ThrowOnAnyError = true,
                ThrowOnDeserializationError = true,
            };
            _restClient = new RestClient(config.BaseUrl);
            _awsDynamoDbClient = awsDynamoDbClient;
        }

        public async Task<MonotributoResponse> FetchAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                RestRequest request = new(_config.GetCategoriesUrl);
                string? cacheControlHeader = Environment.GetEnvironmentVariable("CACHE_CONTROL_HEADER");
                if (!string.IsNullOrWhiteSpace(cacheControlHeader))
                {
                    request.AddHeader("Cache-Control", cacheControlHeader);
                }
                string? timeout = Environment.GetEnvironmentVariable("REQUEST_TIMEOUT_MILLISECONDS");
                if (!string.IsNullOrWhiteSpace(timeout) && int.TryParse(timeout, out int n))
                {
                    request.Timeout = n;
                }
                RestResponse<MonotributoResponse> result = await _restClient.ExecuteAsync<MonotributoResponse>(request, cancellationToken);
                if (result.IsSuccessful)
                {
                    return result.Data ?? throw new($"Fetch failed because the result object is null. Status Code: '({(int)result.StatusCode}) {result.StatusCode}'");
                }
                else
                {
                    throw new($"Fetch failed: '{result.Content}'. Status code '({(int)result.StatusCode}) {result.StatusCode}'", result.ErrorException);
                }
            }
            catch (Exception ex)
            {
                Console.Error.WriteLine($"ERROR: Unexpected error fetching data:");
                Console.Error.WriteLine(ex);
                throw;
            }
        }

        public async Task<bool> PutResponseAsync(MonotributoResponse response, CancellationToken cancellationToken = default)
        {
            string tableName = _config.TableName;
            string partitionKey = DateTime.UtcNow.Year.ToString();
            string newContent = JsonConvert.SerializeObject(response);
            bool insertResponse = true;

            var request = new QueryRequest
            {
                TableName = tableName,
                KeyConditionExpression = "id = :id",
                ExpressionAttributeValues = new Dictionary<string, AttributeValue>
                {
                    { ":id", new AttributeValue { S = partitionKey }
                }
            },
                Limit = 1,
                ScanIndexForward = false,
            };
            QueryResponse queryResponse = await _awsDynamoDbClient.QueryAsync(request, cancellationToken);
            Dictionary<string, AttributeValue> values = queryResponse.Items.FirstOrDefault() ?? [];
            if (values.Any())
            {
                string actualContent = values["content"].S;
                JObject actualJsonContent = JObject.Parse(actualContent);
                JObject newJsonContent = JObject.Parse(newContent);
                if (!JToken.DeepEquals(actualJsonContent, newJsonContent))
                {
                    PublishRequest snsJsonRequest = new()
                    {
                        TopicArn = _config.PublishTopicArn,
                        Subject = _config.PublishSubject,
                        MessageStructure = "json",
                        Message = BuildPublishMessage(actualContent, newContent),
                    };
                    await _awsSnsClient.PublishAsync(snsJsonRequest, cancellationToken);
                }
                else
                {
                    insertResponse = false;
                }
            }

            if (insertResponse)
            {
                await InsertResponseAsync(partitionKey, newContent, cancellationToken); 
            }

            return insertResponse;
        }

        #region Private

        private async Task InsertResponseAsync(string partitionKey, string content, CancellationToken cancellationToken = default)
        {
            string timestamp = DateTime.UtcNow.ToString("s");
            Dictionary<string, AttributeValue> values = new()
            {
                ["id"] = new AttributeValue { S = partitionKey },
                ["fetched_utc"] = new AttributeValue { S = timestamp },
                ["content"] = new AttributeValue { S = content }
            };
            await _awsDynamoDbClient.PutItemAsync(_config.TableName, values, cancellationToken);
        }

        private string BuildPublishMessage(string actualContent, string newContent)
        {
            string message = _config.PublishMessage;
            var jsonMessage = new
            {
                Actual = actualContent,
                New = newContent,
            };
            Dictionary<string, object> messageJsonResponse = new()
            {
                { "default", message },
                { "email", message },
                { "email-json", message },
                { "http", jsonMessage },
                { "https", jsonMessage },
                { "sqs", jsonMessage },
            };

            return JsonConvert.SerializeObject(messageJsonResponse);
        }

        #endregion
    }
}
