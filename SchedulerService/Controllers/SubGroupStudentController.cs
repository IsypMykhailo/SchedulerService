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
[Route("api/v1/institutions/{institutionId:long}/groups/{groupId:long}/sub-groups/{subGroupId:long}/students")]
public class SubGroupStudentController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IInstitutionService _institutionService;
    private readonly IGroupService _groupService;
    private readonly IGroupStudentService _groupStudentService;
    private readonly ISubGroupService _subGroupService;
    private readonly ISubGroupStudentService _subGroupStudentService;

    public SubGroupStudentController(IAdministratorService administratorService,
        IInstitutionService institutionService, 
        IGroupService groupService, 
        ISubGroupService subGroupService,
        ISubGroupStudentService subGroupStudentService, IGroupStudentService groupStudentService)
    {
        _administratorService = administratorService;
        _institutionService = institutionService;
        _groupService = groupService;
        _subGroupService = subGroupService;
        _subGroupStudentService = subGroupStudentService;
        _groupStudentService = groupStudentService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long groupId, long subGroupId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");

        var subGroup = await _subGroupService.GetByIdAsync(subGroupId,
            group,
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            }, ct);

        if (subGroup is null)
            return Response.NotFound($"Sub group by id {subGroupId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();
        
        return Response.Ok(UserMapper.ToResponses(subGroup.Students));
    }

    [HttpPost("add/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> AddStudent(long institutionId, long groupId, long subGroupId, string studentId,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var subGroup = await _subGroupService.GetByIdAsync(subGroupId, group,
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            }, ct);

        if (subGroup is null)
            return Response.NotFound($"Sub group by id {subGroupId} is not found");
                
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();
        
        var student = await _groupStudentService.FindStudentAsync(group, studentId, ct);

        if (student is null)
            return Response.NotFound($"Student by id {studentId} is not a group student");
        
        await _subGroupStudentService.AddStudent(subGroup, student, ct);
        return Response.Ok();
    }

    [HttpDelete("remove/{studentId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveStudent(long institutionId, long groupId, long subGroupId, string studentId,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);
        
        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var subGroup = await _subGroupService.GetByIdAsync(subGroupId, group,
            new Expression<Func<Group, object?>>[]
            {
                e => e.Students
            }, ct);

        if (subGroup is null)
            return Response.NotFound($"Sub group by id {subGroupId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();
        
        var removed = await _subGroupStudentService.RemoveStudent(subGroup, studentId, ct);
        return removed ? Response.Ok() : Response.NotFound($"Student by id {studentId} is not found");
    }
}