using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Response;
using SchedulerService.Services;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Extensions;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/groups/{groupId:long}/homeworks")]
public class HomeworksController : ControllerBase
{
    private readonly IHomeworksService _homeworksService;
    private readonly IInstitutionService _institutionService;
    private readonly IGroupService _groupService;
    private readonly IHomeworkItemsService _homeworkItemsService;
    private readonly IAdministratorService _administratorService;

    public HomeworksController(IHomeworksService homeworksService, 
        IInstitutionService institutionService, 
        IGroupService groupService, 
        IHomeworkItemsService homeworkItemsService, 
        IAdministratorService administratorService)
    {
        _homeworksService = homeworksService;
        _institutionService = institutionService;
        _groupService = groupService;
        _homeworkItemsService = homeworkItemsService;
        _administratorService = administratorService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<HomeworksResponse>), StatusCodes.Status200OK)]
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
        
        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Response.Forbid();
        
        var homeworks = (await _homeworksService.GetAllAsync(groupId, ct)).OrderBy(e => e.DueDate);
        return Response.Ok(HomeworksMapper.ToResponses(homeworks));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(HomeworksResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, long groupId, HomeworksDto homeworksDto, CancellationToken ct)
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
        
        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Response.Forbid();
        var homework = await _homeworksService.CreateAsync(homeworksDto, userId, groupId, ct);
        return Response.Created(HomeworksMapper.ToResponse(homework));
    }

    [HttpPut("update/{homeworkId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long homeworkId, long groupId, HomeworksDto homeworksDto, CancellationToken ct)
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
        
        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Response.Forbid();
        
        var updated = await _homeworksService.UpdateAsync(homeworkId, homeworksDto, userId, groupId, ct);
        return updated ? Response.Ok() : Response.NotFound($"Homework by id {homeworkId} is not found");
    }

    [HttpDelete("delete/{homeworkId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long groupId, long homeworkId, CancellationToken ct)
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
        
        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Response.Forbid();
        
        var deleted = await _homeworksService.DeleteAsync(homeworkId, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Homework by id {homeworkId} is not found");
    }
}