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
[Route("api/v1/institutions/{institutionId:long}/bells-schedules")]
public class BellsScheduleController : ControllerBase
{
    private readonly IBellsScheduleService _bellsScheduleService;
    private readonly IAdministratorService _administratorService;
    private readonly IInstitutionService _institutionService;

    public BellsScheduleController(IBellsScheduleService bellsScheduleService,
        IAdministratorService administratorService,
        IInstitutionService institutionService)
    {
        _bellsScheduleService = bellsScheduleService;
        _administratorService = administratorService;
        _institutionService = institutionService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<BellsScheduleResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        var bellsSchedules = await _bellsScheduleService.GetAllAsync(institution, ct);
        return Response.Ok(BellsScheduleMapper.ToResponses(bellsSchedules));
    }

    [HttpGet("{bellsScheduleId:long}")]
    [ProducesResponseType(typeof(BellsScheduleItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetById(long institutionId, long bellsScheduleId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        var bellsSchedule = await _bellsScheduleService.GetByIdAsync(bellsScheduleId,
            institution, ct: ct);
        
        return bellsSchedule is null 
            ? Response.NotFound($"Bell Schedule by id {bellsScheduleId} is not found") 
            : Response.Ok(BellsScheduleMapper.ToResponse(bellsSchedule));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(BellsScheduleResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, BellsScheduleDto bellsScheduleDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        var bellsSchedule = await _bellsScheduleService.CreateAsync(bellsScheduleDto, institution, ct);
        return Response.Created(BellsScheduleMapper.ToResponse(bellsSchedule));
    }

    [HttpPut("update/{bellsScheduleId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long bellsScheduleId, BellsScheduleDto bellsScheduleDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        var updated = await _bellsScheduleService.UpdateAsync(bellsScheduleId, bellsScheduleDto, institution, ct);
        return updated ? Response.Ok() : Response.NotFound($"Bell schedule by id {bellsScheduleId} is not found");
    }
    
    [HttpDelete("delete/{bellsScheduleId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long bellsScheduleId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        var deleted = await _bellsScheduleService.DeleteAsync(bellsScheduleId, institution, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Bell schedule by id {bellsScheduleId} is not found");
    }
}
