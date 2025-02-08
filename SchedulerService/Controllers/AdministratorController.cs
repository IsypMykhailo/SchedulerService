using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using SchedulerService.Services;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/admins")]
public class AdministratorController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IInstitutionService _institutionService;
    private readonly IGroupStudentService _groupStudentService;

    public AdministratorController(IAdministratorService administratorService,
        IInstitutionService institutionService,
        IGroupStudentService groupStudentService)
    {
        _administratorService = administratorService;
        _institutionService = institutionService;
        _groupStudentService = groupStudentService;
    }
    
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Administrators
            }, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        return Response.Ok(UserMapper.ToResponses(institution.Administrators));
    }

    [HttpPost("add/{adminId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> AddAdministrator(long institutionId, string adminId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Administrators
            }, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!IsUserOwner(institution, userId))
            return Response.Forbid();

        if (await _administratorService.IsUserBelongsToInstitutionAsync(institution, adminId, ct))
        {
            ModelState.AddModelError("User", "User is a member of this institution");
            return Response.ValidationFailed(ModelState);
        }
        
        await _administratorService.AddAdministratorAsync(institution, adminId, ct);
        return Response.Ok();
    }

    [HttpDelete("remove/{adminId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveAdministrator(long institutionId, string adminId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Administrators
            }, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!IsUserOwner(institution, userId))
            return Response.Forbid();
        
        var removed = await _administratorService.RemoveAdministratorAsync(institution, adminId, ct);
        return removed ? Response.Ok() : Response.NotFound($"Administrator by id {adminId} is not found");
    }

    private bool IsUserOwner(Institution institution, string userId)
    {
        return institution.OwnerId == userId;
    }
}