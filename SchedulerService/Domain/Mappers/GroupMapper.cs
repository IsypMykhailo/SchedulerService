using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class GroupMapper
{
    public static GroupResponse ToResponse(Group obj)
    {
        return new()
        {
            Id = obj.Id,
            Name = obj.Name,
            HeadTeacherId = obj.HeadTeacherId,
            ParentGroup = obj.ParentGroup is not null ? ToResponse(obj.ParentGroup) : null,
            Users = UserMapper.ToResponses(obj.Students)
        };
    }

    public static IEnumerable<GroupResponse> ToResponses(IEnumerable<Group> objEnumerable)
    {
        var items = objEnumerable as Group[] ?? objEnumerable.ToArray();
        var responses = new GroupResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}