namespace MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Services.Configuration
{
    internal class MonotributoServiceConfig
    {
        public required string BaseUrl { get; set; }
        public required string GetCategoriesUrl { get; set; }
        public required string TableName { get; set; }
    }
}
