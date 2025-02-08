namespace SchedulerService.Domain.Response;

public class GradesJournalResponse
{
    public long Id { get; set; }
    public string? Name { get; set; }
    public string TeacherId { get; set; } = default!;
    public GroupResponse? Group { get; set; }
    public SubjectResponse Subject { get; set; } = default!;
    public IEnumerable<GradesJournalColumnResponse> Columns { get; set; } = default!;
    public bool HasAccessToEdit { get; set; }
}