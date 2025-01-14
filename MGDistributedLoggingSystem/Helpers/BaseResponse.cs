namespace MGDistributedLoggingSystem.Helpers
{
    public class BaseResponse
    {
        public bool Succeeded { get; set; } = false;
        public string Message { get; set; } = string.Empty;
        public object? Data { get; set; }

        public BaseResponse(string message = "", bool succeeded = false, object? data = null)
        {
            Message = message;
            Succeeded = succeeded;
            Data = data;
        }
    }

    public class BaseResponse<T> : BaseResponse where T : class
    {
        public new T? Data { get; set; }

        public BaseResponse(string message = "", bool succeeded = false, T? data = null)
            : base(message, succeeded, data)
        {
            Data = data;
        }
    }



}
