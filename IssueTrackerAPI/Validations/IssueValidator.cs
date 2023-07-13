using FluentValidation;
using IssueTrackerAPI.DatabaseContext;
using IssueTrackerAPI.Mapping;
using Microsoft.EntityFrameworkCore;

namespace IssueTrackerAPI.Validations
{
    public class IssueValidator : AbstractValidator<IssueCreatingDto>
    {
        protected readonly IssueContext _dbContext;
        public IssueValidator(IssueContext dbContext)
        {
            _dbContext = dbContext;

            RuleFor(x => x.Title)
            .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.Description)
            .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.ProjectId)
                 .NotEmpty().WithMessage("ProjectId is required.");

            RuleFor(x => x.ReporterId)
                .NotEmpty().WithMessage("ReporterId is required.");

            RuleFor(x => x.AssigneeId)
                .NotEmpty().WithMessage("AssigneeId is required.");

            RuleFor(x => x.Priority)
                .IsInEnum()
                .WithMessage("Invalid Priority.");
        }
    }
}
