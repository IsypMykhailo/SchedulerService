using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class AttendanceMapper
{
    public static AttendanceResponse ToResponse(Attendance obj)
    {
        return new()
        {
            Id = obj.Id,
            Description = obj.Description,
            AttendanceType = obj.AttendanceType,
            StudentId = obj.StudentId
        };
    }

    public static IEnumerable<AttendanceResponse> ToResponses(IEnumerable<Attendance> objEnumerable)
    {
        var items = objEnumerable as Attendance[] ?? objEnumerable.ToArray();
        var responses = new AttendanceResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }

    public static Attendance ToEntity(AttendanceDto dto, LessonsScheduleItem lesson, long? id = null)
    {
        var entity = new Attendance
        {
            StudentId = dto.StudentId,
            Description = dto.Description,
            Lesson = lesson,
            AttendanceType = dto.AttendanceType
        };

        if (id is not null)
            entity.Id = (long)id;

        return entity;
    }
}