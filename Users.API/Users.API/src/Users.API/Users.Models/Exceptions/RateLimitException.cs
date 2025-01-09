namespace User.Models.Exceptions
{
    public class RateLimitException : Exception
    {
        public RateLimitException(string message) : base(message)
        {
            
        }   
    }
};