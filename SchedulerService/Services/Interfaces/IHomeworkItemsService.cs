using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Services.Interfaces;

public interface IHomeworkItemsService
{
    Task<HomeworkItems> CreateAsync(HomeworkItemsDto dto, Homeworks homeworks, string studentId, CancellationToken ct = default);
    Task<bool> DeleteAsync(long id, CancellationToken ct = default);
    Task<bool> TeacherUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homeworks, string studentId, CancellationToken ct = default);

    Task<bool> StudentUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homeworks, string studentId, CancellationToken ct = default);

    Task<IEnumerable<HomeworkItems>> GetAllAsync(string teacherId, long groupId,
        CancellationToken ct = default);

    Task<HomeworkItems?> GetByIdAsync(long homeworkId,
        string studentId, long groupId,
        Expression<Func<HomeworkItems, object>>[]? includeProperties = null,
        CancellationToken ct = default);

    Task<bool> IsUserTeacherOrStudentAsync(string userId, CancellationToken ct = default);
    Task<bool> IsUserStudentAsync(string userId, CancellationToken ct = default);

    Task<bool> HomeworkItemExists(string teacherId, long homeworkId, string userId,
        CancellationToken ct = default);

    Task<HomeworksCountResponse> GetHomeworksCount(User student, IEnumerable<Group> groups,
        CancellationToken ct = default);
}