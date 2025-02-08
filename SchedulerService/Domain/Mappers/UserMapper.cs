using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class UserMapper
{
    public static UserResponse ToResponse(User obj)
    {
        return new()
        {
            Id = obj.Id,
        };
    }

    public static IEnumerable<UserResponse> ToResponses(IEnumerable<User> objEnumerable)
    {
        var items = objEnumerable as User[] ?? objEnumerable.ToArray();
        var responses = new UserResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}