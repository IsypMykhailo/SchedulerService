using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class BellsScheduleDtoValidator : AbstractValidator<BellsScheduleDto>
{
    public BellsScheduleDtoValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
        /*RuleFor(e => e.Term)
            .Must(BeFromStartToEnd).WithMessage("StartOfTerm must be less than EndOfTerm");*/
    }

    /*private bool BeFromStartToEnd(TermDto term)
    {
        return term.StartOfTerm < term.EndOfTerm;
    }*/
}