namespace SchedulerService.Domain.Dto;

public class RangeLessonsScheduleItemDto
{
    public string? TeacherId { get; set; }
    public long? SubGroupId { get; set; }
    public int? LessonIndex { get; set; }
    public long SubjectId { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public DateOnly From { get; set; }
    public DateOnly To { get; set; }
}