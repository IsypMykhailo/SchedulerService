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
[Route("api/v1/institutions/{institutionId:long}/groups")]
public class GroupController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IGroupService _groupService;
    private readonly IInstitutionService _institutionService;
    private readonly ITeacherService _teacherService;
    private readonly IGradesJournalService _gradesJournalService;
    private readonly ISubjectService _subjectService;

    public GroupController(IAdministratorService administratorService,
        IGroupService groupService,
        IInstitutionService institutionService, 
        ITeacherService teacherService, 
        IGradesJournalService gradesJournalService, 
        ISubjectService subjectService)
    {
        _administratorService = administratorService;
        _groupService = groupService;
        _institutionService = institutionService;
        _teacherService = teacherService;
        _gradesJournalService = gradesJournalService;
        _subjectService = subjectService;
    }

    [HttpGet("{groupId:long}/journals")]
    [ProducesResponseType(typeof(IEnumerable<GradesJournalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetJournals(long institutionId, long groupId, [FromQuery] long? subjectId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

        if (teacher is null)
            return Response.Forbid();
    
        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);
        
        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");

        Subject? subject = null;
        if (subjectId is not null)
        {
            subject = await _subjectService.GetByIdAsync((long)subjectId, institution, ct: ct);

            if (subject is null)
                return Response.NotFound($"Subject by id {subjectId} is not found");
        }
        
        var journals = await _gradesJournalService.GetAllAsync(institution, teacher, group, subject, ct);

        return Response.Ok(GradesJournalMapper.ToResponses(journals));
    }
    
    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<GroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        var groups = await _groupService.GetAllAsStudentAsync(institution, userId, ct);

        var objEnumerable = groups as Group[] ?? groups.ToArray();
        if (objEnumerable.Any())
            return Response.Ok(GroupMapper.ToResponses(objEnumerable));
        
        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);
        if (teacher is not null)
        {
            groups = await _groupService.GetAllAsync(institution, teacher, ct);
            return Response.Ok(GroupMapper.ToResponses(groups));
        }

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        groups = await _groupService.GetAllAsync(institution, ct);
        return Response.Ok(GroupMapper.ToResponses(groups));
    }
    
    [HttpGet("{groupId:long}")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetById(long institutionId, long groupId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();
        
        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        return group is null
            ? Response.NotFound($"Group by id {institutionId} is not found")
            : Response.Ok(GroupMapper.ToResponse(group));
    }
    
    [HttpPost("create")]
    [ProducesResponseType(typeof(GroupResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, GroupDto groupDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        if (!await _groupService.IsGroupNameValidAsync(institutionId, groupDto.Name, ct))
        {
            ModelState.AddModelError(nameof(groupDto.Name), "Group name already exists in the institution");
            return Response.ValidationFailed(ModelState);
        }

        var headTeacher = await _teacherService.FindTeacherAsync(institution, groupDto.HeadTeacherId, ct);
        
        if (headTeacher is null)
        {
            ModelState.AddModelError(nameof(groupDto.HeadTeacherId), "Head Teacher is not a teacher in the institution");
            return Response.ValidationFailed(ModelState);
        }
        
        var group = await _groupService.CreateAsync(groupDto, institution, headTeacher, ct);
        return Response.Created(GroupMapper.ToResponse(group));
    }
    
    [HttpPut("update/{groupId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long groupId, GroupDto groupDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        if (!await _groupService.IsGroupNameValidAsync(institutionId, groupId, groupDto.Name, ct))
        {
            ModelState.AddModelError(nameof(groupDto.Name), "Group name already exists in the institution");
            return Response.ValidationFailed(ModelState);
        }

        var headTeacher = await _teacherService.FindTeacherAsync(institution, groupDto.HeadTeacherId, ct);
        
        if (headTeacher is null)
        {
            ModelState.AddModelError(nameof(groupDto.HeadTeacherId), "Head Teacher is not a teacher in the institution");
            return Response.ValidationFailed(ModelState);
        }
        
        var updated = await _groupService.UpdateAsync(groupId, groupDto, institution, headTeacher, ct);
        return updated ? Response.Ok() : Response.NotFound($"Group by id {groupId} is not found");
    }
    
    [HttpDelete("delete/{groupId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long groupId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        var deleted = await _groupService.DeleteAsync(groupId, institution, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Group by id {groupId} is not found");
    }
}