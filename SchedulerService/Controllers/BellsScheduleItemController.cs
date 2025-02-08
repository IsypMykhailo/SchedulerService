using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using SchedulerService.Services;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/bells-schedules/{bellsScheduleId:long}/items")]
public class BellsScheduleItemController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IBellsScheduleItemService _bellsScheduleItemService;
    private readonly IBellsScheduleService _bellsScheduleService;
    private readonly IInstitutionService _institutionService;

    public BellsScheduleItemController(IAdministratorService administratorService,
        IBellsScheduleItemService bellsScheduleItemService,
        IInstitutionService institutionService,
        IBellsScheduleService bellsScheduleService)
    {
        _administratorService = administratorService;
        _bellsScheduleItemService = bellsScheduleItemService;
        _institutionService = institutionService;
        _bellsScheduleService = bellsScheduleService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<BellsScheduleItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long bellsScheduleId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var bellsSchedule = await _bellsScheduleService.GetByIdAsync(bellsScheduleId,
            institution,
            new Expression<Func<BellsSchedule, object?>>[]
            {
                e => e.Items
            }, ct: ct);
        
        if (bellsSchedule is null)
            return Response.NotFound($"Bell Schedule by id {bellsScheduleId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        return Response.Ok(BellsScheduleItemMapper.ToResponses(bellsSchedule.Items));
    }

    [HttpGet("{bellsScheduleItemId:long}")]
    [ProducesResponseType(typeof(IEnumerable<BellsScheduleItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetById(long institutionId, long bellsScheduleId, long bellsScheduleItemId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var bellsSchedule = await _bellsScheduleService.GetByIdAsync(bellsScheduleId,
            institution, ct: ct);
        
        if (bellsSchedule is null)
            return Response.NotFound($"Bell Schedule by id {bellsScheduleId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        var bellsScheduleItem =
            await _bellsScheduleItemService.GetByIdAsync(bellsScheduleItemId, bellsSchedule, ct);

        return bellsScheduleItem is null 
            ? Response.NotFound($"Bell Schedule Item by id {bellsScheduleItemId} is not found") 
            : Response.Ok(BellsScheduleItemMapper.ToResponse(bellsScheduleItem));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(BellsScheduleItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, long bellsScheduleId, BellsScheduleItemDto dto,
        [FromQuery] DayOfWeek[] daysToCreate, CancellationToken ct = default)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var bellsSchedule = await _bellsScheduleService.GetByIdAsync(bellsScheduleId, institution, ct: ct);
        
        if (bellsSchedule is null)
            return Response.NotFound($"Bell Schedule by id {bellsScheduleId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        if (!await _bellsScheduleItemService.ValidateIndexesAsync(bellsSchedule, dto, daysToCreate, ct))
        {
            ModelState.AddModelError(nameof(dto.LessonIndex), "Lesson index can not overlap another");
            return Response.ValidationFailed(ModelState);
        }
        
        var bellsScheduleItems = await _bellsScheduleItemService.CreateForDaysAsync(dto, bellsSchedule, daysToCreate, ct);
        return Response.Created(BellsScheduleItemMapper.ToResponses(bellsScheduleItems));
    }

    [HttpPut("update/{bellsScheduleItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long bellsScheduleId,
        long bellsScheduleItemId, BellsScheduleItemDto dto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var bellsSchedule = await _bellsScheduleService.GetByIdAsync(bellsScheduleId, institution, ct: ct);
        
        if (bellsSchedule is null)
            return Response.NotFound($"Bell Schedule by id {bellsScheduleId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var updated = await _bellsScheduleItemService.UpdateAsync(bellsScheduleItemId, dto, bellsSchedule, ct);
        return updated ? Response.Ok() : Response.NotFound($"BellsScheduleItem by id {bellsScheduleItemId} is not found");
    }

    [HttpDelete("delete/{bellsScheduleItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long bellsScheduleId, long bellsScheduleItemId,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var bellsSchedule = await _bellsScheduleService.GetByIdAsync(bellsScheduleId, institution, ct: ct);

        if (bellsSchedule is null)
            return Response.NotFound($"Bell Schedule by id {bellsScheduleId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var deleted = await _bellsScheduleItemService.DeleteAsync(bellsScheduleItemId, bellsSchedule, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Bells Schedule Item by id {bellsScheduleItemId} is not found");
    }
}
