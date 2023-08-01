using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class ProjectUpdatingValidator : AbstractValidator<ProjectUpdatingDto>
    {
        public ProjectUpdatingValidator()
        {
            RuleFor(dto => dto)
                .Must(HaveAtLeastOnePropertyNotNull)
                .WithMessage("At least one property must be updated");
        }

        private bool HaveAtLeastOnePropertyNotNull(ProjectUpdatingDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Name);
        }
    }
}
