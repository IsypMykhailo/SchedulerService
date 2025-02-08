namespace SchedulerService.Domain.Dto;

public class GradesJournalColumnDto
{
    public string? ColumnHeader { get; set; }
    public long? LessonId { get; set; }
    public long? HomeworkId { get; set; }
    public DateOnly? Date { get; set; }
}