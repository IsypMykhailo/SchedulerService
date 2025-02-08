namespace SchedulerService.Domain.Response;

public class LessonsScheduleResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public long BellsScheduleId { get; set; }
}