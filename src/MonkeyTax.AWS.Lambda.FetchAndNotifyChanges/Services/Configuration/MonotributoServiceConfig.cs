namespace MonkeyTax.AWS.Lambda.FetchAndNotifyChanges.Services.Configuration
{
    internal class MonotributoServiceConfig
    {
        public string BaseUrl { get; set; } = null!;
        public string GetCategoriesUrl { get; set; } = null!;
        public string TableName { get; set; } = null!;
        public string PublishTopicArn { get; set; } = null!;
        public string PublishSubject { get; set; } = null!;
        public string PublishMessage { get; set; } = null!;
    }
}
