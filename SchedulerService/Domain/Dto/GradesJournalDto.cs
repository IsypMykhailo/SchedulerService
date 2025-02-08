namespace SchedulerService.Domain.Dto;

public class GradesJournalDto
{
    public string? Name { get; set; }
    public long GroupId { get; set; }
    public long SubjectId { get; set; } = default!;
}