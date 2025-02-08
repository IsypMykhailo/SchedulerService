using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchedulerService.Domain;
using SchedulerService.Domain.Extensions;
using SchedulerService.Domain.Models;
using SchedulerService.Domain.Response;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Controllers.Statistics;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/v1/institutions/{institutionId:long}/attendance/statistics")]
public class AttendanceStatisticsController : ControllerBase
{
    private readonly IInstitutionService _institutionService;
    private readonly ITeacherService _teacherService;
    private readonly IAttendanceService _attendanceService;
    private readonly IGroupStudentService _groupStudentService;
    private readonly IAdministratorService _administratorService;

    public AttendanceStatisticsController(IInstitutionService institutionService,
        ILessonsScheduleItemService lessonsService,
        ITeacherService teacherService,
        IAttendanceService attendanceService, 
        IGroupStudentService groupStudentService, 
        IAdministratorService administratorService)
    {
        _institutionService = institutionService;
        _teacherService = teacherService;
        _attendanceService = attendanceService;
        _groupStudentService = groupStudentService;
        _administratorService = administratorService;
    }

    [HttpGet]
    [ProducesResponseType(typeof(IEnumerable<AttendanceStatisticsResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status403Forbidden)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ApiResponse> Statistics(long institutionId, 
        [FromQuery] DateOnly? endDate, [FromQuery] short months = 6, CancellationToken ct = default)
    {
        if (endDate is null)
        {
            ModelState.AddModelError(nameof(endDate), "EndDate is required.");
            return Response.ValidationFailed(ModelState);
        }

        var institution = await _institutionService.GetByIdAsync(institutionId, ct: ct);

        if (institution is null)
            return Response.NotFound($"Institution by id {institutionId} is not found");

        var userId = User.GetIdFromClaims();
        List<Attendance> attendances;

        var startDate = endDate.Value.AddMonths(-months);
        
        if (await _groupStudentService.FindStudentAsync(institution, userId, ct) is not null)
        {
            attendances = (await _attendanceService.GetAsStudentAsync(userId, startDate, endDate.Value, ct)).ToList();
        }
        else if (await _teacherService.FindTeacherAsync(institution, userId, ct) is not null)
        {
            attendances = (await _attendanceService.GetAsTeacherAsync(userId, startDate, endDate.Value, ct)).ToList();
        }
        else if (await _administratorService.IsUserAdminOrOwnerAsync(institution, userId, ct))
        {
            attendances = (await _attendanceService.GetInstitutionAttendanceAsync(institution, startDate, endDate.Value, ct)).ToList();
        }
        else
        {
            return Response.Forbid();
        }

        List<AttendanceStatisticsResponse> responses = new(months);
        var attendancesThisMonths = new List<Attendance>();
        
        for (var i = 0; i < months; i++)
        {
            startDate = startDate.AddMonths(1);
            attendancesThisMonths.Clear();
            
            foreach (var attendance in attendances)
            {
                if (attendance.Lesson.Date.Year == startDate.Year && attendance.Lesson.Date.Month == startDate.Month)
                    attendancesThisMonths.Add(attendance);
            }

            var count = attendancesThisMonths.Count;
            var isAbsentCount = attendancesThisMonths.Count(e => e.AttendanceType == AttendanceType.Absent);
            var isPresentCount = count - isAbsentCount;

            if (count == 0 || isPresentCount == 0)
            {
                responses.Add(new ()
                {
                    Month = startDate.Month,
                    Year = startDate.Year,
                    Percentage = 0
                });
                continue;
            }
            
            double attendancePercentage = (double)isPresentCount / count * 100.0;
            
            responses.Add(new ()
            {
                Month = startDate.Month,
                Year = startDate.Year,
                Percentage = attendancePercentage
            });
            
            foreach (var attendance in attendancesThisMonths)
            {
                attendances.Remove(attendance);
            }
        }
        
        return Response.Ok(responses);
    }
}