using System.Linq.Expressions;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class GroupStudentService : IGroupStudentService
{
    private readonly ICrudRepository<Group> _groupRepository;
    private readonly ICrudRepository<User> _userRepository;

    public GroupStudentService(ICrudRepository<Group> groupRepository, ICrudRepository<User> userRepository)
    {
        _groupRepository = groupRepository;
        _userRepository = userRepository;
    }

    public async Task AddStudentAsync(Group group, string studentId, CancellationToken ct = default)
    {
        if (IsUserStudentInternal(group.Students, studentId))
            return;

        var student = await _userRepository.GetByIdAsync(studentId, ct: ct);

        if (student is null)
        {
            student = new User { Id = studentId };
            await _userRepository.CreateAsync(student, ct);
        }
        
        group.Students.Add(student);
        await _groupRepository.SaveAsync(ct);
    }

    public async Task<bool> RemoveStudentAsync(Group group, string studentId, CancellationToken ct = default)
    {
        if (!IsUserStudentInternal(group.Students, studentId))
            return false;

        if (!group.Students.RemoveWhere(e => e.Id == studentId)) 
            return false;
        
        var subGroups = await _groupRepository.GetAsync(
            e => e.ParentGroupId == group.Id, 
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            },
            ct: ct);

        foreach (var subGroup in subGroups)
            subGroup.Students.RemoveWhere(e => e.Id == studentId);
        
        await _groupRepository.SaveAsync(ct);
        return true;
    }

    public async Task<User?> FindStudentAsync(Group group, string studentId, CancellationToken ct = default)
    {
        return await _userRepository.GetByIdAsync(studentId,
            e => e.Groups.Contains(group),
            ct: ct);
    }

    public async Task<User?> FindStudentAsync(Institution institution, string studentId, CancellationToken ct = default)
    {
        return await _userRepository.GetByIdAsync(studentId,
            e => e.Groups.Any(group => group.Institution.Id == institution.Id),
            ct: ct);
    }

    public async Task<int> GetStudentsCountAsync(Institution institution, CancellationToken ct)
    {
        return (await _groupRepository.GetAsync(e => e.Institution.Id == institution.Id && e.ParentGroup == null,
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            }, ct)).SelectMany(e => e.Students).Count();
    }

    private bool IsUserStudentInternal(IEnumerable<User> students, string userId)
    {
        foreach (var student in students)
            if (student.Id == userId)
                return true;

        return false;
    }
}