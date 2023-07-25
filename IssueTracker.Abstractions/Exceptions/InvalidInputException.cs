namespace IssueTracker.Abstractions.Exceptions
{
    public class InvalidInputException : ApplicationBaseException
    {
        public InvalidInputException(string message) : base(message)
        {
        }
    }
}
