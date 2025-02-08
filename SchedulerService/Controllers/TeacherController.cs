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
[Route("api/v1/institutions/{institutionId:long}/teachers")]
public class TeachersController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly ITeacherService _teacherService;
    private readonly IInstitutionService _institutionService;
    private readonly ILessonsScheduleItemService _lessonsScheduleItemService;

    public TeachersController(IAdministratorService administratorService,
        ITeacherService teacherService,
        IInstitutionService institutionService, 
        ILessonsScheduleItemService lessonsScheduleItemService)
    {
        _administratorService = administratorService;
        _teacherService = teacherService;
        _institutionService = institutionService;
        _lessonsScheduleItemService = lessonsScheduleItemService;
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
                e => e.Teachers
            }, ct: ct);
        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        return Response.Ok(UserMapper.ToResponses(institution.Teachers));
    }

    [HttpPost("add/{teacherId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> AddTeacher(long institutionId, string teacherId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Teachers
            }, ct: ct);
        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        if (await _administratorService.IsUserBelongsToInstitutionAsync(institution, teacherId, ct))
        {
            ModelState.AddModelError("User", "User is a member of this institution");
            return Response.ValidationFailed(ModelState);
        }

        await _teacherService.AddTeacherAsync(institution, teacherId, ct);
        return Response.Ok();
    }

    [HttpDelete("remove/{teacherId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveTeacher(long institutionId, string teacherId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Teachers
            }, ct: ct);
        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        if (await _teacherService.IsUserHeadTeacherAsync(teacherId, ct))
        {
            ModelState.AddModelError(nameof(teacherId), "Can not remove teacher because he is a head teacher");
            return Response.ValidationFailed(ModelState);
        }
        
        var removed = await _teacherService.RemoveTeacherAsync(institution, teacherId, ct);
        return removed ? Response.Ok() : Response.NotFound($"Teacher by id {teacherId} is not found");
    }
    
    [HttpGet("lessons")]
    [ProducesResponseType(typeof(IEnumerable<LessonsScheduleItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetTeacherLessons(long institutionId,
        [FromQuery] DateOnly? start, [FromQuery] DateOnly? end,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

        if (teacher is null)
            return Response.Forbid();
        
        var lessons = 
            await _lessonsScheduleItemService.GetAllTeachersLessonsAsync(teacher, start, end, ct);

        var responses = TeacherLessonMapper.ToResponses(lessons.Where(e => e.LessonsSchedule.Group != null));
        return Response.Ok(responses.OrderBy(e => e.Date).ThenBy(e => e.LessonIndex));
    }
}