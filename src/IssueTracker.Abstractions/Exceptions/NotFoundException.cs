namespace IssueTracker.Abstractions.Exceptions
{
    public class NotFoundException : ApplicationBaseException
    {
        public NotFoundException(string message) : base(message)
        {
        }
    }
}
