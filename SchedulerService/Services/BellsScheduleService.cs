using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class BellsScheduleService : IBellsScheduleService
{
    private readonly ICrudRepository<BellsSchedule> _bellsScheduleRepository;

    public BellsScheduleService(ICrudRepository<BellsSchedule> bellsScheduleRepository)
    {
        _bellsScheduleRepository = bellsScheduleRepository;
    }

    public async Task<IEnumerable<BellsSchedule>> GetAllAsync(Institution institution, CancellationToken ct = default)
    {
        return await _bellsScheduleRepository.GetAsync(e => e.Institution.Id == institution.Id, ct: ct);
    }

    public async Task<BellsSchedule> CreateAsync(BellsScheduleDto dto, Institution institution, CancellationToken ct = default)
    {
        var bellSchedule = dto.Adapt<BellsScheduleDto, BellsSchedule>();
        /*bellSchedule.ScheduleTerm = dto.Term.Adapt<TermDto, Term>();*/
        bellSchedule.Institution = institution;

        await _bellsScheduleRepository.CreateAsync(bellSchedule, ct);
        await _bellsScheduleRepository.SaveAsync(ct);
        return bellSchedule;
    }
    
    public async Task<BellsSchedule?> GetByIdAsync(long id, Institution institution,
        Expression<Func<BellsSchedule, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _bellsScheduleRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id,
            includeProperties,
            ct);
    }

    public async Task<bool> DeleteAsync(long id, Institution institution, CancellationToken ct = default)
    {
        var bellsSchedule = await _bellsScheduleRepository.GetByIdAsync(id,
            e => e.Institution.Id == institution.Id,
            ct: ct);

        if (bellsSchedule is null)
            return false;
        
        await _bellsScheduleRepository.DeleteAsync(bellsSchedule, ct);
        await _bellsScheduleRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, BellsScheduleDto dto, Institution institution, CancellationToken ct = default)
    {
        var bellsSchedule = await _bellsScheduleRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id,
            ct: ct);

        if (bellsSchedule is null)
            return false;
        
        var updatedBellsSchedule = dto.Adapt<BellsScheduleDto, BellsSchedule>();
        /*updatedBellsSchedule.ScheduleTerm = dto.Term.Adapt<TermDto, Term>();*/
        updatedBellsSchedule.Institution = institution;
        updatedBellsSchedule.Id = bellsSchedule.Id;

        await _bellsScheduleRepository.UpdateAsync(updatedBellsSchedule.Id, updatedBellsSchedule, ct);
        await _bellsScheduleRepository.SaveAsync(ct);
        return true;
    }
}