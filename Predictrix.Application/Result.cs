namespace Predictrix.Application
{
    public class Result
    {
        public required bool IsFailure { get; init; }
        public string Message { get; init; } = string.Empty;

        public static Result Success()
        {
            return new Result { IsFailure = false };
        }

        public static Result Success(string message)
        {
            return new Result { IsFailure = false, Message = message };
        }

        public static Result Failure(string message) => new() { IsFailure = true, Message = message };
    }

    public class Result<T>
    {
        public required bool IsFailure { get; init; }
        public T? Data { get; init; }
        public string Message { get; init; } = string.Empty;

        public static Result<T> Success(T data)
        {
            return new Result<T> { IsFailure = false, Data = data };
        }

        public static Result<T> Failure(string message) => new() { IsFailure = true, Message = message };
    }
}
