namespace SchedulerService.Domain.Dto;

public class HomeworksDto
{
    public string Title { get; set; } = default!;
    
    public List<string> HomeworkPath { get; set; } = default!;

    public string? Description { get; set; } = default!;

    public DateTime DueDate { get; set; } = default!;
    
}