using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class AttendanceDtoValidator : AbstractValidator<AttendanceDto>
{
    public AttendanceDtoValidator()
    {
        RuleFor(e => e.Description)
            .NotEmpty().When(e => e.Description is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
        RuleFor(e => e.StudentId)
            .NotEmpty().WithMessage("Must be not empty or whitespace");
        RuleFor(e => e.AttendanceType)
            .IsInEnum();
    }

    private bool BeFromStartToEnd(TermDto term)
    {
        return term.StartOfTerm < term.EndOfTerm;
    }
}