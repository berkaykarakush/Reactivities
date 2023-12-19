namespace Application.Core
{
    public class Result<T>
    {
        public bool IsSucces { get; set; }
        public T Value { get; set; }
        public string Error { get; set; }
        // Generic method for creating a successful result with a specified value
        public static Result<T> Success(T Value) => new Result<T> { IsSucces = true, Value = Value };
        // Generic method for creating a failure result with a specified error message
        public static Result<T> Failure(string error) => new Result<T> { IsSucces = false, Error = error };
    }
}