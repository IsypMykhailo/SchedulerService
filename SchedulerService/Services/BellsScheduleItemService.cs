using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class BellsScheduleItemService : IBellsScheduleItemService
{
    private readonly ICrudRepository<BellsScheduleItem> _bellsScheduleItemRepository;

    public BellsScheduleItemService(ICrudRepository<BellsScheduleItem> bellsScheduleItemRepository)
    {
        _bellsScheduleItemRepository = bellsScheduleItemRepository;
    }

    public async Task<IEnumerable<BellsScheduleItem>> CreateForDaysAsync(BellsScheduleItemDto dto,
        BellsSchedule bellsSchedule, DayOfWeek[] days, CancellationToken ct = default)
    {
        var bellsScheduleItems = new BellsScheduleItem[days.Length];

        for (var i = 0; i < days.Length; i++)
        {
            bellsScheduleItems[i] = dto.Adapt<BellsScheduleItemDto, BellsScheduleItem>();
            bellsScheduleItems[i].Schedule = bellsSchedule;
            bellsScheduleItems[i].DayOfWeek = days[i];
            await _bellsScheduleItemRepository.CreateAsync(bellsScheduleItems[i], ct);
        }

        await _bellsScheduleItemRepository.SaveAsync(ct);
        return bellsScheduleItems;
    }

    public async Task<bool> ValidateIndexesAsync(BellsSchedule bellsSchedule, BellsScheduleItemDto dto, DayOfWeek[] days, CancellationToken ct)
    {
        return !(await _bellsScheduleItemRepository.GetAsync(
            e => e.Schedule.Id == bellsSchedule.Id && days.Contains(e.DayOfWeek) && e.LessonIndex == dto.LessonIndex, ct: ct))
            .Any();
    }

    public async Task<BellsScheduleItem?> GetByIdAsync(long id, BellsSchedule bellsSchedule, CancellationToken ct)
    {
        return await _bellsScheduleItemRepository.GetByIdAsync(id, 
            e => e.Schedule.Id == bellsSchedule.Id,
            ct: ct);
    }

    public async Task<bool> DeleteAsync(long id, BellsSchedule bellsSchedule, CancellationToken ct = default)
    {
        var bellsScheduleItem = await _bellsScheduleItemRepository.GetByIdAsync(id, 
            e => e.Schedule.Id == bellsSchedule.Id,
            ct: ct);

        if (bellsScheduleItem is null)
            return false;
        
        await _bellsScheduleItemRepository.DeleteAsync(bellsScheduleItem, ct);
        await _bellsScheduleItemRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> UpdateAsync(long id, BellsScheduleItemDto dto,
        BellsSchedule bellsSchedule, CancellationToken ct = default)
    {
        var bellsScheduleItem = await _bellsScheduleItemRepository.GetByIdAsync(id,
            e => e.Schedule.Id == bellsSchedule.Id,
            ct: ct);

        if (bellsScheduleItem is null)
            return false;
        
        var updatedBellsScheduleItem = dto.Adapt<BellsScheduleItemDto, BellsScheduleItem>();
        updatedBellsScheduleItem.Schedule = bellsSchedule;
        updatedBellsScheduleItem.Id = bellsScheduleItem.Id;

        await _bellsScheduleItemRepository.UpdateAsync(updatedBellsScheduleItem.Id, updatedBellsScheduleItem, ct);
        await _bellsScheduleItemRepository.SaveAsync(ct);
        return true;
    }
}