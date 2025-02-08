using SchedulerService.Domain;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Models;

namespace SchedulerService.Services.Interfaces;

public interface IAttendanceService
{
    Task<IEnumerable<Attendance>> GetAsync(LessonsScheduleItem lesson, CancellationToken ct = default);
    Task<IEnumerable<Attendance>> GetAsStudentAsync(string studentId, DateOnly startDate, DateOnly endDate, CancellationToken ct);
    Task<IEnumerable<Attendance>> GetAsTeacherAsync(string teacherId, DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
    Task<IEnumerable<Attendance>> GetInstitutionAttendanceAsync(Institution institution, DateOnly startDate, DateOnly endDate, CancellationToken ct = default);
    Task<Attendance> CreateOrUpdateAsync(LessonsScheduleItem lesson, AttendanceDto attendanceDto, User student, CancellationToken ct);
    Task<IEnumerable<Attendance>> CreateOrUpdateRangeAsync(LessonsScheduleItem lesson, Group group, AttendanceType attendanceType, CancellationToken ct);
}