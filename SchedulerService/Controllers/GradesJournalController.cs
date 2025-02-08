using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
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
[Route("api/v1/institutions/{institutionId:long}/journals")]
public class GradesJournalController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly ITeacherService _teacherService;
    private readonly IGradesJournalService _gradesJournalService;
    private readonly ISubjectService _subjectService;
    private readonly ILessonsScheduleItemService _lessonsScheduleItemService;
    private readonly IAdministratorService _administratorService;

    public GradesJournalController(IInstitutionService institutionService,
        ITeacherService teacherService, 
        IGradesJournalService gradesJournalService, 
        ISubjectService subjectService,
        ILessonsScheduleItemService lessonsScheduleItemService, 
        IAdministratorService administratorService)
    {
        _institutionService = institutionService;
        _teacherService = teacherService;
        _gradesJournalService = gradesJournalService;
        _subjectService = subjectService;
        _lessonsScheduleItemService = lessonsScheduleItemService;
        _administratorService = administratorService;
    }

    [HttpGet("get-teaching-groups")]
    [ProducesResponseType(typeof(IEnumerable<GroupResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetTeachingGroups(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

        if (teacher is null)
            return Response.Forbid();

        var groups = await _lessonsScheduleItemService.FindTeachingGroupsAsync(teacher, ct);
        return Response.Ok(GroupMapper.ToResponses(groups));
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<GradesJournalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        IEnumerable<GradesJournal>? journals;
        
        if (await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
        {
            journals = await _gradesJournalService.GetAllAsync(institution, ct);
        }
        else
        {
            var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

            if (teacher is null)
                return Response.Forbid();

            journals = await _gradesJournalService.GetAllAsync(institution, teacher, ct);
        }
        
        return Response.Ok(GradesJournalMapper.ToResponses(journals));
    }

    [HttpGet("{journalId:long}")]
    [ProducesResponseType(typeof(IEnumerable<GradesJournalResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> GetById(long institutionId, long journalId, [FromQuery] bool withGrades, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        (GradesJournal? journal, bool hasAccessToEdit) journalTuple;
        
        if (await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
        {
            journalTuple = (await GetJournalAsync(journalId, institution, withGrades, ct: ct), false);
        }
        else
        {
            var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

            if (teacher is null)
                return Response.Forbid();

            var journal = await GetJournalAsync(journalId, institution, withGrades, teacher, ct);

            journalTuple = journal is not null ? (journal, journal.TeacherId == userId) : (null, false);
        }
        
        return journalTuple.journal is null ? Response.NotFound($"Grades journal by id {journalId} is not found") : Response.Ok(GradesJournalMapper.ToResponse(journalTuple!));
    }

    private async Task<GradesJournal?> GetJournalAsync(long journalId, Institution institution, bool includeGrades, User? teacher = null,
        CancellationToken ct = default)
    {
        Func<IQueryable<GradesJournal>, IIncludableQueryable<GradesJournal, object>> include;
        if (includeGrades)
        {
            include = journals =>
                journals
                    .Include(e => e.Group)
                        .ThenInclude(e => e.Students)
                    .Include(e => e.Group)
                        .ThenInclude(e => e.ParentGroup)
                    .Include(e => e.Subject)
                    .Include(e => e.Columns)
                        .ThenInclude(e => e.Grades)
                    .Include(e => e.Columns)
                        .ThenInclude(e => e.Lesson)
                            .ThenInclude(e => e!.Subject)
                    .Include(e => e.Columns)
                        .ThenInclude(e => e.Lesson)
                            .ThenInclude(e => e!.SubGroup)
                    .Include(e => e.Columns)
                        .ThenInclude(e => e.Homework)!;
        }
        else
        {
            include = journals =>
                journals
                    .Include(e => e.Group)
                        .ThenInclude(e => e.Students)
                    .Include(e => e.Group)
                        .ThenInclude(e => e.ParentGroup)
                    .Include(e => e.Subject);
        }
        
        
        if (teacher is null)
            return await _gradesJournalService.GetByIdAsync(journalId, institution, include, ct: ct);
        
        return await _gradesJournalService.GetByIdAsync(journalId, institution, teacher, true, include, ct: ct);
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(GradesJournalResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, GradesJournalDto gradesJournalDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var subject = await _subjectService.GetByIdAsync(gradesJournalDto.SubjectId, institution, ct: ct);

        if (subject is null)
        {
            ModelState.AddModelError(nameof(gradesJournalDto.SubjectId), "Subject is not found in this institution");
            return Response.ValidationFailed(ModelState);
        }
        
        var userId = User.GetIdFromClaims();

        var teacher = await _teacherService.FindTeacherWithSubjectAsync(institution, subject, userId, ct);

        if (teacher is null)
            return Response.Forbid();

        var group = await FindTeachingGroupAsync(gradesJournalDto.GroupId, teacher, ct);
        
        if (group is null)
        {
            ModelState.AddModelError(nameof(gradesJournalDto.GroupId), $"User is not teaching group by id {gradesJournalDto.GroupId}");
            return Response.ValidationFailed(ModelState);
        }

        var journal = await _gradesJournalService.CreateAsync(gradesJournalDto, institution, subject, teacher, group, ct);
        return Response.Created(GradesJournalMapper.ToResponse(journal));
    }

    [HttpPut("update/{journalId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long journalId, UpdatedGradesJournalDto gradesJournalDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();

        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

        if (teacher is null)
            return Response.Forbid();

        var updated = await _gradesJournalService.UpdateAsync(journalId, gradesJournalDto, institution,
            teacher, teacher.Id, ct);
        return updated ? Response.Ok() : Response.NotFound($"Grades journal by id {journalId} is not found");
    }
    
    [HttpDelete("delete/{journalId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long journalId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        var teacher = await _gradesJournalService.FindTeacherOfJournalAsync(journalId, userId, ct);

        if (teacher is null)
            return Response.Forbid();
        
        var deleted = await _gradesJournalService.DeleteAsync(journalId, institution, teacher, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Grades journal by id {journalId} is not found");
    }

    private async Task<Group?> FindTeachingGroupAsync(long groupId, User teacher, CancellationToken ct)
    {
        var groups = await _lessonsScheduleItemService.FindTeachingGroupsAsync(teacher, ct);
        return groups.FirstOrDefault(group => group.Id == groupId);
    } 
}
