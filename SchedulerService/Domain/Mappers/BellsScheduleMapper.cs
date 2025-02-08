using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class BellsScheduleMapper
{
    public static BellsScheduleResponse ToResponse(BellsSchedule obj)
    {
        return new()
        {
            Id = obj.Id,
            Name = obj.Name,
        };
    }

    public static IEnumerable<BellsScheduleResponse> ToResponses(IEnumerable<BellsSchedule> objEnumerable)
    {
        var items = objEnumerable as BellsSchedule[] ?? objEnumerable.ToArray();
        var responses = new BellsScheduleResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}