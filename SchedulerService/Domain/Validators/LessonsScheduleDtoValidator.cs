using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class LessonsScheduleDtoValidator : AbstractValidator<LessonsScheduleDto>
{
    public LessonsScheduleDtoValidator()
    {
        RuleFor(e => e.Name)
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
        RuleFor(e => e.BellsScheduleId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
    }
}