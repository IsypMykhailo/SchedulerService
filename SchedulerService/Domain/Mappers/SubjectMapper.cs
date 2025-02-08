using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class SubjectMapper
{
    public static SubjectResponse ToResponse(Subject obj)
    {
        return new SubjectResponse()
        {
            Id = obj.Id,
            Name = obj.Name
        };
    }

    public static IEnumerable<SubjectResponse> ToResponses(IEnumerable<Subject> objEnumerable)
    {
        var items = objEnumerable as Subject[] ?? objEnumerable.ToArray();
        var responses = new SubjectResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}