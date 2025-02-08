using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class SubjectService : ISubjectService
{
    private readonly ICrudRepository<Subject> _subjectRepository;

    public SubjectService(ICrudRepository<Subject> subjectRepository)
    {
        _subjectRepository = subjectRepository;
    }

    public async Task<Subject> CreateAsync(SubjectDto dto, Institution institution, CancellationToken ct = default)
    {
        var subject = dto.Adapt<SubjectDto, Subject>();
        subject.Institution = institution;

        await _subjectRepository.CreateAsync(subject, ct);
        await _subjectRepository.SaveAsync(ct);
        return subject;
    }

    public async Task<Subject?> GetByIdAsync(long id,
        Institution institution,
        Expression<Func<Subject, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _subjectRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id, includeProperties, 
            ct: ct);
    }

    public async Task<bool> DeleteAsync(long id, Institution institution, CancellationToken ct = default)
    {
        var subject = await _subjectRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id,
            ct: ct);

        if (subject is null)
            return false;
        
        await _subjectRepository.DeleteAsync(subject, ct);
        await _subjectRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, SubjectDto dto, Institution institution, CancellationToken ct = default)
    {
        var subject = await _subjectRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id,
            ct: ct);

        if (subject is null)
            return false;
        
        var updatedSubject = dto.Adapt<SubjectDto, Subject>();
        updatedSubject.Institution = institution;
        updatedSubject.Id = subject.Id;

        await _subjectRepository.UpdateAsync(updatedSubject.Id, updatedSubject, ct);
        await _subjectRepository.SaveAsync(ct);
        return true;
    }

    public async Task<bool> IsSubjectNameValidAsync(Institution institution, string subjectName, CancellationToken ct = default)
    {
        var groups = await _subjectRepository.GetAsync(
            e => e.Name == subjectName && e.Institution.Id == institution.Id,
            new Expression<Func<Subject, object?>>[]
            {
                e => e.Institution
            }, ct);

        return !groups.Any();
    }

    public async Task<bool> IsSubjectNameValidAsync(Institution institution, long exceptSubjectId, string subjectName, CancellationToken ct = default)
    {
        var groups = await _subjectRepository.GetAsync(
            e => e.Name == subjectName && e.Institution.Id == institution.Id && e.Id != exceptSubjectId,
            new Expression<Func<Subject, object?>>[]
            {
                e => e.Institution
            }, ct);

        return !groups.Any();
    }
}