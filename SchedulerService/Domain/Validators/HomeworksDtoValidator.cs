using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class HomeworksDtoValidator : AbstractValidator<HomeworksDto>
{
    public HomeworksDtoValidator()
    {
        RuleFor(e => e.Title)
            .NotEmpty().WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
    }
}