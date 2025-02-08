using System.Linq.Expressions;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using Mapster;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class HomeworksService : IHomeworksService
{
    private readonly ICrudRepository<Homeworks> _homeworksRepository;
    private readonly ICrudRepository<Institution> _institutionRepository;

    public HomeworksService(ICrudRepository<Homeworks> homeworksRepository, ICrudRepository<Institution> institutionRepository)
    {
        _homeworksRepository = homeworksRepository;
        _institutionRepository = institutionRepository;
    }
    
    public async Task<IEnumerable<Homeworks>> GetAllAsync(long groupId, CancellationToken ct = default)
    {
        return await _homeworksRepository.GetAsync(e => e.GroupId == groupId, ct: ct);
    }

    public async Task<Homeworks> CreateAsync(HomeworksDto dto, string teacherId, long groupId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homework = dto.Adapt<HomeworksDto, Homeworks>();
        homework.CreatorId = teacherId;
        homework.GroupId = groupId;
        homework.UploadedDate = utcDateTime;
        homework.DueDate = dto.DueDate.ToUniversalTime();

        await _homeworksRepository.CreateAsync(homework, ct);
        await _homeworksRepository.SaveAsync(ct);
        return homework;
    }
    
    public async Task<Homeworks?> GetByIdAsync(long id, long groupId,
        Expression<Func<Homeworks, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _homeworksRepository.GetByIdAsync(id, 
            e => e.GroupId == groupId,
            includeProperties,
            ct);
    }

    public async Task<bool> DeleteAsync(long id, CancellationToken ct = default)
    {
        var homework = await _homeworksRepository.GetByIdAsync(id, ct: ct);

        if (homework is null)
            return false;
        
        await _homeworksRepository.DeleteAsync(homework, ct);
        await _homeworksRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, HomeworksDto dto, string teacherId, long groupId, CancellationToken ct = default)
    {
        DateTime localDateTime = DateTime.Now;
        DateTime utcDateTime = localDateTime.ToUniversalTime();
        var homework = await _homeworksRepository.GetByIdAsync(id, ct: ct);

        if (homework is null)
            return false;
        
        var updatedHomework = dto.Adapt<HomeworksDto, Homeworks>();
        updatedHomework.CreatorId = teacherId;
        updatedHomework.GroupId = groupId;
        updatedHomework.UploadedDate = utcDateTime;
        updatedHomework.DueDate = dto.DueDate.ToUniversalTime();
        updatedHomework.Id = homework.Id;

        await _homeworksRepository.UpdateAsync(updatedHomework.Id, updatedHomework, ct);
        await _homeworksRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> IsUserTeacherAsync(string userId, CancellationToken ct = default)
    {
        return (await _institutionRepository.GetAsync(
                e => e.Teachers.Any(teacher => teacher.Id == userId), 
                ct: ct))
            .Any();
    }
}