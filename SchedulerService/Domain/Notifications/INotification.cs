namespace SchedulerService.Domain.Notifications;

public interface INotification
{
    string Type { get; }
    object State { get; }
    string ReceiverId { get; }
    long? InstitutionId { get; set; }
}