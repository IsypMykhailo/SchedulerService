using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class GradeService : IGradeService
{
    private readonly ICrudRepository<Grade> _gradesRepository;

    public GradeService(ICrudRepository<Grade> gradesRepository)
    {
        _gradesRepository = gradesRepository;
    }
    
    public async Task<IEnumerable<Grade>> GetAllAsync(GradesJournalColumn column, CancellationToken ct = default)
    {
        return await _gradesRepository.GetAsync(
            e => e.JournalColumn.Id == column.Id,
            new Expression<Func<Grade, object?>>[]
            {
                e => e.JournalColumn
            },
            ct: ct);
    }
    
    public async Task<IEnumerable<Grade>> GetAllAsync(Institution institution, User student, CancellationToken ct = default)
    {
        return await _gradesRepository.GetAsync(
            e => e.StudentId == student.Id && e.JournalColumn.Journal.Institution.Id == institution.Id,
            new Expression<Func<Grade, object?>>[]
            {
                e => e.JournalColumn,
                e => e.JournalColumn.Homework,
                e => e.JournalColumn.Lesson,
                e => e.JournalColumn.Journal,
                e => e.JournalColumn.Journal.Subject
            },
            ct: ct);
    }

    public async Task<bool> MarkStudent(GradeDto gradeDto, GradesJournalColumn column, CancellationToken ct = default)
    {
        var grade = (await _gradesRepository.GetAsync(e => e.JournalColumn.Id == column.Id && e.Student.Id == gradeDto.StudentId, ct: ct)).FirstOrDefault();

        if (gradeDto.Points is null)
        {
            if (grade is null)
                return false;
            
            await _gradesRepository.DeleteAsync(grade, ct);
        }
        else if (grade is null)
        {
            grade = gradeDto.Adapt<GradeDto, Grade>();
            grade.JournalColumn = column;

            await _gradesRepository.CreateAsync(grade, ct);
        }
        else
        {
            var id = grade.Id;
            
            grade = gradeDto.Adapt<GradeDto, Grade>();
            grade.Id = id;
            grade.JournalColumn = column;

            await _gradesRepository.UpdateAsync(id, grade, ct);
        }
        
        await _gradesRepository.SaveAsync(ct);
        return true;
    }
}