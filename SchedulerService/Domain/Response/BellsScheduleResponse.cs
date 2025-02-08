namespace SchedulerService.Domain.Response;

public class BellsScheduleResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public TermResponse Term { get; set; } = default!;
}