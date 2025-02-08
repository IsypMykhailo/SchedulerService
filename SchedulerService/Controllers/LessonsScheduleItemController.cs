using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain;
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
[Route("api/v1/institutions/{institutionId:long}/groups/{groupId:long}/lessons-schedule/items")]
public class LessonsScheduleItemController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly IInstitutionService _institutionService;
    private readonly IGroupService _groupService;
    private readonly ISubjectService _subjectService;
    private readonly ILessonsScheduleService _lessonsScheduleService;
    private readonly ILessonsScheduleItemService _lessonsScheduleItemService;
    private readonly ISubGroupService _subGroupService;
    private readonly ITeacherService _teacherService;
    private readonly IGroupStudentService _groupStudentService;

    public LessonsScheduleItemController(IAdministratorService administratorService,
        IInstitutionService institutionService, 
        IGroupService groupService,
        ILessonsScheduleService lessonsScheduleService, 
        ILessonsScheduleItemService lessonsScheduleItemService,
        ISubjectService subjectService,
        ISubGroupService subGroupService, 
        ITeacherService teacherService, 
        IGroupStudentService groupStudentService)
    {
        _administratorService = administratorService;
        _institutionService = institutionService;
        _groupService = groupService;
        _lessonsScheduleService = lessonsScheduleService;
        _lessonsScheduleItemService = lessonsScheduleItemService;
        _subjectService = subjectService;
        _subGroupService = subGroupService;
        _teacherService = teacherService;
        _groupStudentService = groupStudentService;
    }

    [HttpGet("get")]
    [ProducesResponseType(typeof(IEnumerable<LessonsScheduleItemResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long groupId, 
        [FromQuery] DateOnly? start, [FromQuery] DateOnly? end, [FromQuery] string? teacherId,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();

        var student = await _groupStudentService.FindStudentAsync(group, userId, ct);

        if (student is null && !await _administratorService.IsUserHeadTeacherOrAdminOrOwnerAsync(institution, userId, group, ct))
            return Response.Forbid();

        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        
        if (lessonsSchedule is null)
            return Response.NotFound("Lesson schedule is not found");

        IEnumerable<LessonsScheduleItem> lessons;
        
        if (student is not null)
            lessons = await _lessonsScheduleItemService.GetAsync(lessonsSchedule, student, start, end, ct);
        else if (teacherId is not null)
            lessons = await _lessonsScheduleItemService.GetTeachersLessonsInGroupAsync(lessonsSchedule, teacherId, start, end, ct);
        else
            lessons = await _lessonsScheduleItemService.GetAsync(lessonsSchedule, start, end, ct);

        var responses = LessonsScheduleItemMapper.ToResponses(lessons);
        return Response.Ok(responses.OrderBy(e => e.Date).ThenBy(e => e.LessonIndex));
    }

    [HttpGet("{lessonId:long}")]
    [ProducesResponseType(typeof(LessonsScheduleItemResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long groupId, long lessonId, CancellationToken ct)
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
        
        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        
        if (lessonsSchedule is null)
            return Response.NotFound("Lesson schedule is not found");

        var lesson = await _lessonsScheduleItemService.GetByIdAsync(lessonId, lessonsSchedule, teacher, new Expression<Func<LessonsScheduleItem, object?>>[]
        {
            e => e.Subject,
            e => e.SubGroup,
        }, ct: ct);
        
        return lesson is null
            ? Response.NotFound($"Lesson by id {lessonId} is not found")
            : Response.Ok(LessonsScheduleItemMapper.ToResponse(lesson));
    }
    
    [HttpPost("create-one")]
    [ProducesResponseType(typeof(LessonsScheduleItemResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, long groupId,
        SingleLessonsScheduleItemDto singleLessonsScheduleItemDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        
        if (lessonsSchedule is null)
            return Response.NotFound("Lesson schedule is not found");
        
        var subject = await _subjectService.GetByIdAsync(singleLessonsScheduleItemDto.SubjectId, institution, ct: ct);

        if (subject is null)
        {
            ModelState.AddModelError(nameof(singleLessonsScheduleItemDto.SubjectId), "Subject is not found in this institution");
            return Response.ValidationFailed(ModelState);
        }

        Group? subGroup = null;
        
        if (singleLessonsScheduleItemDto.SubGroupId is not null)
        {
            subGroup = await _subGroupService.GetByIdAsync((long)singleLessonsScheduleItemDto.SubGroupId, group, ct: ct);

            if (subGroup is null)
            {
                ModelState.AddModelError(nameof(singleLessonsScheduleItemDto.SubGroupId), "Sub group is not found in this institution");
                return Response.ValidationFailed(ModelState);
            }
        }

        User? teacher = null;
        
        if (!string.IsNullOrWhiteSpace(singleLessonsScheduleItemDto.TeacherId))
        {
            teacher = await _teacherService.FindTeacherWithSubjectAsync(institution, subject, singleLessonsScheduleItemDto.TeacherId, ct);

            if (teacher is null)
            {
                ModelState.AddModelError(nameof(singleLessonsScheduleItemDto.TeacherId), $"Teacher is not found in this institution with subject by id {singleLessonsScheduleItemDto.SubjectId}");
                return Response.ValidationFailed(ModelState);
            }
        }

        var lessonsScheduleItem = await _lessonsScheduleItemService.CreateAsync(singleLessonsScheduleItemDto, lessonsSchedule,
            subject, subGroup, teacher, ct);

        return Response.Created(LessonsScheduleItemMapper.ToResponse(lessonsScheduleItem));
    }
    
    [HttpPost("create-range")]
    [ProducesResponseType(typeof(IEnumerable<LessonsScheduleItemResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, long groupId,
        RangeLessonsScheduleItemDto rangeLessonsScheduleItemDto,
        [FromQuery] LessonFrequency lessonFrequency, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        
        if (lessonsSchedule is null)
            return Response.NotFound("Lesson schedule is not found");

        var subject = await _subjectService.GetByIdAsync(rangeLessonsScheduleItemDto.SubjectId, institution, ct: ct);

        if (subject is null)
        {
            ModelState.AddModelError(nameof(rangeLessonsScheduleItemDto.SubjectId), "Subject is not found in this institution");
            return Response.ValidationFailed(ModelState);
        }

        Group? subGroup = null;
        
        if (rangeLessonsScheduleItemDto.SubGroupId is not null)
        {
            subGroup = await _subGroupService.GetByIdAsync((long)rangeLessonsScheduleItemDto.SubGroupId, group, ct: ct);

            if (subGroup is null)
            {
                ModelState.AddModelError(nameof(rangeLessonsScheduleItemDto.SubGroupId), "Sub group is not found in this institution");
                return Response.ValidationFailed(ModelState);
            }
        }

        User? teacher = null;
        
        if (!string.IsNullOrWhiteSpace(rangeLessonsScheduleItemDto.TeacherId))
        {
            teacher = await _teacherService.FindTeacherWithSubjectAsync(institution, subject, rangeLessonsScheduleItemDto.TeacherId, ct);

            if (teacher is null)
            {
                ModelState.AddModelError(nameof(rangeLessonsScheduleItemDto.TeacherId), $"Teacher is not found in this institution with subject by id {rangeLessonsScheduleItemDto.SubjectId}");
                return Response.ValidationFailed(ModelState);
            }
        }

        var lessons = 
            await _lessonsScheduleItemService.CreateRangeAsync(rangeLessonsScheduleItemDto,
                lessonFrequency,
                lessonsSchedule,
                subject,
                subGroup,
                teacher,
                ct);

        var responses = LessonsScheduleItemMapper.ToResponses(lessons);
        return Response.Created(responses.OrderBy(e => e.Date));
    }

    [HttpPut("update/{lessonId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long groupId, long lessonId,
        SingleLessonsScheduleItemDto singleLessonsScheduleItemDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);

        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        
        if (lessonsSchedule is null)
            return Response.NotFound("Lesson schedule is not found");

        var subject = await _subjectService.GetByIdAsync(singleLessonsScheduleItemDto.SubjectId, institution, ct: ct);

        if (subject is null)
        {
            ModelState.AddModelError(nameof(singleLessonsScheduleItemDto.SubjectId), "Subject is not found in this institution");
            return Response.ValidationFailed(ModelState);
        }

        Group? subGroup = null;
        
        if (singleLessonsScheduleItemDto.SubGroupId is not null)
        {
            subGroup = await _subGroupService.GetByIdAsync((long)singleLessonsScheduleItemDto.SubGroupId, group, ct: ct);

            if (subGroup is null)
            {
                ModelState.AddModelError(nameof(singleLessonsScheduleItemDto.SubGroupId), "Sub group is not found in this institution");
                return Response.ValidationFailed(ModelState);
            }
        }

        User? teacher = null;
        
        if (!string.IsNullOrWhiteSpace(singleLessonsScheduleItemDto.TeacherId))
        {
            teacher = await _teacherService.FindTeacherWithSubjectAsync(institution, subject, singleLessonsScheduleItemDto.TeacherId, ct);

            if (teacher is null)
            {
                ModelState.AddModelError(nameof(singleLessonsScheduleItemDto.TeacherId), $"Teacher is not found in this institution with subject by id {singleLessonsScheduleItemDto.SubjectId}");
                return Response.ValidationFailed(ModelState);
            }
        }

        var updated = await _lessonsScheduleItemService.UpdateAsync(lessonId, singleLessonsScheduleItemDto, lessonsSchedule,
            subject, subGroup, teacher, ct);
        return updated ? Response.Ok() : Response.NotFound($"Lesson by id {lessonId} is not found");
    }
    
    [HttpPut("{lessonId:long}/info/update")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> UpdateInfo(long institutionId, long groupId, long lessonId,
        UpdatedLessonInfoDto lessonInfoDto, CancellationToken ct)
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
        
        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        
        if (lessonsSchedule is null)
            return Response.NotFound("Lesson schedule is not found");

        var updated = await _lessonsScheduleItemService.UpdateInfoAsync(lessonId, lessonInfoDto, lessonsSchedule, teacher, ct);
        return updated ? Response.Ok() : Response.NotFound($"Lesson by id {lessonId} is not found");
    }
    
    [HttpDelete("delete/{lessonId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long groupId, long lessonId, 
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var group = await _groupService.GetByIdAsync(groupId, institution, ct: ct);
        
        if (group is null)
            return Response.NotFound($"Group by id {groupId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var lessonsSchedule = await _lessonsScheduleService.GetAsync(group, ct);
        
        if (lessonsSchedule is null)
            return Response.NotFound("Lesson schedule is not found");
        
        var deleted = await _lessonsScheduleItemService.DeleteAsync(lessonId, lessonsSchedule, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Lesson by id {lessonId} is not found");
    }
}
