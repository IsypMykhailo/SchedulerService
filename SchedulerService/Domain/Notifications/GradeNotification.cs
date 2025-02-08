namespace SchedulerService.Domain.Notifications;

public class GradeNotification : INotification
{
    public string Type { get; } = "Grade";
    public object State { get; init; } = default!;
    public string ReceiverId { get; init; } = default!;
    public long? InstitutionId { get; set; }
}