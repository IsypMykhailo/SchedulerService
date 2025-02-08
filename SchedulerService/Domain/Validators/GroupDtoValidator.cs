using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class GroupDtoValidator : AbstractValidator<GroupDto>
{
    public GroupDtoValidator()
    {
        RuleFor(e => e.Name)
            .NotEmpty().WithMessage("Must be not empty or whitespace")
            .MaximumLength(50).WithMessage("Maximum number of characters is 50");
        RuleFor(e => e.HeadTeacherId)
            .NotEmpty().WithMessage("Must be not empty or whitespace");
    }
}