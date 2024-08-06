using Serilog;

namespace Predictrix.Application
{
    public class Result
    {
        public required bool IsFail { get; init; }
        public string Message { get; private init; } = string.Empty;

        public static Result Ok()
        {
            return new Result { IsFail = false };
        }

        public static Result Ok(string message)
        {
            Log.Information("Success: {Message}", message);
            return new Result { IsFail = false, Message = message };
        }

        public static Result Fail(string message)
        {
            Log.Error("Error: {Message}", message);
            return new Result { IsFail = true, Message = message };
        }
    }

    public class Result<T>
    {
        public required bool IsFail { get; init; }
        public T? Data { get; init; }
        public string Message { get; init; } = string.Empty;

        public static Result<T> Ok(T data)
        {
            return new Result<T> { IsFail = false, Data = data };
        }

        public static Result<T> Fail(string message) => new() { IsFail = true, Message = message };
    }
}
