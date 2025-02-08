using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IBellsScheduleItemService
{
    Task<bool> DeleteAsync(long id, BellsSchedule bellsSchedule, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, BellsScheduleItemDto dto,
        BellsSchedule bellsSchedule, CancellationToken ct = default);

    Task<IEnumerable<BellsScheduleItem>> CreateForDaysAsync(BellsScheduleItemDto dto,
        BellsSchedule bellsSchedule, DayOfWeek[] days, CancellationToken ct = default);

    Task<bool> ValidateIndexesAsync(BellsSchedule bellsSchedule, BellsScheduleItemDto dto, DayOfWeek[] days, CancellationToken ct);
    Task<BellsScheduleItem?> GetByIdAsync(long id, BellsSchedule bellsSchedule, CancellationToken ct);
}