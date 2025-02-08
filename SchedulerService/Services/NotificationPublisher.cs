using Amazon.SQS;
using Amazon.SQS.Model;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using SchedulerService.Domain.Notifications;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class NotificationPublisher : INotificationPublisher
{
    private readonly IAmazonSQS _sqs;
    private readonly string _notificationsQueue;
    private readonly JsonSerializerSettings _jsonSerializerSettings;

    public NotificationPublisher(IAmazonSQS sqs, IConfiguration configuration)
    {
        _sqs = sqs;
        _notificationsQueue = configuration.GetValue<string>("Sqs:Url");
        _jsonSerializerSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.Indented
        };
    }

    public async Task PublishAsync(INotification notification)
    {
        await _sqs.SendMessageAsync(new SendMessageRequest()
        {
            QueueUrl = _notificationsQueue,
            MessageBody = JsonConvert.SerializeObject(notification, _jsonSerializerSettings),
            MessageGroupId = Guid.NewGuid().ToString()
        } );
    }
}