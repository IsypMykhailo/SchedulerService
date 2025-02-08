namespace SchedulerService.Domain.Dto;

public class GradeDto
{
    public string StudentId { get; set; } = default!;
    public short? Points { get; set; }
    public string? Description { get; set; }
}