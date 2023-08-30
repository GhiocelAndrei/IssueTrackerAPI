namespace IssueTracker.Abstractions.Exceptions
{
    public class ApplicationBaseException : Exception
    {
        public ApplicationBaseException(string message) : base(message) 
        { 
        }
    }
}
