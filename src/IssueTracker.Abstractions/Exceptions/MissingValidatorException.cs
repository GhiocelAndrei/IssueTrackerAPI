namespace IssueTracker.Abstractions.Exceptions
{
    public class MissingValidatorException : ApplicationBaseException
    {
        public MissingValidatorException(string message) : base(message)
        {
        }
    }
}
