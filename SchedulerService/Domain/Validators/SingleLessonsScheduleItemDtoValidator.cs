using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class SingleLessonsScheduleItemDtoValidator : AbstractValidator<SingleLessonsScheduleItemDto>
{
    public SingleLessonsScheduleItemDtoValidator()
    {
        RuleFor(e => e.SubjectId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.SubGroupId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.TeacherId)
            .NotEmpty().When(e => e.TeacherId is not null).WithMessage("Must be not empty or whitespace");
        RuleFor(e => e.LessonIndex)
            .GreaterThan(_ => 0).WithMessage("Lesson Index must be positive number")
            .LessThanOrEqualTo(_ => 99).WithMessage("Lesson Index must be less than 100");
    }
}