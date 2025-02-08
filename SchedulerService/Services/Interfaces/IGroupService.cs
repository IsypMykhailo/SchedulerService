using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IGroupService
{
    Task<Group> CreateAsync(GroupDto dto, Institution institution, User headTeacher, CancellationToken ct = default);
    Task<Group?> GetByIdAsync(long id,
        Institution institution,
        Expression<Func<Group, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, Institution institution, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, GroupDto dto, Institution institution, User headTeacher, CancellationToken ct = default);

    Task<bool> IsGroupNameValidAsync(long institutionId, long exceptGroupId, string groupName, CancellationToken ct = default);
    Task<bool> IsGroupNameValidAsync(long institutionId, string groupName, CancellationToken ct = default);
    Task<IEnumerable<Group>> GetAllAsync(Institution institution, CancellationToken ct = default);

    Task<IEnumerable<Group>> GetAllAsync(Institution institution, User headTeacher,
        CancellationToken ct = default);

    Task<IEnumerable<Group>> GetAllAsStudentAsync(Institution institution, string userId, CancellationToken ct);
}