using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IGroupStudentService
{
    Task AddStudentAsync(Group group, string studentId, CancellationToken ct = default);
    Task<bool> RemoveStudentAsync(Group group, string studentId, CancellationToken ct = default);
    Task<User?> FindStudentAsync(Group group, string studentId, CancellationToken ct = default);
    Task<User?> FindStudentAsync(Institution institution, string studentId, CancellationToken ct = default);
    Task<int> GetStudentsCountAsync(Institution institution, CancellationToken ct);
}