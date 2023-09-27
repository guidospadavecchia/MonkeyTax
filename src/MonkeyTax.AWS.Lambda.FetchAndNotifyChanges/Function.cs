using Amazon.Lambda.Core;
using Amazon.SimpleNotificationService;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MonkeyTax.Application.Monotributo.Model;
using MonkeyTax.AWS.DynamoDB.Extensions;
using MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Configuration;
using System.Diagnostics;
using MonotributoService = MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Services.MonotributoService;

// Assembly attribute to enable the Lambda function's JSON input to be converted into a .NET class.
[assembly: LambdaSerializer(typeof(Amazon.Lambda.Serialization.SystemTextJson.DefaultLambdaJsonSerializer))]

namespace MonkeyTax.AWS.Lambda.FetchAndNotifyChanges;

public class Function
{
    private const int MAX_FETCH_RETRIES = 3;

    private readonly IConfiguration _configuration;
    private readonly IServiceProvider _serviceProvider;

    public Function()
    {
        _configuration = ConfigurationService.GetConfiguration();
        _serviceProvider = CreateServiceProvider(_configuration);
    }

    private static IServiceProvider CreateServiceProvider(IConfiguration configuration)
    {
        ServiceCollection serviceCollection = new();
        serviceCollection.AddServices(configuration);
        serviceCollection.AddAwsDynamoDb(configuration);
        serviceCollection.AddScoped<IAmazonSimpleNotificationService, AmazonSimpleNotificationServiceClient>();

        return serviceCollection.BuildServiceProvider();
    }

    /// <summary>
    /// Function entry point.
    /// </summary>
    /// <param name="context">The lambda context.</param>
    public async Task HandleAsync(ILambdaContext context)
    {
        Console.WriteLine("Starting Fetch & Notify function...");
        TimeSpan remainingTime = context.RemainingTime == TimeSpan.Zero ? TimeSpan.FromMinutes(10) : context.RemainingTime;
        CancellationToken cancellationToken = new CancellationTokenSource(remainingTime.Subtract(TimeSpan.FromSeconds(1))).Token;

        MonotributoService monotributoService = _serviceProvider.GetRequiredService<MonotributoService>();

        MonotributoResponse? response = await FetchAsync(monotributoService, cancellationToken);
        if (response != null)
        {
            Console.WriteLine("Data fetched successfully");
            Console.WriteLine("Querying database to see if values have changed...");
            bool valuesChanged = await monotributoService.PutResponseAsync(response);
            Console.WriteLine($"Values changed?: {valuesChanged}");
        }

        Console.WriteLine("Fetch & Notify function finished.");
    }

    private static async Task<MonotributoResponse?> FetchAsync(MonotributoService monotributoService, CancellationToken cancellationToken = default)
    {
        bool error;
        int retries = 0;
        do
        {
            try
            {
                Console.WriteLine("Fetching data from API...");
                return await monotributoService.FetchAsync(cancellationToken);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Failed to fetch data: {ex.Message}");
                Debug.WriteLine(ex);
                Console.WriteLine("Retrying...");
                error = true;
                if (retries < MAX_FETCH_RETRIES)
                    retries++;
                else
                    throw;
            }
        } while (error || retries < MAX_FETCH_RETRIES);

        return null;
    }
}