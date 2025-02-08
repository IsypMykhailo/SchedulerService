using System.Linq.Expressions;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Notifications;
using SchedulerService.Domain.Response;
using SchedulerService.Services;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/journals/{journalId:long}/columns/{columnId:long}/")]
public class GradeController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly IAdministratorService _administratorService;
    private readonly ITeacherService _teacherService;
    private readonly IGradesJournalService _gradesJournalService;
    private readonly IGradesJournalColumnService _gradesJournalColumnService;
    private readonly IGradeService _gradeService;
    private readonly INotificationPublisher _notificationPublisher;

    public GradeController(IInstitutionService institutionService,
        ITeacherService teacherService, 
        IGradesJournalService gradesJournalService, 
        IGradesJournalColumnService gradesJournalColumnService, 
        IGradeService gradeService, 
        IAdministratorService administratorService, 
        INotificationPublisher notificationPublisher)
    {
        _institutionService = institutionService;
        _teacherService = teacherService;
        _gradesJournalService = gradesJournalService;
        _gradesJournalColumnService = gradesJournalColumnService;
        _gradeService = gradeService;
        _administratorService = administratorService;
        _notificationPublisher = notificationPublisher;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(IEnumerable<GradeResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, long journalId, long columnId, CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();

        GradesJournal? journal;
        
        if (await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
        {
            journal = await _gradesJournalService.GetByIdAsync(journalId, institution, ct: ct);
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
        
        var column = await _gradesJournalColumnService.GetByIdAsync(columnId, journal, ct: ct);
        
        if (column is null)
            return Response.NotFound($"Column by id {columnId} is not found");
        
        var grades = await _gradeService.GetAllAsync(column, ct);
        return Response.Ok(GradeMapper.ToResponses(grades));
    }

    [HttpPost("mark-student")]
    [ProducesResponseType(typeof(GradeResponse), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Mark(long institutionId, long journalId, long columnId, GradeDto gradeDto,
        CancellationToken ct)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
            var teacher = await _teacherService.FindTeacherAsync(institution, userId, ct);
    
        if (teacher is null)
            return Response.Forbid();

        var journal = await _gradesJournalService.GetByIdAsync(journalId, institution, teacher, false,
            includeProperties: new Expression<Func<GradesJournal, object?>>[]
            {
                e => e.Subject
            }, ct: ct);
        
        if (journal is null)
            return Response.NotFound($"Journal by id {journalId} is not found");
        
        var column = await _gradesJournalColumnService.GetByIdAsync(columnId, journal, new Expression<Func<GradesJournalColumn, object?>>[]
        {
            e => e.Homework,
            e => e.Homework!.Group,
            e => e.Lesson,
            e => e.Lesson!.Subject,
            e => e.Lesson!.SubGroup,
        }, ct: ct);
        
        if (column is null)
            return Response.NotFound($"Column by id {columnId} is not found");

        var marked = await _gradeService.MarkStudent(gradeDto, column, ct);

        if (gradeDto.Points is not null && marked)
        {
            await _notificationPublisher.PublishAsync(new GradeNotification()
            {
                State = new
                {
                    Grade = gradeDto, 
                    Column = GradesJournalColumnMapper.ToResponse(column),
                    TeacherId = journal.TeacherId,
                    Subject = SubjectMapper.ToResponse(journal.Subject),
                },
                ReceiverId = gradeDto.StudentId,
                InstitutionId = institutionId,
            });
        }
        
        return marked == false ? Response.NotFound($"Student by id {gradeDto.StudentId} is not found") : Response.Ok();
    }
}
