namespace SchedulerService.Domain.Response;

public class LessonsScheduleItemResponse
{
    public long Id { get; set; }
    public string? TeacherId { get; set; }
    public GroupResponse? SubGroup { get; set; }
    public int? LessonIndex { get; set; }
    public SubjectResponse Subject { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string? Theme { get; set; }
    public string? HomeworkDescription { get; set; }
}