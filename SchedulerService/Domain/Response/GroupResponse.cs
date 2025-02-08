namespace SchedulerService.Domain.Response;

public class GroupResponse
{
    public long Id { get; set; }
    public string Name { get; set; } = default!;
    public string HeadTeacherId { get; set; } = default!;
    public GroupResponse? ParentGroup { get; set; }
    public IEnumerable<UserResponse> Users { get; set; } = default!;
}