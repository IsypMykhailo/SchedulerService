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
[Route("api/v1/institutions/{institutionId:long}/journals/{journalId:long}/columns")]
public class GradesJournalColumnController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly IAdministratorService _administratorService;
    private readonly ITeacherService _teacherService;
    private readonly IGradesJournalService _gradesJournalService;
    private readonly ILessonsScheduleItemService _lessonsScheduleItemService;
    private readonly IGradesJournalColumnService _gradesJournalColumnService;
    private readonly IHomeworksService _homeworksService;

    public GradesJournalColumnController(IInstitutionService institutionService,
        ITeacherService teacherService, 
        IGradesJournalService gradesJournalService,
        ILessonsScheduleItemService lessonsScheduleItemService, 
        IGradesJournalColumnService gradesJournalColumnService, 
        IAdministratorService administratorService, 
        IHomeworksService homeworksService)
    {
        _institutionService = institutionService;
        _teacherService = teacherService;
        _gradesJournalService = gradesJournalService;
        _lessonsScheduleItemService = lessonsScheduleItemService;
        _gradesJournalColumnService = gradesJournalColumnService;
        _administratorService = administratorService;
        _homeworksService = homeworksService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<GradesJournalColumnResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long journalId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        GradesJournal? journal;

        if (await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
        {
            journal = await _gradesJournalService.GetByIdAsync(journalId, institution, ct: ct);

            if (journal is null)
                return Response.NotFound($"Journal by id {journalId} is not found");
        }
        else
        {
            var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

            if (teacher is null)
                return Response.Forbid();

            journal = await _gradesJournalService.GetByIdAsync(journalId, institution, teacher, true, includeProperties: null, ct: ct);
        }
        
        if (journal is null)
            return Response.NotFound($"Journal by id {journalId} is not found");

        var columns = await _gradesJournalColumnService.GetAllAsync(journal, ct);
        return Response.Ok(GradesJournalColumnMapper.ToResponses(columns));
    }

    [HttpGet("{columnId:long}")]
    [ProducesResponseType(typeof(IEnumerable<GradesJournalColumnResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long journalId, long columnId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

        if (teacher is null)
            return Response.Forbid();

        var journal = await _gradesJournalService.GetByIdAsync(journalId, institution, teacher, false, includeProperties: null, ct: ct);
        
        if (journal is null)
            return Response.NotFound($"Journal by id {journalId} is not found");

        var column = await _gradesJournalColumnService.GetByIdAsync(columnId, journal, ct: ct);
        
        return column is null
            ? Response.NotFound($"Grades journal column by id {columnId} is not found")
            : Response.Ok(GradesJournalColumnMapper.ToResponse(column));
    }

    [HttpPost("create")]
    [ProducesResponseType(typeof(GradesJournalColumnResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Create(long institutionId, long journalId, GradesJournalColumnDto gradesJournalColumnDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();

        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

        if (teacher is null)
            return Response.Forbid();

        var journal = await _gradesJournalService.GetByIdAsync(journalId, institution, teacher,
            includeProperties: new Expression<Func<GradesJournal, object?>>[]
            {
                e => e.Group,
                e => e.Group.LessonsSchedule,
                e => e.Group.ParentGroup,
                e => e.Group.ParentGroup!.LessonsSchedule
            },
            ct: ct);

        if (journal is null)
            return Response.NotFound($"Journal by id {journalId} is not found");

        LessonsScheduleItem? lesson = null;
        Homeworks? homework = null;

        if (gradesJournalColumnDto.LessonId is not null)
        {
            var lessonSchedule = journal.Group.LessonsSchedule ?? journal.Group.ParentGroup?.LessonsSchedule;
            
            if (lessonSchedule is null)
                return Response.NotFound($"Lesson schedule of group is not found so it is impossible to reference to lesson by id {gradesJournalColumnDto.LessonId}");

            lesson = await _lessonsScheduleItemService.GetByIdAsync((long)gradesJournalColumnDto.LessonId,
                lessonSchedule, teacher, 
                new Expression<Func<LessonsScheduleItem, object?>>[]
                {
                    e => e.Subject,
                },
                ct: ct);

            if (lesson is null)
                return Response.NotFound($"Lesson by id {gradesJournalColumnDto.LessonId} is not found");
        }
        else if (gradesJournalColumnDto.HomeworkId is not null)
        {
            homework = await _homeworksService.GetByIdAsync((long)gradesJournalColumnDto.HomeworkId, journal.Group.Id, ct: ct);
            
            if (homework is null)
                return Response.NotFound($"Homework by id {gradesJournalColumnDto.HomeworkId} is not found");
        }
        
        var column = await _gradesJournalColumnService.CreateAsync(gradesJournalColumnDto, journal, lesson, homework, ct);
        return Response.Created(GradesJournalColumnMapper.ToResponse(column));
    }

    [HttpPut("update/{columnId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Update(long institutionId, long journalId, long columnId, UpdatedGradesJournalColumnDto gradesJournalDto, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");
        
        var userId = User.GetIdFromClaims();

        var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);

        if (teacher is null)
            return Response.Forbid();

        var journal = await _gradesJournalService.GetByIdAsync(journalId, institution, teacher, includeProperties: null, ct: ct);

        if (journal is null)
            return Response.NotFound($"Journal by id {journalId} is not found");
        
        var updated = await _gradesJournalColumnService.UpdateAsync(columnId, gradesJournalDto, journal, ct);
        return updated ? Response.Ok() : Response.NotFound($"Grades journal column by id {columnId} is not found");
    }
    
    [HttpDelete("delete/{columnId:long}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Delete(long institutionId, long journalId, long columnId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        
        var teacher = await _gradesJournalService.FindTeacherOfJournalAsync(journalId, userId, ct);

        if (teacher is null)
            return Response.Forbid();
        
        var journal = await _gradesJournalService.GetByIdAsync(journalId, institution, teacher, includeProperties: null,  ct: ct);

        if (journal is null)
            return Response.NotFound($"Journal by id {journalId} is not found");
        
        var deleted = await _gradesJournalColumnService.DeleteAsync(columnId, journal, ct);
        return deleted ? Response.Ok() : Response.NotFound($"Grades journal column by id {columnId} is not found");
    }
}
