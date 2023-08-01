using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class IssueUpdatingValidator : AbstractValidator<IssueUpdatingDto>
    {
        public IssueUpdatingValidator()
        {
            RuleFor(dto => dto)
                .Must(HaveAtLeastOnePropertyNotNull)
                .WithMessage("At least one property must be updated");

            RuleFor(x => x.Priority)
               .IsInEnum()
               .WithMessage("Invalid Priority.");
        }

        private bool HaveAtLeastOnePropertyNotNull(IssueUpdatingDto dto)
        {
            return dto.ProjectId != null ||
                   !string.IsNullOrWhiteSpace(dto.Title) ||
                   !string.IsNullOrWhiteSpace(dto.Description) ||
                   dto.Priority != null ||
                   dto.ReporterId != null ||
                   dto.AssigneeId != null;
        }
    }
}
