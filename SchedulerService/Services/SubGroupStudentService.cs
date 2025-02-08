using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class SubGroupStudentService : ISubGroupStudentService
{
    private readonly ICrudRepository<Group> _subGroupRepository;

    public SubGroupStudentService(ICrudRepository<Group> subGroupRepository)
    {
        _subGroupRepository = subGroupRepository;
    }

    public async Task AddStudent(Group subGroup, User student, CancellationToken ct = default)
    {
        if (IsUserStudentInternal(subGroup.Students, student.Id))
            return;
        
        subGroup.Students.Add(student);
        await _subGroupRepository.SaveAsync(ct);
    }

    public async Task<bool> RemoveStudent(Group subGroup, string studentId, CancellationToken ct = default)
    {
        if (!IsUserStudentInternal(subGroup.Students, studentId))
            return false;

        if (!subGroup.Students.RemoveWhere(e => e.Id == studentId)) 
            return false;
        
        await _subGroupRepository.SaveAsync(ct);
        return true;
    }

    private bool IsUserStudentInternal(IEnumerable<User> students, string userId)
    {
        foreach (var student in students)
            if (student.Id == userId)
                return true;

        return false;
    }
}