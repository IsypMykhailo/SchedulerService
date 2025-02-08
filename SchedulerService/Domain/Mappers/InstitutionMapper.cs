using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class InstitutionMapper
{
    public static InstitutionResponse ToResponse((Institution Institution, UserRole Role) obj)
    {
        return new()
        {
            Id = obj.Institution.Id,
            Description = obj.Institution.Description,
            Name = obj.Institution.Name,
            CreatorId = obj.Institution.CreatorId,
            OwnerId = obj.Institution.OwnerId,
            Role = obj.Role
        };
    }

    public static IEnumerable<InstitutionResponse> ToResponses(IEnumerable<(Institution, UserRole)> objEnumerable)
    {
        var items = objEnumerable.ToArray();
        var responses = new InstitutionResponse[items.Length];

        for (var i = 0; i < responses.Length; i++)
        {
            responses[i] = ToResponse(items[i]);
        }

        return responses;
    }
}