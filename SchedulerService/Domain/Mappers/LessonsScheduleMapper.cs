using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class LessonsScheduleMapper
{
    public static LessonsScheduleResponse ToResponse(LessonsSchedule obj)
    {
        return new()
        {
            Id = obj.Id,
            Name = obj.Name,
            BellsScheduleId = obj.BellsSchedule.Id
        };
    }
}