namespace SchedulerService.Domain.Response;

public class StudentGradeResponse
{
    public long Id { get; set; }
    public short Points { get; set; }
    public string? Description { get; set; }
    public GradesJournalColumnResponse Column { get; set; } = default!;
}