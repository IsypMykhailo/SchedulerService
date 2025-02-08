using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class BellsScheduleItemMapper
{
    public static BellsScheduleItemResponse ToResponse(BellsScheduleItem obj)
    {
        return new()
        {
            Id = obj.Id,
            LessonIndex = obj.LessonIndex,
            DayOfWeek = obj.DayOfWeek,
            LessonStartTime = obj.LessonStartTime,
            LessonEndTime = obj.LessonEndTime
        };
    }

    public static IEnumerable<BellsScheduleItemResponse> ToResponses(IEnumerable<BellsScheduleItem> objEnumerable)
    {
        var items = objEnumerable as BellsScheduleItem[] ?? objEnumerable.ToArray();
        var responses = new BellsScheduleItemResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}