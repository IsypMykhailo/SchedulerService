using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IAdministratorService
{
    Task<bool> IsUserAdminOrOwnerAsync(Institution institution, string userId, CancellationToken ct = default);
    Task<bool> IsUserHeadTeacherOrAdminOrOwnerAsync(Institution institution, string userId, Group group, CancellationToken ct = default);
    Task<bool> IsUserBelongsToInstitutionAsync(Institution institution, string userId, CancellationToken ct = default);
    Task AddAdministratorAsync(Institution institution, string adminId, CancellationToken ct = default);
    Task<bool> RemoveAdministratorAsync(Institution institution, string adminId, CancellationToken ct = default);

    Task<bool> IsUserTeacherOrAdminOrOwnerAsync(Institution institution, string userId,
        CancellationToken ct = default);
}