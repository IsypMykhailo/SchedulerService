namespace SchedulerService.Domain.Dto;

public class SingleLessonsScheduleItemDto
{
    public string? TeacherId { get; set; }
    public long? SubGroupId { get; set; }
    public int? LessonIndex { get; set; }
    public long SubjectId { get; set; }
    public DateOnly Date { get; set; }
}