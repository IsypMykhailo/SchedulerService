using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class GradeMapper
{
    public static GradeResponse ToResponse(Grade obj)
    {
        return new()
        {
            Id = obj.Id,
            Description = obj.Description,
            Points = obj.Points,
            StudentId = obj.StudentId,
        };
    }

    public static IEnumerable<GradeResponse> ToResponses(IEnumerable<Grade> objEnumerable)
    {
        var items = objEnumerable as Grade[] ?? objEnumerable.ToArray();
        var responses = new GradeResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}