using FluentValidation;
using IssueTrackerAPI.Mapping;
using IssueTrackerAPI.Models;

namespace IssueTrackerAPI.Validations
{
    public class UserValidator : AbstractValidator<UserCreatingDto>
    {
        public UserValidator() 
        {
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Name is required.")
                .Matches(@"^[^\s]*$").WithMessage("Name cannot contain spaces.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("A valid email is required.");

            RuleFor(x => x.Role)
                .NotEmpty().WithMessage("Role is required.")
                .Matches(@"^[^\s]*$").WithMessage("Role cannot contain spaces.");
        }
    }
}
