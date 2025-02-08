using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class GradesJournalDtoValidator : AbstractValidator<GradesJournalDto>
{
    public GradesJournalDtoValidator()
    {
        RuleFor(e => e.GroupId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.SubjectId)
            .GreaterThan(0).WithMessage("Must be greater than 0");
        RuleFor(e => e.Name)
            .NotEmpty().When(e => e.Name is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
    }
}