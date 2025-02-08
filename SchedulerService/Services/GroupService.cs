using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class GroupService : IGroupService
{
    private readonly ICrudRepository<Group> _groupRepository;
    private readonly ICrudRepository<LessonsScheduleItem> _lessonsService;
    private readonly ICrudRepository<LessonsSchedule> _lessonsScheduleService;

    public GroupService(ICrudRepository<Group> groupRepository, ICrudRepository<LessonsScheduleItem> lessonsService, ICrudRepository<LessonsSchedule> lessonsScheduleService)
    {
        _groupRepository = groupRepository;
        _lessonsService = lessonsService;
        _lessonsScheduleService = lessonsScheduleService;
    }

    public async Task<IEnumerable<Group>> GetAllAsync(Institution institution, CancellationToken ct = default)
    {
        return await _groupRepository.GetAsync(e => e.Institution.Id == institution.Id && e.ParentGroup == null, ct: ct);
    }

    public async Task<IEnumerable<Group>> GetAllAsync(Institution institution, User headTeacher,
        CancellationToken ct = default)
    {
        return await _groupRepository.GetAsync(e => e.Institution.Id == institution.Id && e.HeadTeacher.Id == headTeacher.Id && e.ParentGroup == null, ct: ct);
    }

    public async Task<IEnumerable<Group>> GetAllAsStudentAsync(Institution institution, string userId,
        CancellationToken ct = default)
    {
        return await _groupRepository.GetAsync(e => e.Institution.Id == institution.Id && e.Students.Any(s => s.Id == userId) && e.ParentGroup == null, ct: ct);
    }

    public async Task<Group> CreateAsync(GroupDto dto, Institution institution, User headTeacher, CancellationToken ct = default)
    {
        var group = dto.Adapt<GroupDto, Group>();
        group.HeadTeacher = headTeacher;
        group.Institution = institution;

        await _groupRepository.CreateAsync(group, ct);
        await _groupRepository.SaveAsync(ct);
        return group;
    }

    public async Task<Group?> GetByIdAsync(long id, Institution institution,
        Expression<Func<Group, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _groupRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id,
            includeProperties, ct);
    }
    
    public async Task<bool> DeleteAsync(long id, Institution institution, CancellationToken ct = default)
    {
        var group = await _groupRepository.GetByIdAsync(id,
            e => e.Institution.Id == institution.Id,
            ct: ct);

        if (group is null)
            return false;
        
        var lessons = await _lessonsService.GetAsync(e => e.LessonsSchedule.Group.Id == group.Id, ct: ct);
        await _lessonsService.DeleteRangeAsync(lessons, ct);
        
        var lessonSchedules = await _lessonsScheduleService.GetAsync(e => e.Group.Id == group.Id, ct: ct);
        await _lessonsScheduleService.DeleteRangeAsync(lessonSchedules, ct);

        var subGroups = await _groupRepository.GetAsync(e => e.ParentGroupId == group.Id, ct: ct);
        await _groupRepository.DeleteRangeAsync(subGroups, ct);
            
        await _groupRepository.DeleteAsync(group, ct);
        await _groupRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, GroupDto dto, Institution institution, User headTeacher, CancellationToken ct = default)
    {
        var group = await _groupRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id,
            new Expression<Func<Group, object?>>[]
            {
                e => e.SubGroups  
            },
            ct: ct);

        if (group is null)
            return false;
        
        var updatedGroup = dto.Adapt<GroupDto, Group>();
        updatedGroup.HeadTeacher = headTeacher;
        updatedGroup.Institution = institution;
        updatedGroup.LessonsScheduleId = group.LessonsScheduleId;
        updatedGroup.Id = group.Id;
        updatedGroup.SubGroups = group.SubGroups;

        foreach (var subGroup in updatedGroup.SubGroups)
        {
            subGroup.HeadTeacher = headTeacher;
        }

        await _groupRepository.UpdateAsync(updatedGroup.Id, updatedGroup, ct);
        await _groupRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> IsGroupNameValidAsync(long institutionId, string groupName, CancellationToken ct = default)
    {
        var groups = await _groupRepository.GetAsync(
            e => e.Name == groupName && e.Institution.Id == institutionId,
            new Expression<Func<Group, object?>>[]
            {
                e => e.Institution
            }, ct);

        return !groups.Any();
    }

    public async Task<bool> IsGroupNameValidAsync(long institutionId, long exceptGroupId, string groupName, CancellationToken ct = default)
    {
        var groups = await _groupRepository.GetAsync(
            e => e.Name == groupName && e.Institution.Id == institutionId && e.Id != exceptGroupId,
            new Expression<Func<Group, object?>>[]
            {
                e => e.Institution
            }, ct);

        return !groups.Any();
    }
}