using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class ProjectUpdatingValidator : AbstractValidator<ProjectUpdatingDto>
    {
        public ProjectUpdatingValidator()
        {
            RuleFor(x => x.Name)
               .NotEmpty().WithMessage("Name is required.");
        }
    }
}
