using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using SchedulerService.Services;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Extensions;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/groups/{groupId:long}/homeworks/{homeworkId:long}/items")]
public class HomeworkItemsController : ControllerBase
{
    private readonly IHomeworksService _homeworksService;
    private readonly IHomeworkItemsService _homeworkItemsService;
    private readonly IInstitutionService _institutionService;
    private readonly IGroupService _groupService;

    public HomeworkItemsController(IHomeworksService homeworksService, 
        IHomeworkItemsService homeworkItemsService, 
        IInstitutionService institutionService, 
        IGroupService groupService)
    {
        _homeworksService = homeworksService;
        _homeworkItemsService = homeworkItemsService;
        _institutionService = institutionService;
        _groupService = groupService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<HomeworkItemsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetAll(long institutionId, long groupId, long homeworkId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var homework = await _homeworksService.GetByIdAsync(homeworkId,
            groupId,
            new Expression<Func<Homeworks, object?>>[]
            {
                e => e.Items
            }, ct: ct);
        
        if (homework is null)
            return Response.NotFound($"Homework by id {homeworkId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _homeworksService.IsUserTeacherAsync(userId, ct))
            return Response.Forbid();

        return Response.Ok(HomeworkItemsMapper.ToResponses(homework.Items));
    }

    [HttpGet("students/{studentId}")]
    [ProducesResponseType(typeof(IEnumerable<HomeworkItemsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetOne(long institutionId, string studentId, long groupId, long homeworkId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var homework = await _homeworkItemsService.GetByIdAsync(
            homeworkId, studentId, groupId, ct: ct);

        if (homework is null)
            return Response.NotFound($"HomeworkItem by id {homeworkId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Response.Forbid();

        return Response.Ok(HomeworkItemsMapper.ToResponse(homework));
    }

    [HttpGet("my-homework")]
    [ProducesResponseType(typeof(IEnumerable<HomeworkItemsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetMy(long institutionId, long groupId, long homeworkId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        var userId = User.GetIdFromClaims();
        var homework = await _homeworkItemsService.GetByIdAsync(
            homeworkId, userId, groupId, ct: ct);

        if (homework is null)
            return Response.NotFound($"HomeworkItem by id {homeworkId} is not found");
        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Response.Forbid();
        return Response.Ok(HomeworkItemsMapper.ToResponse(homework));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(HomeworkItemsResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, long groupId, long homeworkId, HomeworkItemsDto dto, 
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, ct: ct);
        
        if (homework is null)
            return Response.NotFound($"Homework by id {homeworkId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _homeworkItemsService.IsUserStudentAsync(userId, ct))
            return Response.Forbid();

        if (await _homeworkItemsService.HomeworkItemExists(homework.CreatorId, homeworkId, userId, ct: ct))
            return Response.Forbid();

        var homeworkItem = await _homeworkItemsService.CreateAsync(dto, homework, userId, ct);
        return Response.Created(HomeworkItemsMapper.ToResponse(homeworkItem));
    }

    [HttpPut("update/{homeworkItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long groupId, long homeworkId,
        long homeworkItemId, HomeworkItemsDto dto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, ct: ct);
        
        if (homework is null)
            return Response.NotFound($"Homework by id {homeworkId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Response.Forbid();

        if (await _homeworksService.IsUserTeacherAsync(userId, ct))
        {
            var updated = await _homeworkItemsService.TeacherUpdateAsync(homeworkItemId, dto, homework, userId, ct);
            return updated ? Response.Ok() : Response.NotFound($"HomeworkItem by id {homeworkItemId} is not found");
        }
        else
        {
            var updated = await _homeworkItemsService.StudentUpdateAsync(homeworkItemId, dto, homework, userId, ct);
            return updated ? Response.Ok() : Response.NotFound($"HomeworkItem by id {homeworkItemId} is not found");

        }
    }

    [HttpDelete("delete/{homeworkItemId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long groupId, long homeworkId, long homeworkItemId,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var homework = await _homeworksService.GetByIdAsync(homeworkId, groupId, ct: ct);

        if (homework is null)
            return Response.NotFound($"Homework by id {homeworkId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _homeworkItemsService.IsUserTeacherOrStudentAsync(userId, ct))
            return Response.Forbid();

        var deleted = await _homeworkItemsService.DeleteAsync(homeworkItemId, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Homework Item by id {homeworkItemId} is not found");
    }
}