using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface ILessonsScheduleService
{
    Task<LessonsSchedule?> GetAsync(Group group, CancellationToken ct = default);
    Task<LessonsSchedule> CreateAsync(LessonsScheduleDto dto, Group group, BellsSchedule bellsSchedule, CancellationToken ct = default);
    Task<bool> UpdateAsync(Group group, BellsSchedule bellsSchedule, LessonsScheduleDto dto, CancellationToken ct = default);
}