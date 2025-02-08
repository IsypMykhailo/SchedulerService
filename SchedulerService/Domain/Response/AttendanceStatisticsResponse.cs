namespace SchedulerService.Domain.Response;

public class AttendanceStatisticsResponse
{
    public int Month { get; set; }
    public int Year { get; set; }
    public double Percentage { get; set; }
}