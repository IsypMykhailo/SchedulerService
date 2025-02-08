namespace SchedulerService.Domain.Dto;

public class HomeworkItemsDto
{
    public List<string> CompletedHomework { get; set; } = default!;

    public string? Comment { get; set; } = default!;
}