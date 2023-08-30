using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class ProjectCreatingValidator : AbstractValidator<ProjectCreatingDto>
    {
        public ProjectCreatingValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");
        }
    }
}
