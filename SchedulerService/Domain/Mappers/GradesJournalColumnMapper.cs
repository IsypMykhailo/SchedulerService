using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class GradesJournalColumnMapper
{
    public static GradesJournalColumnResponse ToResponse(GradesJournalColumn obj)
    {
        return new()
        {
            Id = obj.Id,
            Date = obj.Date,
            Lesson = obj.Lesson is null ? null : LessonsScheduleItemMapper.ToResponse(obj.Lesson),
            ColumnHeader = obj.ColumnHeader,
            Homework = obj.Homework is null ? null : HomeworksMapper.ToResponse(obj.Homework),
            Grades = GradeMapper.ToResponses(obj.Grades)
        };
    }

    public static IEnumerable<GradesJournalColumnResponse> ToResponses(IEnumerable<GradesJournalColumn> objEnumerable)
    {
        var items = objEnumerable as GradesJournalColumn[] ?? objEnumerable.ToArray();
        var responses = new GradesJournalColumnResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}