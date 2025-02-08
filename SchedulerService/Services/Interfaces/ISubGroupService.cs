using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface ISubGroupService
{
    Task<Group> CreateAsync(SubGroupDto dto, Group group, Institution institution, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetAllAsync(Group group, CancellationToken ct = default);
    Task<Group?> GetByIdAsync(long id,
        Group group,
        Expression<Func<Group, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, Group group, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, SubGroupDto dto, Group group, Institution institution, CancellationToken ct = default);
    Task<bool> IsSubGroupNameValidAsync(Group group, string subGroupName, CancellationToken ct = default);
    Task<bool> IsSubGroupNameValidAsync(Group group, long exceptSubGroupId, string subGroupName, CancellationToken ct = default);
}