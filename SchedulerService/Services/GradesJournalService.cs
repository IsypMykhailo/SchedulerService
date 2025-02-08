using System.Linq.Expressions;
using Mapster;
using Microsoft.EntityFrameworkCore.Query;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class GradesJournalService : IGradesJournalService
{
    private readonly ICrudRepository<GradesJournal> _gradesJournalRepository;

    public GradesJournalService(ICrudRepository<GradesJournal> gradesJournalRepository)
    {
        _gradesJournalRepository = gradesJournalRepository;
    }

    public async Task<User?> FindTeacherOfJournalAsync(long gradesJournalId, string userId,
        CancellationToken ct = default)
    {
        return (await _gradesJournalRepository.GetByIdAsync(gradesJournalId,
            e => e.TeacherId == userId,
            new Expression<Func<GradesJournal, object?>>[]
            {
                e => e.Teacher
            },
            ct: ct))?.Teacher;
    }

    public async Task<IEnumerable<GradesJournal>> GetAllAsync(Institution institution, User teacher, Group group, Subject? subject = null, CancellationToken ct = default)
    {
        return await _gradesJournalRepository.GetAsync(e => e.Institution.Id == institution.Id 
                                                            && e.Teacher.Id == teacher.Id 
                                                            && (e.Group.Id == group.Id || e.Group.ParentGroupId == group.Id) 
                                                            && (subject == null || e.Subject.Id == subject.Id),
            new Expression<Func<GradesJournal, object?>>[]
            {
                e => e.Group,
                e => e.Group.ParentGroup,
                e => e.Subject
            },
            ct: ct);
    }

    public async Task<IEnumerable<GradesJournal>> GetAllAsync(Institution institution, CancellationToken ct = default)
    {
        return await _gradesJournalRepository.GetAsync(e => e.Institution.Id == institution.Id,
            new Expression<Func<GradesJournal, object?>>[]
            {
                e => e.Group,
                e => e.Group.ParentGroup,
                e => e.Subject
            },
            ct: ct);
    }

    public async Task<IEnumerable<GradesJournal>> GetAllAsync(Institution institution, User teacher, CancellationToken ct = default)
    {
        return await _gradesJournalRepository.GetAsync(e => e.Institution.Id == institution.Id 
                                                            && (e.TeacherId == teacher.Id || e.Group.HeadTeacher.Id == teacher.Id),
            new Expression<Func<GradesJournal, object?>>[]
            {
                e => e.Group,
                e => e.Group.ParentGroup,
                e => e.Subject
            },
            ct: ct);
    }

    public async Task<GradesJournal> CreateAsync(GradesJournalDto dto, Institution institution, Subject subject,
        User teacher, Group group, CancellationToken ct = default)
    {
        var journal = dto.Adapt<GradesJournalDto, GradesJournal>();
        journal.Institution = institution;
        journal.Group = group;
        journal.Subject = subject;
        journal.Teacher = teacher;
        
        await _gradesJournalRepository.CreateAsync(journal, ct);
        await _gradesJournalRepository.SaveAsync(ct);
        return journal;
    }
    
    public async Task<GradesJournal?> GetByIdAsync(long id, Institution institution,
        Func<IQueryable<GradesJournal>, IIncludableQueryable<GradesJournal, object>>? include = null,
        CancellationToken ct = default)
    {
        return await _gradesJournalRepository.FirstOrDefaultAsync(id, 
            e => e.Institution.Id == institution.Id,
            include,
            ct: ct);
    }
    
    public async Task<GradesJournal?> GetByIdAsync(long id, Institution institution, User teacher, bool hasAccessHeadTeacher = false,
        Func<IQueryable<GradesJournal>, IIncludableQueryable<GradesJournal, object>>? include = null,
        CancellationToken ct = default)
    {
        Expression<Func<GradesJournal, bool>> predicate;
        
        if (hasAccessHeadTeacher)
            predicate = e =>
                e.Institution.Id == institution.Id && (e.Teacher.Id == teacher.Id || e.Group.HeadTeacher.Id == teacher.Id);
        else
            predicate = e =>
                e.Institution.Id == institution.Id && e.Teacher.Id == teacher.Id;

        return await _gradesJournalRepository.FirstOrDefaultAsync(id, predicate, include, ct: ct);
    }
    
    public async Task<GradesJournal?> GetByIdAsync(long id, Institution institution, User teacher, bool hasAccessHeadTeacher = false,
        Expression<Func<GradesJournal, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        Expression<Func<GradesJournal, bool>> predicate;
        
        if (hasAccessHeadTeacher)
            predicate = e =>
                e.Institution.Id == institution.Id && (e.TeacherId == teacher.Id || e.Group.HeadTeacherId == teacher.Id);
        else
            predicate = e =>
                e.Institution.Id == institution.Id && e.TeacherId == teacher.Id;

        return await _gradesJournalRepository.GetByIdAsync(id, predicate, includeProperties, ct);
    }

    public async Task<bool> DeleteAsync(long id, Institution institution, User teacher, CancellationToken ct = default)
    {
        var journal = await _gradesJournalRepository.GetByIdAsync(id,
            e => e.Institution.Id == institution.Id && e.TeacherId == teacher.Id, 
            ct: ct);

        if (journal is null)
            return false;
        
        await _gradesJournalRepository.DeleteAsync(journal, ct);
        await _gradesJournalRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, UpdatedGradesJournalDto dto, Institution institution,
        User newTeacher, string prevTeacherId, CancellationToken ct = default)
    {
        var gradesJournal = await _gradesJournalRepository.GetByIdAsync(id, 
            e => e.Institution.Id == institution.Id && e.TeacherId == prevTeacherId,
            ct: ct);

        if (gradesJournal is null)
            return false;
        
        var updatedJournal = dto.Adapt<UpdatedGradesJournalDto, GradesJournal>();
        updatedJournal.TeacherId = newTeacher.Id;
        updatedJournal.Teacher = newTeacher;
        updatedJournal.Institution = institution;
        updatedJournal.Id = gradesJournal.Id;

        await _gradesJournalRepository.UpdateAsync(updatedJournal.Id, updatedJournal, ct);
        await _gradesJournalRepository.SaveAsync(ct);
        return true;
    }
}