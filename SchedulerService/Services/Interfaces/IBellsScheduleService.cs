using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IBellsScheduleService
{
    Task<BellsSchedule> CreateAsync(BellsScheduleDto dto, Institution institution, CancellationToken ct = default);
    Task<BellsSchedule?> GetByIdAsync(long id,
        Institution institution,
        Expression<Func<BellsSchedule, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, Institution institution, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, BellsScheduleDto dto, Institution institution, CancellationToken ct = default);
    Task<IEnumerable<BellsSchedule>> GetAllAsync(Institution institution, CancellationToken ct = default);
}