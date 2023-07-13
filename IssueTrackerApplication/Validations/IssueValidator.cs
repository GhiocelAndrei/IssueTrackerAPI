using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class IssueValidator : AbstractValidator<IssueCreatingDto>
    { 
        public IssueValidator()
        {

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
