namespace SchedulerService.Domain.Response;

public class BellsScheduleItemResponse
{
    public long Id { get; set; }
    public short LessonIndex { get; set; }
    public DayOfWeek DayOfWeek { get; set; }
    public TimeOnly LessonStartTime { get; set; }
    public TimeOnly LessonEndTime { get; set; }
}