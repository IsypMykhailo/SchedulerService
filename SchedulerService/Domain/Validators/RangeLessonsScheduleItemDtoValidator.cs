using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class RangeLessonsScheduleItemDtoValidator : AbstractValidator<RangeLessonsScheduleItemDto>
{
    public RangeLessonsScheduleItemDtoValidator()
    {
        RuleFor(e => e.DayOfWeek).IsInEnum();
        RuleFor(e => e.SubjectId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.SubGroupId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.TeacherId)
            .NotEmpty().When(e => e.TeacherId is not null).WithMessage("Must be not empty or whitespace");
        RuleFor(e => e.From)
            .LessThanOrEqualTo(e => e.To)
            .WithMessage("From Date can not be greater than To Date");
        RuleFor(e => e.LessonIndex)
            .GreaterThan(_ => 0).WithMessage("Lesson Index must be positive number")
            .LessThanOrEqualTo(_ => 99).WithMessage("Lesson Index must be less than 100");
        RuleFor(e => e)
            .Must(BeLessThanYear)
            .WithMessage("Range between From Date and To Date must be less than or equal to 1 year");
    }
    
    private bool BeLessThanYear(RangeLessonsScheduleItemDto dto)
    {
        var daysOffset = dto.To.DayNumber - dto.From.DayNumber;
        return daysOffset <= 366;
    }
}