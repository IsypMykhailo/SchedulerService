using SchedulerService.Domain.Notifications;

namespace SchedulerService.Services.Interfaces;

public interface INotificationPublisher
{
    Task PublishAsync(INotification notification);
}