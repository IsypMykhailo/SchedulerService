namespace SchedulerService.Domain.Dto;

public class BellsScheduleItemDto
{
    public short LessonIndex { get; set; } = default!;
    public TimeOnly LessonStartTime { get; set; } = default!;
    public TimeOnly LessonEndTime { get; set; } = default!;
}