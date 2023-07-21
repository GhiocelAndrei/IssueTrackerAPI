namespace IssueTracker.Application.Authorization
{
    public class ScopesList
    {
        public string[] Value { get; set; }

        public ScopesList(string[] value)
        {
            Value = value;
        }
    }
}
