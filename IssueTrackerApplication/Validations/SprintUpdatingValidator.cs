using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class SprintUpdatingValidator : AbstractValidator<SprintUpdatingDto>
    {
        public SprintUpdatingValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.Active)
                .NotNull()
                .WithMessage("Active flag is required.");

            RuleFor(x => x.StartDate)
                .NotEmpty()
                .WithMessage("Start date is required.");

            RuleFor(x => x.EndDate)
                .NotEmpty()
                .WithMessage("End date is required.");
        }
    }
}
