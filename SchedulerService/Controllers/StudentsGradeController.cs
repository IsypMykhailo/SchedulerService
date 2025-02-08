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
[Route("api/v1/institutions/{institutionId:long}/grades")]
public class StudentsGradeController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly IGroupStudentService _groupStudentService;
    private readonly IGradeService _gradeService;

    public StudentsGradeController(IInstitutionService institutionService,
        IGradeService gradeService, 
        IGroupStudentService groupStudentService)
    {
        _institutionService = institutionService;
        _gradeService = gradeService;
        _groupStudentService = groupStudentService;
    }

    [HttpGet("all")]
    [ProducesResponseType(typeof(List<KeyValuePair<SubjectResponse, List<StudentGradeResponse>>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, CancellationToken ct = default)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        var student = await _groupStudentService.FindStudentAsync(institution, userId, ct);

        if (student is null)
            return Response.Forbid();
        
        var grades = await _gradeService.GetAllAsync(institution, student, ct);
        var gradesGroupedBySubject = new List<KeyValuePair<SubjectResponse, List<StudentGradeResponse>>>();
        
        foreach (var grade in OrderGrades(grades))
        {
            var subject = grade.JournalColumn.Journal.Subject;
            
            if (!gradesGroupedBySubject.Exists(e => e.Key.Id == subject.Id))
                gradesGroupedBySubject.Add(new KeyValuePair<SubjectResponse, List<StudentGradeResponse>>(
                    SubjectMapper.ToResponse(subject), new List<StudentGradeResponse>()));
            
            var kvp = gradesGroupedBySubject.FirstOrDefault(e => e.Key.Id == subject.Id);
            kvp.Value.Add(StudentGradeMapper.ToResponse(grade));
        }
        
        return Response.Ok(gradesGroupedBySubject);
    }

    [HttpGet("last")]
    [ProducesResponseType(typeof(List<KeyValuePair<SubjectResponse, StudentGradeResponse>>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Get(long institutionId, [FromQuery] int numberOfGrades = 5, CancellationToken ct = default)
    {
        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        var student = await _groupStudentService.FindStudentAsync(institution, userId, ct);

        if (student is null)
            return Response.Forbid();
        
        var grades = await _gradeService.GetAllAsync(institution, student, ct);
        var gradesArr = grades as Grade[] ?? grades.ToArray();
        var studentGrades = new List<KeyValuePair<SubjectResponse, StudentGradeResponse>>(gradesArr.Length);

        foreach (var grade in OrderGrades(gradesArr).Reverse())
        {
            if (studentGrades.Count < numberOfGrades)
                studentGrades.Add(new KeyValuePair<SubjectResponse, StudentGradeResponse>(
                    SubjectMapper.ToResponse(grade.JournalColumn.Journal.Subject),StudentGradeMapper.ToResponse(grade)));
            else break;
        }

        return Response.Ok(studentGrades);
    }

    private IOrderedEnumerable<Grade> OrderGrades(IEnumerable<Grade> grades)
    {
        return grades.OrderBy(e =>
        {
            if (e.JournalColumn.Lesson != null) return e.JournalColumn.Lesson.Date;
            if (e.JournalColumn.Homework != null) return DateOnly.FromDateTime(e.JournalColumn.Homework.UploadedDate);
            return e.JournalColumn.Date!.Value;
        });
    }
}
