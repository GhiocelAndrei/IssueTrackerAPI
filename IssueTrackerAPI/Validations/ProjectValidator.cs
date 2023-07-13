using FluentValidation;
using IssueTrackerAPI.Mapping;

namespace IssueTrackerAPI.Validations
{
    public class ProjectValidator : AbstractValidator<ProjectCreatingDto>
    {
        public ProjectValidator() 
        {
            RuleFor(x => x.Name)
          .NotEmpty().WithMessage("Name is required.");
        }
    }
}
