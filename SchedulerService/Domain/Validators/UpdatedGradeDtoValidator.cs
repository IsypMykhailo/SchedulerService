using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class UpdatedGradeDtoValidator : AbstractValidator<UpdatedGradeDto>
{
    public UpdatedGradeDtoValidator()
    {
        RuleFor(e => e.Points)
            .GreaterThan((short)0).WithMessage("Must be greater than 0")
            .LessThanOrEqualTo((short)12).WithMessage("Must be less than or equal to 12");
        RuleFor(e => e.Description)
            .NotEmpty().When(e => e.Description is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
    }
}