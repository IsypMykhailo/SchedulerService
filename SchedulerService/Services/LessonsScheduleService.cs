using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class LessonsScheduleService : ILessonsScheduleService
{
    private readonly ICrudRepository<LessonsSchedule> _lessonsScheduleRepository;

    public LessonsScheduleService(ICrudRepository<LessonsSchedule> lessonsScheduleRepository)
    {
        _lessonsScheduleRepository = lessonsScheduleRepository;
    }

    public async Task<LessonsSchedule?> GetAsync(Group group, CancellationToken ct = default)
    {
        return (await _lessonsScheduleRepository
                .GetAsync(e => e.Group.Id == group.Id || (group.ParentGroupId != null && e.Group.Id == group.ParentGroupId),
                    new Expression<Func<LessonsSchedule, object?>>[]
                    {
                        e => e.BellsSchedule,
                    },
                    ct: ct))
                .FirstOrDefault();
    }
    
    public async Task<LessonsSchedule> CreateAsync(LessonsScheduleDto dto, Group group, BellsSchedule bellsSchedule, CancellationToken ct = default)
    {
        var lessonSchedule = dto.Adapt<LessonsScheduleDto, LessonsSchedule>();
        lessonSchedule.Group = group;
        lessonSchedule.BellsSchedule = bellsSchedule;

        await _lessonsScheduleRepository.CreateAsync(lessonSchedule, ct);
        await _lessonsScheduleRepository.SaveAsync(ct);
        return lessonSchedule;
    }

    public async Task<bool> UpdateAsync(Group group, BellsSchedule bellsSchedule, LessonsScheduleDto dto, CancellationToken ct = default)
    {
        if (group.LessonsScheduleId is null)
            return false;
        
        var lessonsSchedule = await _lessonsScheduleRepository.GetByIdAsync(group.LessonsScheduleId, ct: ct);

        if (lessonsSchedule is null)
            return false;
        
        var updatedLessonsSchedule = dto.Adapt<LessonsScheduleDto, LessonsSchedule>();
        updatedLessonsSchedule.Group = group;
        updatedLessonsSchedule.BellsSchedule = bellsSchedule;
        updatedLessonsSchedule.Id = lessonsSchedule.Id;

        await _lessonsScheduleRepository.UpdateAsync(updatedLessonsSchedule.Id, updatedLessonsSchedule, ct);
        await _lessonsScheduleRepository.SaveAsync(ct);
        return true;
    }
}