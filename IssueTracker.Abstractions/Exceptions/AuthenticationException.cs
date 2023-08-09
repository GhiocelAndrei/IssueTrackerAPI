namespace IssueTracker.Abstractions.Exceptions
{
    public class AuthenticationException : ApplicationBaseException
    {
        public AuthenticationException(string message) : base(message)
        {
        }
    }
}
