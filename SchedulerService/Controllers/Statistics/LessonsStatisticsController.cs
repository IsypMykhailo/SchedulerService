using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers.Statistics;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/lessons")]
public class LessonsStatisticsController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly IGroupStudentService _groupStudentService;
    private readonly ITeacherService _teacherService;
    private readonly ILessonsScheduleItemService _lessonsService;

    public LessonsStatisticsController(IInstitutionService institutionService,
        IGroupStudentService groupStudentService, 
        ITeacherService teacherService, 
        ILessonsScheduleItemService lessonsService)
    {
        _institutionService = institutionService;
        _groupStudentService = groupStudentService;
        _teacherService = teacherService;
        _lessonsService = lessonsService;
    }

    [HttpGet("count")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Count(long institutionId, 
        [FromQuery] DateOnly? start, [FromQuery] DateOnly? end, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        IEnumerable<LessonsScheduleItem> lessons;

        if (await _groupStudentService.FindStudentAsync(institution, userId, ct) is not null)
        {
            lessons = await _lessonsService.GetStudentsLessons(institution, userId, start, end, ct);
        }
        else if (await _teacherService.FindTeacherAsync(institution, userId, ct) is not null)
        {
            lessons = await _lessonsService.GetTeachersLessons(institution, userId, start, end, ct);
        }
        else
        {
            return Response.Forbid();
        }
        
        return Response.Ok(new
        {
            Count = lessons.Count()
        });
    }
}