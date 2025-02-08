using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface ISubjectService
{
    Task<Subject> CreateAsync(SubjectDto dto, Institution institution, CancellationToken ct = default);
    Task<Subject?> GetByIdAsync(long id,
        Institution institution,
        Expression<Func<Subject, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, Institution institution, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, SubjectDto dto, Institution institution, CancellationToken ct = default);
    Task<bool> IsSubjectNameValidAsync(Institution institution, string subjectName, CancellationToken ct = default);
    Task<bool> IsSubjectNameValidAsync(Institution institution, long exceptSubjectId, string subjectName, CancellationToken ct = default);
}