namespace SchedulerService.Domain.Response;

public class AttendanceResponse
{
    public long Id { get; set; }
    public string StudentId { get; set; } = default!;
    public string? Description { get; set; }
    public AttendanceType AttendanceType { get; set; }
}