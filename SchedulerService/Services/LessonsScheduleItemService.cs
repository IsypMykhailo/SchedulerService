using System.Collections.Concurrent;
using System.Globalization;
using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class LessonsScheduleItemService : ILessonsScheduleItemService
{
    private static readonly ConcurrentDictionary<DateOnly, int> CachedWeeksOfYear = new();
    private readonly ICrudRepository<LessonsScheduleItem> _lessonsScheduleItemRepository;

    public LessonsScheduleItemService(ICrudRepository<LessonsScheduleItem> lessonsScheduleItemRepository)
    {
        _lessonsScheduleItemRepository = lessonsScheduleItemRepository;
    }

    public async Task<bool> UpdateInfoAsync(long id, UpdatedLessonInfoDto dto, LessonsSchedule lessonsSchedule, User teacher,
        CancellationToken ct = default)
    {
        var lessonsScheduleItem = await _lessonsScheduleItemRepository.GetByIdAsync(id,
            e => e.LessonsSchedule.Id == lessonsSchedule.Id && e.TeacherId == teacher.Id,
            ct: ct);

        if (lessonsScheduleItem is null)
            return false;

        lessonsScheduleItem.Theme = dto.Theme;
        lessonsScheduleItem.HomeworkDescription = dto.HomeworkDescription;

        await _lessonsScheduleItemRepository.SaveAsync(ct);
        return true;
    }

    public async Task<IEnumerable<Group>> FindTeachingGroupsAsync(User teacher, CancellationToken ct = default)
    {
        var teachersLessons =
            await _lessonsScheduleItemRepository.GetAsync(
                e => e.Teacher != null && e.Teacher.Id == teacher.Id,
                new Expression<Func<LessonsScheduleItem, object?>>[]
                {
                    e => e.SubGroup,
                    e => e.SubGroup!.ParentGroup,
                    e => e.LessonsSchedule.Group
                },
                ct: ct);

        var groups = new Dictionary<long, Group>();

        foreach (var lesson in teachersLessons)
        {
            if (lesson.SubGroup is not null && lesson.SubGroupId is not null)
                groups.TryAdd((long)lesson.SubGroupId, lesson.SubGroup);
            else
                groups.TryAdd(lesson.LessonsSchedule.Group.Id, lesson.LessonsSchedule.Group);
        }

        return groups.Values;
    }

    public async Task<IEnumerable<LessonsScheduleItem>> GetAsync(LessonsSchedule lessonsSchedule,
        DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default)
    {
        start ??= DateOnly.MinValue;
        end ??= DateOnly.MaxValue;

        return await _lessonsScheduleItemRepository
            .GetAsync(e => e.LessonsSchedule.Id == lessonsSchedule.Id 
                           && start <= e.Date && e.Date <= end,
                new Expression<Func<LessonsScheduleItem, object?>>[]
                {
                    e => e.Subject,
                    e => e.SubGroup
                }, ct: ct);
    }
    
    public async Task<IEnumerable<LessonsScheduleItem>> GetTeachersLessonsInGroupAsync(LessonsSchedule lessonsSchedule,
        string teacherId, DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default)
    {
        start ??= DateOnly.MinValue;
        end ??= DateOnly.MaxValue;

        return await _lessonsScheduleItemRepository
            .GetAsync(e => e.LessonsSchedule.Id == lessonsSchedule.Id && e.TeacherId == teacherId 
                           && start <= e.Date && e.Date <= end,
                new Expression<Func<LessonsScheduleItem, object?>>[]
                {
                    e => e.Subject,
                    e => e.SubGroup
                }, ct: ct);
    }
    
    public async Task<IEnumerable<LessonsScheduleItem>> GetAllTeachersLessonsAsync(User teacher, 
        DateOnly? start = null, DateOnly? end = null, CancellationToken ct = default)
    {
        start ??= DateOnly.MinValue;
        end ??= DateOnly.MaxValue;

        return await _lessonsScheduleItemRepository
            .GetAsync(e => e.TeacherId == teacher.Id && start <= e.Date && e.Date <= end,
                new Expression<Func<LessonsScheduleItem, object?>>[]
                {
                    e => e.Subject,
                    e => e.LessonsSchedule,
                    e => e.LessonsSchedule.Group,
                    e => e.LessonsSchedule.Group.HeadTeacher,
                    e => e.LessonsSchedule.Group.ParentGroup,
                    e => e.SubGroup,
                    e => e.SubGroup!.ParentGroup,
                }, ct: ct);
    }

    public async Task<IEnumerable<LessonsScheduleItem>> GetAsync(LessonsSchedule lessonsSchedule, User student, DateOnly? start, DateOnly? end, CancellationToken ct)
    {
        start ??= DateOnly.MinValue;
        end ??= DateOnly.MaxValue;

        return await _lessonsScheduleItemRepository
            .GetAsync(e => e.LessonsSchedule.Id == lessonsSchedule.Id 
                           && start <= e.Date && e.Date <= end 
                           && (e.SubGroup == null || e.SubGroup.Students.Any(user => user.Id == student.Id)),
                new Expression<Func<LessonsScheduleItem, object?>>[]
                {
                    e => e.Subject,
                    e => e.SubGroup
                }, ct: ct);
    }

    public async Task<IEnumerable<LessonsScheduleItem>> GetStudentsLessons(Institution institution, string userId, DateOnly? start, DateOnly? end,
        CancellationToken ct = default)
    {
        start ??= DateOnly.MinValue;
        end ??= DateOnly.MaxValue;
        
        return await _lessonsScheduleItemRepository
            .GetAsync(e => e.LessonsSchedule.Group.Students.Any(s => s.Id == userId) 
                           && start <= e.Date && e.Date <= end
                           && (e.SubGroup == null || e.SubGroup.Students.Any(user => user.Id == userId)), ct: ct);
    }

    public async Task<IEnumerable<LessonsScheduleItem>> GetTeachersLessons(Institution institution, string userId, DateOnly? start, DateOnly? end,
        CancellationToken ct = default)
    {
        start ??= DateOnly.MinValue;
        end ??= DateOnly.MaxValue;
        
        return await _lessonsScheduleItemRepository
            .GetAsync(e => e.TeacherId == userId
                           && start <= e.Date && e.Date <= end, ct: ct);
    }

    public async Task<LessonsScheduleItem?> GetByIdAsync(long id, LessonsSchedule lessonsSchedule, User teacher,
        Expression<Func<LessonsScheduleItem, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _lessonsScheduleItemRepository.GetByIdAsync(id,
            e => e.LessonsSchedule.Id == lessonsSchedule.Id && e.TeacherId == teacher.Id,
            includeProperties,
            ct);
    }

    public async Task<LessonsScheduleItem?> GetByIdAsync(long id, LessonsSchedule lessonsSchedule,
        Expression<Func<LessonsScheduleItem, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _lessonsScheduleItemRepository.GetByIdAsync(id,
            e => e.LessonsSchedule.Id == lessonsSchedule.Id,
            includeProperties,
            ct);
    }

    public async Task<LessonsScheduleItem?> GetByIdAsync(long id,
        Expression<Func<LessonsScheduleItem, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _lessonsScheduleItemRepository.GetByIdAsync(id, includeProperties: includeProperties, ct: ct);
    }
    
    public async Task<LessonsScheduleItem> CreateAsync(SingleLessonsScheduleItemDto dto,
        LessonsSchedule lessonsSchedule,
        Subject subject,
        Group? subGroup = null,
        User? teacher = null,
        CancellationToken ct = default)
    {
        var lessonsScheduleItem = dto.Adapt<SingleLessonsScheduleItemDto, LessonsScheduleItem>();
        lessonsScheduleItem.LessonsSchedule = lessonsSchedule;
        lessonsScheduleItem.Subject = subject;
        lessonsScheduleItem.SubGroup = subGroup;
        lessonsScheduleItem.Teacher = teacher;

        await _lessonsScheduleItemRepository.CreateAsync(lessonsScheduleItem, ct);
        await _lessonsScheduleItemRepository.SaveAsync(ct);
        return lessonsScheduleItem;
    }

    public async Task<IEnumerable<LessonsScheduleItem>> CreateRangeAsync(RangeLessonsScheduleItemDto dto, LessonFrequency lessonFrequency, 
        LessonsSchedule lessonsSchedule, Subject subject, Group? subGroup = null, User? teacher = null, 
        CancellationToken ct = default)
    {
        var firstDayOfLesson = 0;
        var fromDayOfWeek = dto.From.DayOfWeek;
        while (fromDayOfWeek != dto.DayOfWeek)
        {
            firstDayOfLesson++;

            if (fromDayOfWeek == DayOfWeek.Saturday)
                fromDayOfWeek = 0;
            else
                fromDayOfWeek++;
        }

        var daysOffset = dto.To.DayNumber - dto.From.DayNumber;
        var prevDay = firstDayOfLesson;
        var firstTimeIterated = false;
        
        List<LessonsScheduleItem> lessons = new(daysOffset / 7);
        
        for (var i = firstDayOfLesson; i < daysOffset; i++)
        {
            if (prevDay + 7 != i && firstTimeIterated) 
                continue;
            
            firstTimeIterated = true;
            prevDay = i;

            var lessonDate = dto.From.AddDays(i);
            
            switch (lessonFrequency)
            {
                case LessonFrequency.EveryEvenWeek when 
                    GetWeekOfYear(lessonDate) % 2 != 0:
                case LessonFrequency.EveryOddWeek when 
                    GetWeekOfYear(lessonDate) + 1 % 2 == 0:
                    continue;
            }

            lessons.Add(new LessonsScheduleItem()
            {
                LessonsSchedule = lessonsSchedule,
                Subject = subject,
                SubGroup = subGroup,
                Teacher = teacher,
                Date = lessonDate,
                LessonIndex = dto.LessonIndex
            });
        }
        
        await _lessonsScheduleItemRepository.CreateRangeAsync(lessons, ct);
        await _lessonsScheduleItemRepository.SaveAsync(ct);
        return lessons;
    }

    public async Task<bool> DeleteAsync(long id, LessonsSchedule lessonsSchedule, CancellationToken ct = default)
    {
        var lessonsScheduleItem = await _lessonsScheduleItemRepository.GetByIdAsync(id,
            e => e.LessonsSchedule.Id == lessonsSchedule.Id,
            ct: ct);

        if (lessonsScheduleItem is null)
            return false;
        
        await _lessonsScheduleItemRepository.DeleteAsync(lessonsScheduleItem, ct);
        await _lessonsScheduleItemRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> UpdateAsync(long id,
        SingleLessonsScheduleItemDto dto,
        LessonsSchedule lessonsSchedule,
        Subject subject,
        Group? subGroup = null,
        User? teacher = null,
        CancellationToken ct = default)
    {
        var lessonsScheduleItem = await _lessonsScheduleItemRepository.GetByIdAsync(id,
            e => e.LessonsSchedule.Id == lessonsSchedule.Id,
            ct: ct);

        if (lessonsScheduleItem is null)
            return false;
        
        var updatedLessonsScheduleItem = dto.Adapt<SingleLessonsScheduleItemDto, LessonsScheduleItem>();
        updatedLessonsScheduleItem.LessonsSchedule = lessonsSchedule;
        updatedLessonsScheduleItem.SubjectId = subject.Id;
        updatedLessonsScheduleItem.SubGroup = subGroup;
        updatedLessonsScheduleItem.Teacher = teacher;
        updatedLessonsScheduleItem.Id = lessonsScheduleItem.Id;

        await _lessonsScheduleItemRepository.UpdateAsync(updatedLessonsScheduleItem.Id, updatedLessonsScheduleItem, ct);
        await _lessonsScheduleItemRepository.SaveAsync(ct);
        return true;
    }

    private int GetWeekOfYear(DateOnly date)
    {
        var weekOfYear = CachedWeeksOfYear.GetOrAdd(date, value => ISOWeek.GetWeekOfYear(value.ToDateTime(TimeOnly.MinValue)));
        return weekOfYear;
    }
}