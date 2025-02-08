using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using SchedulerService.Repositories;
using SchedulerService.Services;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/teachers/{teacherId}/subjects")]
public class TeachingSubjectController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly ITeacherService _teacherService;
    private readonly IInstitutionService _institutionService;
    private readonly ISubjectService _subjectService;
    private readonly ICrudRepository<User> _teacherRepository;

    public TeachingSubjectController(IAdministratorService administratorService,
        ITeacherService teacherService,
        IInstitutionService institutionService,
        ISubjectService subjectService,
        ICrudRepository<User> teacherRepository)
    {
        _administratorService = administratorService;
        _teacherService = teacherService;
        _institutionService = institutionService;
        _subjectService = subjectService;
        _teacherRepository = teacherRepository;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, string teacherId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Teachers
            }, ct: ct);
        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var teacher = await _teacherService.GetByIdAsync(teacherId, institution, ct: ct);
        if (teacher is null)
            return Response.NotFound($"Teacher by id {teacherId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();
        
        return Response.Ok(SubjectMapper.ToResponses(teacher.Subjects));
    }

    [HttpPost("add/{subjectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> AddSubjectToTeacher(long institutionId, string teacherId, long subjectId,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Teachers
            }, ct: ct);
        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var teacher = await _teacherService.GetByIdAsync(teacherId, institution, ct: ct);
        if (teacher is null)
            return Response.NotFound($"Teacher by id {teacherId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var subject = await _subjectService.GetByIdAsync(subjectId, institution, ct: ct);
        
        if (subject is null)
            return Response.NotFound($"Subject by id {subjectId} is not found");
        
        teacher.Subjects.Add(subject);
        await _teacherRepository.SaveAsync(ct);
        return Response.Ok();
    }
    
    [HttpDelete("remove/{subjectId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> RemoveSubjectFromTeacher(long institutionId, string teacherId, long subjectId,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Teachers
            }, ct: ct);
        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var teacher = await _teacherService.GetByIdAsync(teacherId, institution, ct: ct);
        if (teacher is null)
            return Response.NotFound($"Teacher by id {teacherId} is not found");

        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        var subject = await _subjectService.GetByIdAsync(subjectId, institution, ct: ct);
        
        if (subject is null)
            return Response.NotFound($"Subject by id {subjectId} is not found");
        
        teacher.Subjects.Remove(subject);
        await _teacherRepository.SaveAsync(ct);
        return Response.Ok();
    }
}