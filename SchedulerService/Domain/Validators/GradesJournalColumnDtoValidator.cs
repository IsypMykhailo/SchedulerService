using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class GradesJournalColumnDtoValidator : AbstractValidator<GradesJournalColumnDto>
{
    public GradesJournalColumnDtoValidator()
    {
        RuleFor(e => e.HomeworkId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.LessonId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.ColumnHeader)
            .NotEmpty().When(e => e.ColumnHeader is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
    }
}