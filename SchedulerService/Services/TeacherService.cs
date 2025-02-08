using System.Linq.Expressions;
using Microsoft.OpenApi.Any;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class TeacherService : ITeacherService
{
    private readonly ICrudRepository<User> _userRepository;
    private readonly ICrudRepository<Institution> _institutionRepository;

    public TeacherService(ICrudRepository<User> userRepository, ICrudRepository<Institution> institutionRepository)
    {
        _userRepository = userRepository;
        _institutionRepository = institutionRepository;
    }

    public async Task<User?> FindTeacherWithSubjectAsync(Institution institution, Subject subject, string teacherId, CancellationToken ct = default)
    {
        return await _userRepository.GetByIdAsync(teacherId, 
            e => e.TeachingInstitutions.Contains(institution) && e.Subjects.Contains(subject),
            ct: ct);
    }

    /*public async Task<IEnumerable<User>> GetAllTeachersBySubjectAsync(Institution institution, Subject subject, CancellationToken ct = default)
    {

        return await _userRepository.GetAsync(e => e.TeachingInstitutions.Contains(institution)
                                                   && e.Subjects.Contains(subject), ct: ct);
    }*/

    public async Task<User?> FindTeacherAsync(Institution institution, string teacherId, CancellationToken ct = default)
    {
        return await _userRepository.GetByIdAsync(teacherId, 
            e => e.TeachingInstitutions.Contains(institution),
            ct: ct);
    }

    public async Task<bool> IsUserHeadTeacherAsync(string teacherId, CancellationToken ct)
    {
        return (await _institutionRepository.GetAsync(
            e => e.Groups.Any(group => group.HeadTeacherId == teacherId), 
            ct: ct))
            .Any();
    }

    public async Task AddTeacherAsync(Institution institution, string teacherId, CancellationToken ct = default)
    {
        if (IsUserTeacherInternal(institution.Teachers, teacherId))
            return;

        var teacher = await _userRepository.GetByIdAsync(teacherId, ct: ct) ?? new User {Id = teacherId};

        institution.Teachers.Add(teacher);
        await _institutionRepository.SaveAsync(ct);
    }

    public async Task<bool> RemoveTeacherAsync(Institution institution, string teacherId, CancellationToken ct = default)
    {
        if (!IsUserTeacherInternal(institution.Teachers, teacherId))
            return false;

        if (!institution.Teachers.RemoveWhere(e => e.Id == teacherId)) 
            return false;

        await _institutionRepository.SaveAsync(ct);
        return true;
    }

    public async Task<User?> GetByIdAsync( string teacherId,
        Institution institution,
        CancellationToken ct = default)
    {
        if (!IsUserTeacherInternal(institution.Teachers, teacherId))
            return null;
        
        return await _userRepository.GetByIdAsync(teacherId, includeProperties: new Expression<Func<User, object?>>[]
        {
            e => e.Subjects
        }, ct: ct);
    }

    private bool IsUserTeacherInternal(IEnumerable<User> teachers, string userId)
    {
        foreach (var teacher in teachers)
            if (teacher.Id == userId)
                return true;

        return false;
    }
}