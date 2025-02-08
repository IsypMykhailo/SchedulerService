using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IGradesJournalService
{
    Task<IEnumerable<GradesJournal>> GetAllAsync(Institution institution, CancellationToken ct = default);
    Task<IEnumerable<GradesJournal>> GetAllAsync(Institution institution, User teacher, CancellationToken ct = default);
    Task<IEnumerable<GradesJournal>> GetAllAsync(Institution institution, User teacher, Group group, Subject? subject = null, CancellationToken ct = default);

    Task<GradesJournal> CreateAsync(GradesJournalDto dto, Institution institution, Subject subject,
        User teacher, Group group, CancellationToken ct = default);

    Task<GradesJournal?> GetByIdAsync(long id, Institution institution, User teacher, bool hasAccessHeadTeacher = false,
        Expression<Func<GradesJournal, object?>>[]? includeProperties = null,
        CancellationToken ct = default);

    Task<GradesJournal?> GetByIdAsync(long id, Institution institution, User teacher, bool hasAccessHeadTeacher = false,
        Func<IQueryable<GradesJournal>, IIncludableQueryable<GradesJournal, object>>? include = null,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(long id, Institution institution, User teacher, CancellationToken ct = default);

    Task<bool> UpdateAsync(long id, UpdatedGradesJournalDto dto, Institution institution, 
        User newTeacher, string prevTeacherId, CancellationToken ct = default);

    Task<User?> FindTeacherOfJournalAsync(long gradesJournalId, string userId,
        CancellationToken ct = default);

    Task<GradesJournal?> GetByIdAsync(long id, Institution institution,
        Func<IQueryable<GradesJournal>, IIncludableQueryable<GradesJournal, object>>? include = null,
        CancellationToken ct = default);
}