namespace SchedulerService.Domain.Response;

public class TeacherLessonResponse
{
    public long Id { get; set; }
    public GroupResponse Group { get; set; } = default!;
    public GroupResponse? SubGroup { get; set; }
    public int? LessonIndex { get; set; }
    public SubjectResponse Subject { get; set; } = default!;
    public DateOnly Date { get; set; }
    public string? Theme { get; set; }
    public string? HomeworkDescription { get; set; }
}