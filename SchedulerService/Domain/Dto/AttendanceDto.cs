namespace SchedulerService.Domain.Dto;

public class AttendanceDto
{
    public string StudentId { get; set; } = default!;
    public string? Description { get; set; }
    public AttendanceType AttendanceType { get; set; }
}