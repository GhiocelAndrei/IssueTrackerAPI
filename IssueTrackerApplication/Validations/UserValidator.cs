using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class UserValidator : AbstractValidator<UserCreatingDto>
    {
        public UserValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.");
        }
    }
}
