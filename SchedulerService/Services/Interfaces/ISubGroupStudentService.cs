using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface ISubGroupStudentService
{
    Task AddStudent(Group subGroup, User student, CancellationToken ct = default);
    Task<bool> RemoveStudent(Group subGroup, string studentId, CancellationToken ct = default);
}