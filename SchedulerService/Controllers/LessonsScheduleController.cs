using System.Net;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Response;
using SchedulerService.Services;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/groups/{groupId:long}/lessons-schedule")]
public class LessonsScheduleController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IInstitutionService _institutionService;
    private readonly IGroupService _groupService;
    private readonly ILessonsScheduleService _lessonsScheduleService;
    private readonly IBellsScheduleService _bellsScheduleService;

    public LessonsScheduleController(IAdministratorService administratorService,
        IInstitutionService institutionService, 
        IGroupService groupService,
        ILessonsScheduleService lessonsScheduleService,
        IBellsScheduleService bellsScheduleService)
    {
        _administratorService = administratorService;
        _institutionService = institutionService;
        _groupService = groupService;
        _lessonsScheduleService = lessonsScheduleService;
        _bellsScheduleService = bellsScheduleService;
    }

    [HttpGet("get")]
    [ProducesResponseType(typeof(LessonsScheduleResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long groupId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        return lessonsSchedule is null 
            ? Response.NotFound("Lesson schedule is not found") 
            : Response.Ok(LessonsScheduleMapper.ToResponse(lessonsSchedule));
    }
    
    [HttpPost("create")]
    [ProducesResponseType(typeof(LessonsScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status409Conflict)]
    public async Task<ApiResponse> Create(long institutionId, long groupId, LessonsScheduleDto lessonsScheduleDto, 
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");

        if (group.LessonsScheduleId is not null)
            return Response.Custom(HttpStatusCode.Conflict, new { Message = "Lesson Schedule already exists" } );
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var bellsSchedule = 
            await _bellsScheduleService.GetByIdAsync(lessonsScheduleDto.BellsScheduleId, institution, ct: ct);

        if (bellsSchedule is null)
        {
            ModelState.AddModelError(nameof(lessonsScheduleDto.BellsScheduleId), "Bell schedule in institution is not found");
            return Response.ValidationFailed(ModelState);
        }
        
        var lessonSchedule = await _lessonsScheduleService.CreateAsync(lessonsScheduleDto, group, bellsSchedule, ct);
        return Response.Created(LessonsScheduleMapper.ToResponse(lessonSchedule));
    }

    [HttpPut("update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long groupId, LessonsScheduleDto lessonsScheduleDto, 
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        if (group.LessonsScheduleId is null)
            return Response.NotFound("Lesson schedule is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var bellsSchedule = 
            await _bellsScheduleService.GetByIdAsync(lessonsScheduleDto.BellsScheduleId, institution, ct: ct);

        if (bellsSchedule is null)
        {
            ModelState.AddModelError(nameof(lessonsScheduleDto.BellsScheduleId), "Bell schedule in institution is not found");
            return Response.ValidationFailed(ModelState);
        }
        
        var updated = await _lessonsScheduleService.UpdateAsync(group, bellsSchedule, lessonsScheduleDto, ct);
        return updated ? Response.Ok() : Response.NotFound("Lesson schedule is not found");
    }
}
