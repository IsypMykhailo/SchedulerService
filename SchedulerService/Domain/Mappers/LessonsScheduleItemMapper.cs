using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class LessonsScheduleItemMapper
{
    public static LessonsScheduleItemResponse ToResponse(LessonsScheduleItem obj)
    {
        return new LessonsScheduleItemResponse()
        {
            Id = obj.Id,
            Date = obj.Date,
            LessonIndex = obj.LessonIndex,
            TeacherId = obj.TeacherId,
            SubGroup = obj.SubGroup != null ? GroupMapper.ToResponse(obj.SubGroup) : null,
            Subject = SubjectMapper.ToResponse(obj.Subject),
            Theme = obj.Theme,
            HomeworkDescription = obj.HomeworkDescription
        };
    }

    public static IEnumerable<LessonsScheduleItemResponse> ToResponses(IEnumerable<LessonsScheduleItem> objEnumerable)
    {
        var items = objEnumerable as LessonsScheduleItem[] ?? objEnumerable.ToArray();
        var responses = new LessonsScheduleItemResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}