using System.Linq.Expressions;
using SchedulerService.Domain;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface ILessonsScheduleItemService
{
    Task<IEnumerable<LessonsScheduleItem>> GetAsync(LessonsSchedule lessonsSchedule, 
        DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default);
    
    Task<LessonsScheduleItem?> GetByIdAsync(long id, LessonsSchedule lessonsSchedule, User teacher,
        Expression<Func<LessonsScheduleItem, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    
    Task<LessonsScheduleItem?> GetByIdAsync(long id, LessonsSchedule lessonsSchedule,
        Expression<Func<LessonsScheduleItem, object?>>[]? includeProperties = null,
        CancellationToken ct = default);
    
    Task<LessonsScheduleItem?> GetByIdAsync(long id,
        Expression<Func<LessonsScheduleItem, object?>>[]? includeProperties = null,
        CancellationToken ct = default);

    Task<LessonsScheduleItem> CreateAsync(SingleLessonsScheduleItemDto dto,
        LessonsSchedule lessonsSchedule,
        Subject subject,
        Group? subGroup = null,
        User? teacher = null,
        CancellationToken ct = default);

    Task<IEnumerable<LessonsScheduleItem>> CreateRangeAsync(RangeLessonsScheduleItemDto dto,
        LessonFrequency lessonFrequency,
        LessonsSchedule lessonsSchedule,
        Subject subject,
        Group? subGroup = null,
        User? teacher = null,
        CancellationToken ct = default);

    Task<bool> DeleteAsync(long id, LessonsSchedule lessonsSchedule, CancellationToken ct = default);

    Task<bool> UpdateAsync(long id,
        SingleLessonsScheduleItemDto dto,
        LessonsSchedule lessonsSchedule,
        Subject subject,
        Group? subGroup = null,
        User? teacher = null,
        CancellationToken ct = default);
    
    Task<bool> UpdateInfoAsync(long id,
        UpdatedLessonInfoDto dto,
        LessonsSchedule lessonsSchedule,
        User teacher,
        CancellationToken ct = default);

    Task<IEnumerable<Group>> FindTeachingGroupsAsync(User teacher, CancellationToken ct = default);

    Task<IEnumerable<LessonsScheduleItem>> GetTeachersLessonsInGroupAsync(LessonsSchedule lessonsSchedule,
        string teacherId, DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default);

    Task<IEnumerable<LessonsScheduleItem>> GetAllTeachersLessonsAsync(User teacher, 
        DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default);

    Task<IEnumerable<LessonsScheduleItem>> GetAsync(LessonsSchedule lessonsSchedule, User student, DateOnly? start, DateOnly? end, CancellationToken ct);
    Task<IEnumerable<LessonsScheduleItem>> GetStudentsLessons(Institution institution, string userId, DateOnly? start, DateOnly? end, CancellationToken ct = default);
    Task<IEnumerable<LessonsScheduleItem>> GetTeachersLessons(Institution institution, string userId, DateOnly? start, DateOnly? end, CancellationToken ct = default);
}