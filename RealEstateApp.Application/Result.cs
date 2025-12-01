namespace RealEstateApp.Application
{
    public class Result
    {
        public bool Succeeded { get; set; }
        public List<string>? Errors { get; set; }
        public string? Message { get; set; }

        public static Result Ok()
        {
            return new Result { Succeeded = true };
        }

        public static Result Ok(string message)
        {
            return new Result { Succeeded = true, Message = message };
        }

        public static Result Fail(string error)
        {
            return new Result { Succeeded = false, Errors = new List<string> { error } };
        }

        public static Result Fail(List<string> errors)
        {
            return new Result { Succeeded = false, Errors = errors };
        }
    }

    public class Result<T>
    {
        public bool Succeeded { get; set; }
        public T? Data { get; set; }
        public List<string>? Errors { get; set; }
        public string? Message { get; set; }

        public static Result<T> Ok(T data)
        {
            return new Result<T> { Succeeded = true, Data = data };
        }

        public static Result<T> Ok(T data, string message)
        {
            return new Result<T> { Succeeded = true, Data = data, Message = message };
        }

        public static Result<T> Fail(string error)
        {
            return new Result<T> { Succeeded = false, Errors = new List<string> { error } };
        }

        public static Result<T> Fail(List<string> errors)
        {
            return new Result<T> { Succeeded = false, Errors = errors };
        }
    }
}

