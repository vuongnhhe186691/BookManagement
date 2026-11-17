namespace A_PRN212.Application.Common
{
    public class ServiceResult
    {
        public bool IsSuccess { get; protected set; }
        public string Message { get; protected set; } = string.Empty;

        protected ServiceResult(bool isSuccess, string message)
        {
            IsSuccess = isSuccess;
            Message = message;
        }

        public static ServiceResult Success(string message = "") => new(true, message);
        public static ServiceResult Failure(string message) => new(false, message);
    }

    public class ServiceResult<T> : ServiceResult
    {
        public T? Data { get; private set; }

        private ServiceResult(bool isSuccess, T? data, string message)
            : base(isSuccess, message)
        {
            Data = data;
        }

        public static ServiceResult<T> Success(T data, string message = "") => new(true, data, message);
        public new static ServiceResult<T> Failure(string message) => new(false, default, message);
    }
}
