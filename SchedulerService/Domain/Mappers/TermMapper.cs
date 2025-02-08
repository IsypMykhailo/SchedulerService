using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;

namespace SchedulerService.Domain.Mappers;

public static class TermMapper
{
    public static TermResponse ToResponse(Term obj)
    {
        return new TermResponse()
        {
            StartOfTerm = obj.StartOfTerm,
            EndOfTerm = obj.EndOfTerm
        };
    }
}