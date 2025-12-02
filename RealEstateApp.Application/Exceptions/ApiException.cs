namespace RealEstateApp.Application.Exceptions
{
    public class ApiException : Exception
    {
        public int StatusCode { get; }

        public ApiException(string message, int statusCode = 500)
            : base(message)
        {
            StatusCode = statusCode;
        }

        public ApiException(string message, Exception innerException, int statusCode = 500)
            : base(message, innerException)
        {
            StatusCode = statusCode;
        }
    }
}
