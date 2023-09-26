using Amazon.DynamoDBv2.DataModel;
using Amazon.DynamoDBv2;
using Amazon.Extensions.NETCore.Setup;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace MonkeyTax.AWS.DynamoDB.Extensions
{
    public static class AwsDynamoDbExtensions
    {
        public static IServiceCollection AddAwsDynamoDb(this IServiceCollection serviceCollection, IConfiguration configuration)
        {
            AWSOptions awsOptions = configuration.GetAWSOptions();
            serviceCollection.AddDefaultAWSOptions(awsOptions);
            serviceCollection.AddAWSService<IAmazonDynamoDB>();
            serviceCollection.AddScoped<IDynamoDBContext, DynamoDBContext>();

            return serviceCollection;
        }
    }
}