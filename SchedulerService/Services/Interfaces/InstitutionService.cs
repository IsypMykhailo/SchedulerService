using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;

namespace SchedulerService.Services.Interfaces;

public class InstitutionService : IInstitutionService
{
    private readonly ICrudRepository<Institution> _institutionRepository;
    private readonly ICrudRepository<User> _userRepository;

    public InstitutionService(ICrudRepository<Institution> institutionRepository, ICrudRepository<User> userRepository)
    {
        _institutionRepository = institutionRepository;
        _userRepository = userRepository;
    }

    public async Task<Institution> CreateAsync(InstitutionDto dto, string userId, CancellationToken ct = default)
    {
        var institution = dto.Adapt<InstitutionDto, Institution>();
        
        var owner = await _userRepository.GetByIdAsync(userId, ct: ct);

        if (owner is null)
        {
            owner = new User { Id = userId };
            await _userRepository.CreateAsync(owner, ct);
        }
        
        institution.Creator = owner;
        institution.Owner = owner;
        
        await _institutionRepository.CreateAsync(institution, ct);
        await _institutionRepository.SaveAsync(ct);
        return institution;
    }

    public async Task<IEnumerable<(Institution, UserRole)>> GetAllAsync(string userId, CancellationToken ct = default)
    {
        var owningInstitutions =
            (await _institutionRepository.GetAsync(
                e => e.OwnerId == userId,
                ct: ct)).Select(e => (e, UserRole.Owner));

        var user = new User { Id = userId };
        
        var administratingInstitutions =
            (await _institutionRepository.GetAsync(
                e => e.Administrators.Contains(user), 
                ct: ct)).Select(e => (e, UserRole.Admin));

        var teachingInstitutions =
            (await _institutionRepository.GetAsync(
                e => e.Teachers.Contains(user),
                ct: ct)).Select(e => (e, UserRole.Teacher));
        
        var institutionsAsStudent =
            (await _institutionRepository.GetAsync(
                i => i.Groups.Any(g => g.Students.Contains(user)),
                ct: ct)).Select(e => (e, UserRole.Student));
        
        return owningInstitutions
            .Concat(administratingInstitutions)
            .Concat(teachingInstitutions)
            .Concat(institutionsAsStudent);
    }

    public async Task<Institution?> GetByIdAsync(long id,
        Expression<Func<Institution, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _institutionRepository.GetByIdAsync(id, null, includeProperties, ct);
    }
    
    public async Task<(Institution, UserRole)?> GetByIdAsync(long id, string userId, CancellationToken ct = default)
    {
        var institution = await _institutionRepository.GetByIdAsync(id, e => e.OwnerId == userId, ct: ct);

        if (institution is not null)
            return (institution, UserRole.Owner);

        var user = new User() { Id = userId };
        institution = await _institutionRepository.GetByIdAsync(id, e => e.Administrators.Contains(user), ct: ct);

        if (institution is not null)
            return (institution, UserRole.Admin);

        institution = await _institutionRepository.GetByIdAsync(id, e => e.Teachers.Contains(user), ct: ct);

        if (institution is not null)
            return (institution, UserRole.Teacher);

        institution = await _institutionRepository.GetByIdAsync(id, e => 
            e.Groups.Any(group => group.Students.Contains(user)), ct: ct);

        if (institution is not null)
            return (institution, UserRole.Student);

        return null;
    }

    public async Task UpdateAsync(Institution institution, InstitutionDto dto, CancellationToken ct = default)
    {
        var updatedInstitution = dto.Adapt<InstitutionDto, Institution>();
        updatedInstitution.Id = institution.Id;
        updatedInstitution.CreatorId = institution.CreatorId;
        updatedInstitution.OwnerId = institution.OwnerId;
        
        await _institutionRepository.UpdateAsync(institution.Id, updatedInstitution, ct);
        await _institutionRepository.SaveAsync(ct);
    }

    public async Task<bool> DeleteAsync(long id, string userId, CancellationToken ct = default)
    {
        var institution = await _institutionRepository.GetByIdAsync(id, ct: ct);

        if (institution is null || institution.OwnerId != userId)
            return false;
        
        await _institutionRepository.DeleteAsync(institution, ct);
        await _institutionRepository.SaveAsync(ct);
        return true;
    }
}