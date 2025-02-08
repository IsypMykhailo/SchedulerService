using System.Linq.Expressions;
using SchedulerService.Domain;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IInstitutionService
{
    Task<Institution> CreateAsync(InstitutionDto dto, string userId, CancellationToken ct = default);
    Task<IEnumerable<(Institution, UserRole)>> GetAllAsync(string userId, CancellationToken ct = default);
    Task<Institution?> GetByIdAsync(long id, Expression<Func<Institution, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    Task UpdateAsync(Institution institution, InstitutionDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, string userId, CancellationToken ct = default);
    Task<(Institution, UserRole)?> GetByIdAsync(long id, string userId, CancellationToken ct = default);
}