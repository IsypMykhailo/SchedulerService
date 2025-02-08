using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using Mapster;
using SchedulerService.Domain.Response;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class HomeworkItemsService : IHomeworkItemsService
{
    private readonly ICrudRepository<HomeworkItems> _homeworkItemsRepository;
    private readonly ICrudRepository<Homeworks> _homeworksRepository;
    private readonly ICrudRepository<Institution> _institutionRepository;
    
    public HomeworkItemsService(ICrudRepository<HomeworkItems> homeworkItemsRepository,
        ICrudRepository<Institution> institutionRepository,
        ICrudRepository<Homeworks> homeworksRepository)
    {
        _homeworkItemsRepository = homeworkItemsRepository;
        _institutionRepository = institutionRepository;
        _homeworksRepository = homeworksRepository;
    }

    public async Task<IEnumerable<HomeworkItems>> GetAllAsync(string teacherId, long groupId, CancellationToken ct = default)
    {
        return await _homeworkItemsRepository.GetAsync(e => e.Homework.GroupId == groupId && e.Homework.CreatorId == teacherId, ct: ct);
    }
    
    public async Task<HomeworkItems?> GetByIdAsync(long homeworkId,
        string studentId, long groupId,
        Expression<Func<HomeworkItems, object>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return (await _homeworkItemsRepository.GetAsync(e =>
                e.StudentId == studentId && e.Homework.Id == homeworkId && e.Homework.GroupId == groupId,
            includeProperties: new Expression<Func<HomeworkItems, object?>>[]
            {
                e => e.Homework
            },
            ct)).FirstOrDefault();
    }
    
    public async Task<HomeworkItems> CreateAsync(HomeworkItemsDto dto, Homeworks homework, string studentId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homeworkItem = dto.Adapt<HomeworkItemsDto, HomeworkItems>();
        homeworkItem.Homework = homework;
        homeworkItem.StudentId = studentId;
        homeworkItem.HomeworkUploaded = utcDateTime;
        homeworkItem.HomeworkUpdated = utcDateTime;
        if (homeworkItem.HomeworkUploaded > homeworkItem.Homework.DueDate)
        {
            homeworkItem.BeforeDueDate = false;
        }
        else
        {
            homeworkItem.BeforeDueDate = true;
        }
        await _homeworkItemsRepository.CreateAsync(homeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return homeworkItem;
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var homeworkItem = await _homeworkItemsRepository.GetByIdAsync(id, ct: ct);

        if (homeworkItem is null)
            return false;
        
        await _homeworkItemsRepository.DeleteAsync(homeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> TeacherUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homework, string studentId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homeworkItem = await _homeworkItemsRepository.GetByIdAsync(id, ct: ct);

        if (homeworkItem is null)
            return false;
        
        homeworkItem.HomeworkUpdated = utcDateTime;

        await _homeworkItemsRepository.UpdateAsync(homeworkItem.Id, homeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> StudentUpdateAsync(long id, HomeworkItemsDto dto,
        Homeworks homework, string studentId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homeworkItem = await _homeworkItemsRepository.GetByIdAsync(id, ct: ct);

        if (homeworkItem is null)
            return false;

        var updatedHomeworkItem = dto.Adapt<HomeworkItemsDto, HomeworkItems>();
        updatedHomeworkItem.Id = homeworkItem.Id;
        updatedHomeworkItem.Homework = homework;
        updatedHomeworkItem.HomeworkId = homeworkItem.HomeworkId;
        updatedHomeworkItem.StudentId = studentId;
        updatedHomeworkItem.HomeworkUpdated = utcDateTime;
        updatedHomeworkItem.BeforeDueDate = homeworkItem.BeforeDueDate;

        await _homeworkItemsRepository.UpdateAsync(updatedHomeworkItem.Id, updatedHomeworkItem, ct);
        await _homeworkItemsRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> IsUserTeacherOrStudentAsync(string userId, CancellationToken ct = default)
    {
        return (await _institutionRepository.GetAsync(
                e => e.Groups.Any(group => group.Students.Any(student => student.Id == userId))
                || e.Teachers.Any(teacher => teacher.Id == userId), 
                ct: ct))
            .Any();
    }
    
    public async Task<bool> IsUserStudentAsync(string userId, CancellationToken ct = default)
    {
        return (await _institutionRepository.GetAsync(
                e => e.Groups.Any(group => group.Students.Any(student => student.Id == userId)), 
                ct: ct))
            .Any();
    }

    public async Task<bool> HomeworkItemExists(string teacherId, long homeworkId, string userId, CancellationToken ct = default)
    {
        return (await _homeworkItemsRepository.GetAsync(
                e => e.Homework.Id == homeworkId && e.Homework.CreatorId == teacherId && e.StudentId == userId, 
                ct: ct))
            .Any();
    }

    public async Task<HomeworksCountResponse> GetHomeworksCount(User student, IEnumerable<Group> groups, CancellationToken ct = default)
    {
        // TODO build an expression for this
        List<Homeworks> homeworks = new();
        foreach (var group in groups)
        {
            homeworks.AddRange(await _homeworksRepository.GetAsync(e => e.GroupId == group.Id, ct: ct));
        }

        List<HomeworkItems> items = new(homeworks.Count);
        foreach (var homework in homeworks)
        {
            items.AddRange(await _homeworkItemsRepository.GetAsync(e => e.StudentId == student.Id && e.Homework.Id == homework.Id, ct: ct));
        }

        var countResponse = new HomeworksCountResponse
        {
            Total = homeworks.Count,
            Done = 0,
            Missing = 0,
            Pending = 0,
        };

        foreach (var homework in homeworks)
        {
            var doneHomework = items.Find(e => e.HomeworkId == homework.Id);

            if (doneHomework is not null)
            {
                countResponse.Done++;
            }
            else if (DateTime.UtcNow > homework.DueDate)
            {
                countResponse.Missing++;
            }
            else
            {
                countResponse.Pending++;
            }
        }

        return countResponse;
    }
}