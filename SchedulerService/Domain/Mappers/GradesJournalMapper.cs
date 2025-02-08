using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class GradesJournalMapper
{
    public static GradesJournalResponse ToResponse((GradesJournal journal, bool hasAccessToEdit) obj)
    {
        return new()
        {
            Id = obj.journal.Id,
            Name = obj.journal.Name,
            TeacherId = obj.journal.TeacherId,
            Group = GroupMapper.ToResponse(obj.journal.Group),
            Subject = SubjectMapper.ToResponse(obj.journal.Subject),
            Columns = GradesJournalColumnMapper.ToResponses(obj.journal.Columns),
            HasAccessToEdit = obj.hasAccessToEdit
        };
    }
    
    public static GradesJournalResponse ToResponse(GradesJournal obj)
    {
        return new()
        {
            Id = obj.Id,
            Name = obj.Name,
            TeacherId = obj.TeacherId,
            Group = GroupMapper.ToResponse(obj.Group),
            Subject = SubjectMapper.ToResponse(obj.Subject),
            Columns = GradesJournalColumnMapper.ToResponses(obj.Columns)
        };
    }

    public static IEnumerable<GradesJournalResponse> ToResponses(IEnumerable<GradesJournal> objEnumerable)
    {
        var items = objEnumerable as GradesJournal[] ?? objEnumerable.ToArray();
        var responses = new GradesJournalResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}