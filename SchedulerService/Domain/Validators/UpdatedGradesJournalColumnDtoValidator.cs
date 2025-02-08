using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class UpdatedGradesJournalColumnDtoValidator : AbstractValidator<UpdatedGradesJournalColumnDto>
{
    public UpdatedGradesJournalColumnDtoValidator()
    {
        RuleFor(e => e.ColumnHeader)
            .NotEmpty().When(e => e.ColumnHeader is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
    }
}