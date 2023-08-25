using IssueTracker.Abstractions.Enums;

namespace IssueTracker.Abstractions.Mapping
{
    public class SearchResultDto
    {
        public SearchResultType Type { get; set; }
        public string TitleOrName { get; set; }
    }
}
