using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class UpdatedGradesJournalDtoValidator : AbstractValidator<UpdatedGradesJournalDto>
{
    public UpdatedGradesJournalDtoValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty().When(e => e.Name is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
    }
}