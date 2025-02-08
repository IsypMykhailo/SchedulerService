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
[Route("api/v1/institutions/{institutionId:long}/groups/{groupId:long}/sub-groups")]
public class SubGroupController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly ISubGroupService _subGroupService;
    private readonly IGroupService _groupService;
    private readonly IInstitutionService _institutionService;

    public SubGroupController(IAdministratorService administratorService,
        ISubGroupService subGroupService,
        IGroupService groupService, IInstitutionService institutionService)
    {
        _administratorService = administratorService;
        _subGroupService = subGroupService;
        _groupService = groupService;
        _institutionService = institutionService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<GroupResponse>), StatusCodes.Status200OK)]
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

        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();

        var subGroups = await _subGroupService.GetAllAsync(group, ct);
        return Response.Ok(GroupMapper.ToResponses(subGroups));
    }
    
    [HttpGet("{subGroupId:long}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetById(long institutionId, long groupId, long subGroupId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);
        
        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();

        var subGroup = await _subGroupService.GetByIdAsync(subGroupId, group, ct: ct);
        return subGroup is null
            ? Response.NotFound($"Sub Group by id {subGroupId} is not found")
            : Response.Ok(GroupMapper.ToResponse(subGroup));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, long groupId, SubGroupDto subGroupDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();

        if (!await _subGroupService.IsSubGroupNameValidAsync(group, subGroupDto.Name, ct))
        {
            ModelState.AddModelError(nameof(subGroupDto.Name), "Subgroup name already exists in the institution");
            return Response.ValidationFailed(ModelState);
        }
        
        var subGroup = await _subGroupService.CreateAsync(subGroupDto, group, institution, ct);
        return Response.Created(GroupMapper.ToResponse(subGroup));
    }

    [HttpPut("update/{subGroupId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long groupId, long subGroupId, SubGroupDto subGroupDto,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();

        if (!await _subGroupService.IsSubGroupNameValidAsync(group, subGroupId, subGroupDto.Name, ct))
        {
            ModelState.AddModelError(nameof(subGroupDto.Name), "Subgroup name already exists in the institution");
            return Response.ValidationFailed(ModelState);
        }
        
        var updated = await _subGroupService.UpdateAsync(subGroupId, subGroupDto, group, institution, ct);
        return updated ? Response.Ok() : Response.NotFound($"Subgroup by id {subGroupId} is not found");
    }
    
    [HttpDelete("delete/{subGroupId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long groupId, long subGroupId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();
        
        var deleted = await _subGroupService.DeleteAsync(subGroupId, group, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Subgroup by id {subGroupId} is not found");
    }
}