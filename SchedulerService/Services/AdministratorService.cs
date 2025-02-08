using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class AdministratorService : IAdministratorService
{
    private readonly ICrudRepository<Institution> _institutionRepository;
    private readonly ICrudRepository<User> _userRepository;
    private readonly ICrudRepository<Group> _groupRepository;

    public AdministratorService(ICrudRepository<Institution> institutionRepository,
        ICrudRepository<User> userRepository,
        ICrudRepository<Group> groupRepository)
    {
        _institutionRepository = institutionRepository;
        _userRepository = userRepository;
        _groupRepository = groupRepository;
    }
    
    public async Task<bool> IsUserAdminOrOwnerAsync(Institution institution, string userId, CancellationToken ct = default)
    {
        var user = await _userRepository.GetByIdAsync(userId,
            e => e.OwningInstitutions.Contains(institution)
                 || e.AdministratingInstitutions.Contains(institution), ct: ct);

        return user is not null;
    }

    public async Task<bool> IsUserHeadTeacherOrAdminOrOwnerAsync(Institution institution, string userId, Group group, CancellationToken ct = default)
    {
        return userId == group.HeadTeacherId 
               || await IsUserAdminOrOwnerAsync(institution, userId, ct);
    }

    public async Task<bool> IsUserTeacherOrAdminOrOwnerAsync(Institution institution, string userId,
        CancellationToken ct = default)
    {
        return await IsUserTeacherOrAdminOrOwnerInternalAsync(institution, userId, ct);
    }
    
    public async Task<bool> IsUserBelongsToInstitutionAsync(Institution institution, string userId, CancellationToken ct = default)
    {
        if (await IsUserTeacherOrAdminOrOwnerInternalAsync(institution, userId, ct))
            return true;

        var user = new User { Id = userId };
        
        var groups = await _groupRepository.GetAsync(
            e => e.Institution.Id == institution.Id && e.Students.Contains(user),
            ct: ct);

        return groups.Any();
    }

    public async Task AddAdministratorAsync(Institution institution, string adminId, CancellationToken ct = default)
    {
        if (IsUserAdminInternal(institution.Administrators, adminId))
            return;

        var admin = await _userRepository.GetByIdAsync(adminId, ct: ct) ?? new User { Id = adminId };
        institution.Administrators.Add(admin);
        await _institutionRepository.SaveAsync(ct);
    }
    
    public async Task<bool> RemoveAdministratorAsync(Institution institution, string adminId, CancellationToken ct = default)
    {
        if (!IsUserAdminInternal(institution.Administrators, adminId))
            return false;

        if (!institution.Administrators.RemoveWhere(e => e.Id == adminId)) 
            return false;
        
        await _institutionRepository.SaveAsync(ct);
        return true;
    }
    
    private bool IsUserAdminInternal(IEnumerable<User> admins, string userId)
    {
        foreach (var admin in admins)
            if (admin.Id == userId)
                return true;

        return false;
    }

    private async Task<bool> IsUserTeacherOrAdminOrOwnerInternalAsync(Institution institution, string userId, CancellationToken ct)
    {
        var user = await _userRepository.GetByIdAsync(userId,
            e => e.OwningInstitutions.Contains(institution)
                 || e.AdministratingInstitutions.Contains(institution)
                 || e.TeachingInstitutions.Contains(institution), ct: ct);

        return user is not null;
    }
}