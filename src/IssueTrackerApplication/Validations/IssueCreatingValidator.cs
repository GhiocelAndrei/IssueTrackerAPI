﻿using FluentValidation;
using IssueTracker.Abstractions.Mapping;

namespace IssueTracker.Application.Validations
{
    public class IssueCreatingValidator : AbstractValidator<IssueCreatingDto>
    { 
        public IssueCreatingValidator()
        {

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.");

            RuleFor(x => x.ProjectId)
                 .NotEmpty().WithMessage("ProjectId is required.");

            RuleFor(x => x.ReporterId)
                .NotEmpty().WithMessage("ReporterId is required.");

            RuleFor(x => x.Priority)
                .IsInEnum()
                .WithMessage("Invalid Priority.");
        }
    }
}
