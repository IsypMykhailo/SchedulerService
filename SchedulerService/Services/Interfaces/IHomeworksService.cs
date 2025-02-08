using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IHomeworksService
{
    Task<Homeworks> CreateAsync(HomeworksDto dto, string teacherId, long groupId, CancellationToken ct = default);
    Task<Homeworks?> GetByIdAsync(long id, long groupId,
        Expression<Func<Homeworks, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> UpdateAsync(long id, HomeworksDto dto, string teacherId, long groupId, CancellationToken ct = default);
    Task<IEnumerable<Homeworks>> GetAllAsync(long groupId, CancellationToken ct = default);

    Task<bool> IsUserTeacherAsync(string userId, CancellationToken ct = default);
}