using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class TeacherLessonMapper
{
    public static TeacherLessonResponse ToResponse(LessonsScheduleItem obj)
    {
        return new TeacherLessonResponse()
        {
            Id = obj.Id,
            Date = obj.Date,
            LessonIndex = obj.LessonIndex,
            Group = GroupMapper.ToResponse(obj.LessonsSchedule.Group),
            SubGroup = obj.SubGroup != null ? GroupMapper.ToResponse(obj.SubGroup) : null,
            Subject = SubjectMapper.ToResponse(obj.Subject),
            Theme = obj.Theme,
            HomeworkDescription = obj.HomeworkDescription
        };
    }

    public static IEnumerable<TeacherLessonResponse> ToResponses(IEnumerable<LessonsScheduleItem> objEnumerable)
    {
        var items = objEnumerable as LessonsScheduleItem[] ?? objEnumerable.ToArray();
        var responses = new TeacherLessonResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}