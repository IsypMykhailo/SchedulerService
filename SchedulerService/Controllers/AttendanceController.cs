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
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/lessons/{lessonId:long}/attendance")]
public class AttendanceController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly ILessonsScheduleItemService _lessonsService;
    private readonly ITeacherService _teacherService;
    private readonly IAttendanceService _attendanceService;
    private readonly IGroupStudentService _groupStudentService;

    public AttendanceController(ILessonsScheduleItemService lessonsService,
        ITeacherService teacherService,
        IInstitutionService institutionService, 
        IAttendanceService attendanceService, 
        IGroupStudentService groupStudentService)
    {
        _lessonsService = lessonsService;
        _teacherService = teacherService;
        _institutionService = institutionService;
        _attendanceService = attendanceService;
        _groupStudentService = groupStudentService;
    }

    [HttpGet("get")]
    [ProducesResponseType(typeof(IEnumerable<AttendanceResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long lessonId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var lesson = await _lessonsService.GetByIdAsync(lessonId,
            new Expression<Func<LessonsScheduleItem, object?>>[]
            {
                e => e.SubGroup!.Students,
                e => e.LessonsSchedule.Group.Students
            }, ct: ct);

        var userId = User.GetIdFromClaims();
        
        if (lesson is null || lesson.TeacherId != userId)
            return Response.NotFound($"Lesson by id {lessonId} is not found");

        if (await _teacherService.FindTeacherAsync(institution, userId, ct) is null)
            return Response.Forbid();

        var attendees = await _attendanceService.GetAsync(lesson, ct);
        var students = lesson.SubGroup == null ? lesson.LessonsSchedule.Group.Students : lesson.SubGroup.Students;

        var studentsAttendees = students.Select(x =>
        {
            var attendance = attendees.FirstOrDefault(e => e.StudentId == x.Id);
            var attendanceResponse = attendance == null ? null : AttendanceMapper.ToResponse(attendance);
            
            return new KeyValuePair<string, AttendanceResponse?>(x.Id, attendanceResponse);
        });

        return Response.Ok(studentsAttendees);

    }

    [HttpPost("check-one")]
    [ProducesResponseType(typeof(AttendanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> CheckOne(long institutionId, long lessonId, AttendanceDto attendanceDto, 
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var lesson = await _lessonsService.GetByIdAsync(lessonId,
            new Expression<Func<LessonsScheduleItem, object?>>[]
            {
                e => e.SubGroup,
                e => e.LessonsSchedule.Group
            }, ct: ct);

        var userId = User.GetIdFromClaims();
        
        if (lesson is null || lesson.TeacherId != userId)
            return Response.NotFound($"Lesson by id {lessonId} is not found");

        if (await _teacherService.FindTeacherAsync(institution, userId, ct) is null)
            return Response.Forbid();

        var student = await _groupStudentService.FindStudentAsync(lesson.SubGroup ?? lesson.LessonsSchedule.Group, attendanceDto.StudentId, ct);

        if (student is null)
            return Response.NotFound($"Student by id {attendanceDto.StudentId} is not a group student");
        
        var attendance = await _attendanceService.CreateOrUpdateAsync(lesson, attendanceDto, student, ct);
        return Response.Ok(AttendanceMapper.ToResponse(attendance));
    }

    [HttpPost("check-range")]
    [ProducesResponseType(typeof(AttendanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> CheckOne(long institutionId, long lessonId, AttendanceDto[] attendanceDtos, 
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var lesson = await _lessonsService.GetByIdAsync(lessonId,
            new Expression<Func<LessonsScheduleItem, object?>>[]
            {
                e => e.SubGroup,
                e => e.LessonsSchedule.Group
            }, ct: ct);

        var userId = User.GetIdFromClaims();
        
        if (lesson is null || lesson.TeacherId != userId)
            return Response.NotFound($"Lesson by id {lessonId} is not found");

        if (await _teacherService.FindTeacherAsync(institution, userId, ct) is null)
            return Response.Forbid();

        var attendances = new List<Attendance>(attendanceDtos.Length);

        foreach (var dto in attendanceDtos)
        {
            var student = await _groupStudentService.FindStudentAsync(lesson.SubGroup ?? lesson.LessonsSchedule.Group, dto.StudentId, ct);

            if (student is null)
                return Response.NotFound($"Student by id {dto.StudentId} is not a group student");
        
            attendances.Add(await _attendanceService.CreateOrUpdateAsync(lesson, dto, student, ct));
        }
        
        return Response.Ok(AttendanceMapper.ToResponses(attendances));
    }
    
    [HttpPost("check-all")]
    [ProducesResponseType(typeof(AttendanceResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> CheckAll(long institutionId, long lessonId, AttendanceType attendanceType, 
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var lesson = await _lessonsService.GetByIdAsync(lessonId,
            new Expression<Func<LessonsScheduleItem, object?>>[]
            {
                e => e.SubGroup,
                e => e.LessonsSchedule.Group
            }, ct: ct);

        var userId = User.GetIdFromClaims();
        
        if (lesson is null || lesson.TeacherId != userId)
            return Response.NotFound($"Lesson by id {lessonId} is not found");

        if (await _teacherService.FindTeacherAsync(institution, userId, ct) is null)
            return Response.Forbid();

        var attendances = await _attendanceService.CreateOrUpdateRangeAsync(lesson, lesson.SubGroup ?? lesson.LessonsSchedule.Group, attendanceType, ct);
        return Response.Ok(AttendanceMapper.ToResponses(attendances));
    }
}
