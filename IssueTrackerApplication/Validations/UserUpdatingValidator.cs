using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class UserUpdatingValidator : AbstractValidator<UserUpdatingDto>
    {
        public UserUpdatingValidator()
        {
            RuleFor(dto => dto)
                .Must(HaveAtLeastOnePropertyNotNull)
                .WithMessage("At least one property must be updated");
        }

        private bool HaveAtLeastOnePropertyNotNull(UserUpdatingDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Name) ||
                   !string.IsNullOrWhiteSpace(dto.Email) ||
                    !string.IsNullOrWhiteSpace(dto.Role);
        }
    }
}
