namespace ReservaHoteleraApp.DAL.Models
{
    public class OperationResult<T>
    {
        public bool Success { get; set; } = true;
        public T? Data { get; set; }
        public string? Message { get; set; }
    }

    public static class OperationResult
    {
        public static OperationResult<T> Failure<T>(string message)
        {
            return new OperationResult<T>
            {
                Success = false,
                Message = message
            };
        }

        public static OperationResult<T> Success<T>(T data, string message = "")
        {
            return new OperationResult<T>
            {
                Success = true,
                Data = data,
                Message = message
            };
        }
    }
}
