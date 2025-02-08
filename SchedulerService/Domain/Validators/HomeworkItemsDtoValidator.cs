using FluentValidation;
using SchedulerService.Domain.Dto;

namespace SchedulerService.Domain.Validators;

public class HomeworkItemsDtoValidator : AbstractValidator<HomeworkItemsDto>
{
    public HomeworkItemsDtoValidator()
    {
    }
}