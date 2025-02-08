using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class SubGroupService : ISubGroupService
{
    private readonly ICrudRepository<Group> _subGroupRepository;

    public SubGroupService(ICrudRepository<Group> subGroupRepository)
    {
        _subGroupRepository = subGroupRepository;
    }
    
    public async Task<Group> CreateAsync(SubGroupDto dto, Group group, Institution institution, CancellationToken ct = default)
    {
        var subGroup = dto.Adapt<SubGroupDto, Group>();
        subGroup.ParentGroup = group;
        subGroup.HeadTeacherId = group.HeadTeacherId;
        subGroup.Institution = institution;

        await _subGroupRepository.CreateAsync(subGroup, ct);
        await _subGroupRepository.SaveAsync(ct);
        return subGroup;
    }

    public async Task<IEnumerable<Group>> GetAllAsync(Group group, CancellationToken ct = default)
    {
        return await _subGroupRepository.GetAsync(e => e.ParentGroup != null && e.ParentGroup.Id == group.Id, ct: ct);
    }

    public async Task<Group?> GetByIdAsync(long id, Group group, Expression<Func<Group, object?>>[]? includeProperties = null, CancellationToken ct = default)
    {
        return await _subGroupRepository.GetByIdAsync(id,
            e => e.ParentGroup != null && e.ParentGroup.Id == group.Id,
            includeProperties, ct);
    }

    public async Task<bool> DeleteAsync(long id, Group group, CancellationToken ct = default)
    {
        var subGroup = await _subGroupRepository.GetByIdAsync(id, 
            e => e.ParentGroup != null && e.ParentGroup.Id == group.Id,
            ct: ct);

        if (subGroup is null)
            return false;
        
        await _subGroupRepository.DeleteAsync(subGroup, ct);
        await _subGroupRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, SubGroupDto dto, Group group, Institution institution, CancellationToken ct = default)
    {
        var subGroup = await _subGroupRepository.GetByIdAsync(id,
            e => e.ParentGroup != null && e.ParentGroup.Id == group.Id,
            ct: ct);

        if (subGroup is null)
            return false;
        
        var updatedSubGroup = dto.Adapt<SubGroupDto, Group>();
        updatedSubGroup.ParentGroup = group;
        updatedSubGroup.ParentGroupId = group.Id;
        updatedSubGroup.Id = subGroup.Id;
        updatedSubGroup.HeadTeacherId = group.HeadTeacherId;
        updatedSubGroup.Institution = institution;

        await _subGroupRepository.UpdateAsync(updatedSubGroup.Id, updatedSubGroup, ct);
        await _subGroupRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> IsSubGroupNameValidAsync(Group group, string subGroupName, CancellationToken ct = default)
    {
        var groups = await _subGroupRepository.GetAsync(
            e => e.ParentGroup != null && e.Name == subGroupName && e.ParentGroup.Id == group.Id, 
            ct: ct);

        return !groups.Any();
    }

    public async Task<bool> IsSubGroupNameValidAsync(Group group, long exceptSubGroupId, string subGroupName, CancellationToken ct = default)
    {
        var groups = await _subGroupRepository.GetAsync(
            e => e.ParentGroup != null && e.Name == subGroupName && e.ParentGroup.Id == group.Id && e.Id != exceptSubGroupId, 
            ct: ct);

        return !groups.Any();
    }
}