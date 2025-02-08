using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface ITeacherService
{
    Task<User?> FindTeacherWithSubjectAsync(Institution institution, Subject subject, string teacherId, CancellationToken ct = default);
    Task AddTeacherAsync(Institution institution, string teacherId, CancellationToken ct = default);
    Task<bool> RemoveTeacherAsync(Institution institution, string teacherId, CancellationToken ct = default);
    Task<User?> GetByIdAsync(string teacherId,
        Institution institution,
        CancellationToken ct = default);
    Task<User?> FindTeacherAsync(Institution institution, string teacherId, CancellationToken ct = default);
    Task<bool> IsUserHeadTeacherAsync(string teacherId, CancellationToken ct);
}