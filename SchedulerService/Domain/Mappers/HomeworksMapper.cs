using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using Microsoft.EntityFrameworkCore.Storage;

namespace SchedulerService.Domain.Mappers;

public class HomeworksMapper
{
    public static HomeworksResponse ToResponse(Homeworks obj)
    {
        return new()
        {
            Id = obj.Id,
            Title = obj.Title,
            HomeworkPath = obj.HomeworkPath,
            Description = obj.Description,
            UploadedDate = obj.UploadedDate,
            DueDate = obj.DueDate
        };
    }

    public static IEnumerable<HomeworksResponse> ToResponses(IEnumerable<Homeworks> objEnumerable)
    {
        var items = objEnumerable as Homeworks[] ?? objEnumerable.ToArray();
        var responses = new HomeworksResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}