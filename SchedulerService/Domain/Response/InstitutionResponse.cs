namespace SchedulerService.Domain.Response;

public class InstitutionResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string? Description { get; set; }
    public string OwnerId { get; set; } = default!;
    public string CreatorId { get; set; } = default!;
    public UserRole Role { get; set; }
}