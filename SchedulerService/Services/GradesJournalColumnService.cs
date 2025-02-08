using System.Linq.Expressions;
using Mapster;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class GradesJournalColumnService : IGradesJournalColumnService
{
    private readonly ICrudRepository<GradesJournalColumn> _gradesJournalColumnRepository;

    public GradesJournalColumnService(ICrudRepository<GradesJournalColumn> gradesJournalColumnRepository)
    {
        _gradesJournalColumnRepository = gradesJournalColumnRepository;
    }
    
    public async Task<IEnumerable<GradesJournalColumn>> GetAllAsync(GradesJournal journal, CancellationToken ct = default)
    {
        return await _gradesJournalColumnRepository.GetAsync(
            e => e.Journal.Id == journal.Id,
            new Expression<Func<GradesJournalColumn, object?>>[]
            {
                e => e.Lesson,
                e => e.Lesson!.Subject
            },
            ct: ct);
    }

    public async Task<GradesJournalColumn> CreateAsync(GradesJournalColumnDto dto, GradesJournal journal, 
        LessonsScheduleItem? lesson, Homeworks? homework, CancellationToken ct = default)
    {
        var journalColumn = dto.Adapt<GradesJournalColumnDto, GradesJournalColumn>();
        journalColumn.Journal = journal;
        journalColumn.Lesson = lesson;
        journalColumn.Homework = homework; 
        
        await _gradesJournalColumnRepository.CreateAsync(journalColumn, ct);
        await _gradesJournalColumnRepository.SaveAsync(ct);
        return journalColumn;
    }
    
    public async Task<GradesJournalColumn?> GetByIdAsync(long id, GradesJournal journal,
        Expression<Func<GradesJournalColumn, object?>>[]? includeProperties = null,
        CancellationToken ct = default)
    {
        return await _gradesJournalColumnRepository.GetByIdAsync(id, 
            e => e.Journal.Id == journal.Id,
            includeProperties,
            ct);
    }

    public async Task<bool> DeleteAsync(long id, GradesJournal journal, CancellationToken ct = default)
    {
        var journalColumn = await _gradesJournalColumnRepository.GetByIdAsync(id,
            e => e.Journal.Id == journal.Id, 
            ct: ct);

        if (journalColumn is null)
            return false;
        
        await _gradesJournalColumnRepository.DeleteAsync(journalColumn, ct);
        await _gradesJournalColumnRepository.SaveAsync(ct);
        return true;
    }
    
    public async Task<bool> UpdateAsync(long id, UpdatedGradesJournalColumnDto dto, 
        GradesJournal journal, CancellationToken ct = default)
    {
        var journalColumn = await _gradesJournalColumnRepository.GetByIdAsync(id, 
            e => e.Journal.Id == journal.Id,
            ct: ct);

        if (journalColumn is null)
            return false;
        
        var updatedJournalColumn = dto.Adapt<UpdatedGradesJournalColumnDto, GradesJournalColumn>();
        updatedJournalColumn.Journal = journal;
        updatedJournalColumn.Date = journalColumn.Date;
        updatedJournalColumn.Lesson = journalColumn.Lesson;
        updatedJournalColumn.Homework = journalColumn.Homework;
        updatedJournalColumn.Id = journalColumn.Id;

        await _gradesJournalColumnRepository.UpdateAsync(updatedJournalColumn.Id, updatedJournalColumn, ct);
        await _gradesJournalColumnRepository.SaveAsync(ct);
        return true;
    }
}