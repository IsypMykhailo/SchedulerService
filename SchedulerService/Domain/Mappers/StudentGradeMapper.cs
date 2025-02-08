using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class StudentGradeMapper
{
    public static StudentGradeResponse ToResponse(Grade obj)
    {
        return new()
        {
            Id = obj.Id,
            Description = obj.Description,
            Points = obj.Points,
            Column = GradesJournalColumnMapper.ToResponse(obj.JournalColumn),
        };
    }

    public static IEnumerable<StudentGradeResponse> ToResponses(IEnumerable<Grade> objEnumerable)
    {
        var items = objEnumerable as Grade[] ?? objEnumerable.ToArray();
        var responses = new StudentGradeResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}