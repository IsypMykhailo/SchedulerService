using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class BellsScheduleItemDtoValidator : AbstractValidator<BellsScheduleItemDto>
{
    public BellsScheduleItemDtoValidator()
    {
        RuleFor(e => e.LessonIndex)
            .GreaterThan(_ => 0).WithMessage("Lesson Index must be positive number")
            .LessThanOrEqualTo(_ => 99).WithMessage("Lesson index must be less than 100");
        RuleFor(e => e.LessonStartTime)
            .LessThan(e => e.LessonEndTime).WithMessage("Lesson Start Time must be less than Lesson End Time");
    }
}