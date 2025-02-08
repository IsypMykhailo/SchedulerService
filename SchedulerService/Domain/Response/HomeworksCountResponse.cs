namespace SchedulerService.Domain.Response;

public class HomeworksCountResponse
{
    public int Total { get; set; }
    public int Missing { get; set; }
    public int Pending { get; set; }
    public int Done { get; set; }
}