using System.Linq.Expressions;
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
[Route("api/v1/institutions/{institutionId:long}/groups/{groupId:long}/students")]
public class GroupStudentController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IGroupService _groupService;
    private readonly IGroupStudentService _groupStudentService;
    private readonly IInstitutionService _institutionService;

    public GroupStudentController(IAdministratorService administratorService,
        IGroupStudentService groupStudentService,
        IInstitutionService institutionService, IGroupService groupService)
    {
        _administratorService = administratorService;
        _groupStudentService = groupStudentService;
        _institutionService = institutionService;
        _groupService = groupService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long groupId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, 
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            }, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();
        
        return Response.Ok(UserMapper.ToResponses(group.Students));
    }

    [HttpPost("add/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> AddStudent(long institutionId, long groupId, string studentId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, 
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            }, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();

        if (await _administratorService.IsUserBelongsToInstitutionAsync(institution, studentId, ct))
        {
            ModelState.AddModelError(nameof(studentId), "Can not add the student because he already belongs to this institution");
            return Response.ValidationFailed(ModelState);
        }
        
        await _groupStudentService.AddStudentAsync(group, studentId, ct);
        return Response.Ok();
    }

    [HttpDelete("remove/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveStudent(long institutionId, long groupId, string studentId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, 
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            }, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();
        
        var removed = await _groupStudentService.RemoveStudentAsync(group, studentId, ct);
        return removed ? Response.Ok() : Response.NotFound($"Student by id {studentId} is not found");
    }
}