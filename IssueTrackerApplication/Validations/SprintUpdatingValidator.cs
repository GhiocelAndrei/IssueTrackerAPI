using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class SprintUpdatingValidator : AbstractValidator<SprintUpdatingDto>
    {
        public SprintUpdatingValidator() 
        {
            RuleFor(dto => dto)
                .Must(HaveAtLeastOnePropertyNotNull)
                .WithMessage("At least one property must be updated");
        }

        private bool HaveAtLeastOnePropertyNotNull(SprintUpdatingDto dto)
        {
            return !string.IsNullOrWhiteSpace(dto.Name) ||
                   !string.IsNullOrWhiteSpace(dto.Description) ||
                    dto.Active.HasValue ||
                    dto.StartDate.HasValue ||
                    dto.EndDate.HasValue;
        }
    }
}
