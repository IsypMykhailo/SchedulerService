namespace SchedulerService.Domain.Response;

public class GradeResponse
{
    public long Id { get; set; }
    public string StudentId { get; set; } = default!;
    public short Points { get; set; }
    public string? Description { get; set; }
}