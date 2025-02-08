using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public class HomeworkItemsMapper
{
    public static HomeworkItemsResponse ToResponse(HomeworkItems obj)
    {
        return new()
        {
            Id = obj.Id,
            CompletedHomework = obj.CompletedHomework,
            HomeworkUploaded = obj.HomeworkUploaded,
            HomeworkUpdated = obj.HomeworkUpdated,
            Comment = obj.Comment,
            BeforeDueDate = obj.BeforeDueDate,
            StudentId = obj.StudentId
        };
    }

    public static IEnumerable<HomeworkItemsResponse> ToResponses(IEnumerable<HomeworkItems> objEnumerable)
    {
        var items = objEnumerable as HomeworkItems[] ?? objEnumerable.ToArray();
        var responses = new HomeworkItemsResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}