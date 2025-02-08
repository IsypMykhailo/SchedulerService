namespace SchedulerService.Domain.Response;

public class GradesJournalColumnResponse
{
    public long Id { get; set; }
    public string? ColumnHeader { get; set; }
    public LessonsScheduleItemResponse? Lesson { get; set; }
    public DateOnly? Date { get; set; }
    public HomeworksResponse? Homework { get; set; }
    public IEnumerable<GradeResponse> Grades { get; set; } = default!;
}