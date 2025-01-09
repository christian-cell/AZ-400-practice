namespace User.Models.Exceptions
{
    public class ValidationException : Exception
    {
        public ValidationError[] Errors { get; set; }
        
        public ValidationException(string message) : base(message){}
       
        public ValidationException(string message, Exception innerException) : base(message , innerException){}

        public ValidationException(ValidationError[] errors) : base($"There are {errors.Length} validation errors") => Errors = errors;
        
        public ValidationException(string message, ValidationError[] errors) : base(message) => Errors = errors;
    }
};