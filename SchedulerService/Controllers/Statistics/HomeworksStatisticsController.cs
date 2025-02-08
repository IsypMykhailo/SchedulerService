using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Response;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers.Statistics;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/homeworks")]
public class HomeworksStatisticsController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly IGroupService _groupService;
    private readonly IHomeworkItemsService _homeworkItemsService;
    private readonly IGroupStudentService _groupStudentService;

    public HomeworksStatisticsController(IInstitutionService institutionService,
        IGroupService groupService,
        IHomeworkItemsService homeworkItemsService, 
        IGroupStudentService groupStudentService)
    {
        _institutionService = institutionService;
        _groupService = groupService;
        _homeworkItemsService = homeworkItemsService;
        _groupStudentService = groupStudentService;
    }

    [HttpGet("count")]
    [ProducesResponseType(typeof(HomeworksCountResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Count(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        var student = await _groupStudentService.FindStudentAsync(institution, userId, ct);
        
        if (student is null)
            return Response.Forbid();
        
        var groups = await _groupService.GetAllAsStudentAsync(institution, student.Id, ct);
        return Response.Ok(await _homeworkItemsService.GetHomeworksCount(student, groups, ct));
    }
}