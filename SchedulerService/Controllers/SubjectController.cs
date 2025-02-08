using System.Linq.Expressions;
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
[Route("api/v1/institutions/{institutionId:long}/subjects")]
public class SubjectController : ControllerBase
{
    private readonly IAdministratorService _administratorService;
    private readonly ISubjectService _subjectService;
    private readonly IInstitutionService _institutionService;
    private readonly ITeacherService _teacherService;

    public SubjectController(IAdministratorService administratorService,
        ISubjectService subjectService,
        IInstitutionService institutionService, 
        ITeacherService teacherService)
    {
        _administratorService = administratorService;
        _subjectService = subjectService;
        _institutionService = institutionService;
        _teacherService = teacherService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Subjects
            }, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();
        
        return Response.Ok(SubjectMapper.ToResponses(institution.Subjects));
    }

    [HttpGet("{subjectId:long}")]
    [ProducesResponseType(typeof(SubjectResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long subjectId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();

        var subject = await _subjectService.GetByIdAsync(subjectId, institution, ct: ct);

        return subject is null
            ? Response.NotFound($"Subject by id {subjectId} is not found")
            : Response.Ok(SubjectMapper.ToResponse(subject));
    }

    [HttpGet("teaching-all")]
    [ProducesResponseType(typeof(IEnumerable<SubjectResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetTeachingAll(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Teachers
            }, ct: ct);
        
        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        var teacher = await _teacherService.GetByIdAsync(userId, institution, ct: ct);
        
        if (teacher is null)
            return Response.Forbid();
        
        return Response.Ok(SubjectMapper.ToResponses(teacher.Subjects));
    }
    
    [HttpGet("{subjectId:long}/teachers")]
    [ProducesResponseType(typeof(IEnumerable<UserResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetTeachers(long institutionId, long subjectId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId,
            new Expression<Func<Institution, object?>>[]
            {
                e => e.Subjects,
                e => e.Teachers
            }, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var subject = await _subjectService.GetByIdAsync(subjectId, institution,
            new Expression<Func<Subject, object?>>[]
            {
                e => e.TeachingSubjects
            }, ct: ct);

        if (subject is null)
            return Response.NotFound($"Subject by id {subjectId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserBelongsToInstitutionAsync(institution, userId, ct))
            return Response.Forbid();
        
        return Response.Ok(UserMapper.ToResponses(subject.TeachingSubjects));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(SubjectResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, SubjectDto subjectDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();

        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        if (!await _subjectService.IsSubjectNameValidAsync(institution, subjectDto.Name, ct))
        {
            ModelState.AddModelError(nameof(subjectDto.Name), "Subject name already exists in the institution");
            return Response.ValidationFailed(ModelState);
        }

        var subject = await _subjectService.CreateAsync(subjectDto, institution, ct);
        return Response.Created(SubjectMapper.ToResponse(subject));
    }
    
    [HttpPut("update/{subjectId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long subjectId, SubjectDto subjectDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();

        if (!await _subjectService.IsSubjectNameValidAsync(institution, subjectId, subjectDto.Name, ct))
        {
            ModelState.AddModelError(nameof(subjectDto.Name), "Subject name already exists in the institution");
            return Response.ValidationFailed(ModelState);
        }
        
        var updated = await _subjectService.UpdateAsync(subjectId, subjectDto, institution, ct);
        return updated ? Response.Ok() : Response.NotFound($"Subject by id {subjectId} is not found");
    }
    
    [HttpDelete("delete/{subjectId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long subjectId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();
        
        if (!await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
            return Response.Forbid();
        
        var deleted = await _subjectService.DeleteAsync(subjectId, institution, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Subject by id {subjectId} is not found");
    }
}