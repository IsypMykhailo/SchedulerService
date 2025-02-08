using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IGradesJournalColumnService
{
    Task<IEnumerable<GradesJournalColumn>> GetAllAsync(GradesJournal journal, CancellationToken ct = default);

    Task<GradesJournalColumn> CreateAsync(GradesJournalColumnDto dto, GradesJournal journal, 
        LessonsScheduleItem? lesson, Homeworks? homework, CancellationToken ct = default);

    Task<GradesJournalColumn?> GetByIdAsync(long id, GradesJournal journal,
        Expression<Func<GradesJournalColumn, object?>>[]? includeProperties = null,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(long id, GradesJournal journal, CancellationToken ct = default);

    Task<bool> UpdateAsync(long id, UpdatedGradesJournalColumnDto dto, 
        GradesJournal journal, CancellationToken ct = default);
}