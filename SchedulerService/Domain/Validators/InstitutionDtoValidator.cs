using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class InstitutionDtoValidator : AbstractValidator<InstitutionDto>
{
    public InstitutionDtoValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
        RuleFor(e => e.Description)
            .MaximumLength(500).WithMessage("Maximum number of characters is 500");
    }
}