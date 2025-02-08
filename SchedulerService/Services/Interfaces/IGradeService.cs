using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IGradeService
{
    Task<IEnumerable<Grade>> GetAllAsync(GradesJournalColumn column, CancellationToken ct = default);
    Task<IEnumerable<Grade>> GetAllAsync(Institution institution, User student, CancellationToken ct = default);
    Task<bool> MarkStudent(GradeDto grade, GradesJournalColumn column, CancellationToken ct = default);
}