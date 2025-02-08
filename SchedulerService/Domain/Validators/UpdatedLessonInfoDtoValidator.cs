using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class UpdatedLessonInfoDtoValidator : AbstractValidator<UpdatedLessonInfoDto>
{
    public UpdatedLessonInfoDtoValidator()
    {
        RuleFor(e => e.Theme)
            .NotEmpty().When(e => e.Theme is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(250).WithMessage("Maximum number of characters is 250");
        RuleFor(e => e.HomeworkDescription)
            .NotEmpty().When(e => e.HomeworkDescription is not null).WithMessage("Must be not empty or whitespace")
            .MaximumLength(250).WithMessage("Maximum number of characters is 250");
    }
}