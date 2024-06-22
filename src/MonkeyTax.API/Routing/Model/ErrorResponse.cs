namespace MonkeyTax.API.Routing.Model
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public required string ErrorCode { get; set; }
        public required string ErrorMessage { get; set; }
        public Exception? Exception { get; set; }
    }
}
