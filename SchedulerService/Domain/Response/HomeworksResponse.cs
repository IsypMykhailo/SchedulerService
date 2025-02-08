namespace SchedulerService.Domain.Response;

public class HomeworksResponse
{
    public long Id { get; set; }
    
    public string Title { get; set; } = default!;
    
    public List<string> HomeworkPath { get; set; } = default!;

    public string? Description { get; set; } = default!;

    public DateTime UploadedDate { get; set; } = default!;

    public DateTime DueDate { get; set; } = default!;
}