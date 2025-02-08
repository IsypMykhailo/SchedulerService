namespace SchedulerService.Domain.Response;

public class HomeworkItemsResponse
{
    public long Id { get; set; }
    
    public List<string> CompletedHomework { get; set; } = default!;

    public DateTime HomeworkUploaded { get; set; } = default!;

    public DateTime? HomeworkUpdated { get; set; } = default!;

    public string? Comment { get; set; } = default!;
    
    public bool BeforeDueDate { get; set; }

    public string StudentId { get; set; } = default!;
}