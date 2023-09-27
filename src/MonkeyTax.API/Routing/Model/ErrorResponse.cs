namespace MonkeyTax.API.Routing.Model
{
    public class ErrorResponse
    {
        public int StatusCode { get; set; }
        public string ErrorCode { get; set; } = null!;
        public string ErrorMessage { get; set; } = null!;
        public Exception? Exception { get; set; }
    }
}
