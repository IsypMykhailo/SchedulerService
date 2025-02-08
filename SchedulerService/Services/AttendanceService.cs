using System.Linq.Expressions;
using SchedulerService.Domain;
using SchedulerService.Domain.Dto;
using SchedulerService.Domain.Mappers;
using SchedulerService.Domain.Models;
using SchedulerService.Repositories;
using SchedulerService.Services.Interfaces;

namespace SchedulerService.Services;

public class AttendanceService : IAttendanceService
{
    private readonly ICrudRepository<Attendance> _attendanceRepository;
    private readonly ICrudRepository<User> _userRepository;

    public AttendanceService(ICrudRepository<Attendance> attendanceRepository, ICrudRepository<User> userRepository)
    {
        _attendanceRepository = attendanceRepository;
        _userRepository = userRepository;
    }

    public async Task<IEnumerable<Attendance>> GetAsync(LessonsScheduleItem lesson, CancellationToken ct = default)
    {
        return await _attendanceRepository.GetAsync(e => e.Lesson.Id == lesson.Id, ct: ct);
    }

    public async Task<IEnumerable<Attendance>> GetAsStudentAsync(string studentId, DateOnly startDate, DateOnly endDate, CancellationToken ct)
    {
        return await _attendanceRepository.GetAsync(
            e => e.StudentId == studentId && e.Lesson.Date > startDate &&
                 e.Lesson.Date < endDate, new Expression<Func<Attendance, object?>>[]
            {
                e => e.Lesson
            }, ct: ct);
    }

    public async Task<IEnumerable<Attendance>> GetAsTeacherAsync(string teacherId, DateOnly startDate, DateOnly endDate, CancellationToken ct = default)
    {
        return await _attendanceRepository.GetAsync(
            e => e.Lesson.TeacherId == teacherId && e.Lesson.Date > startDate &&
                 e.Lesson.Date < endDate, new Expression<Func<Attendance, object?>>[]
            {
                e => e.Lesson
            }, ct: ct);
    }

    public async Task<IEnumerable<Attendance>> GetInstitutionAttendanceAsync(Institution institution, DateOnly startDate, DateOnly endDate,
        CancellationToken ct = default)
    {
        return await _attendanceRepository.GetAsync(
            e => e.Lesson.LessonsSchedule.Group.Institution.Id == institution.Id && e.Lesson.Date > startDate &&
                 e.Lesson.Date < endDate, new Expression<Func<Attendance, object?>>[]
            {
                e => e.Lesson
            }, ct: ct);
    }

    public async Task<Attendance> CreateOrUpdateAsync(LessonsScheduleItem lesson, AttendanceDto attendanceDto, User student, CancellationToken ct)
    {
        var attendance = (await _attendanceRepository.GetAsync(
            e => e.Lesson.Id == lesson.Id && e.Student.Id == student.Id, ct: ct))
            .FirstOrDefault();

        if (attendance is not null)
        {
            attendance = AttendanceMapper.ToEntity(attendanceDto, lesson, attendance.Id);
            await _attendanceRepository.UpdateAsync(attendance.Id, attendance, ct);
            await _attendanceRepository.SaveAsync(ct);
            return attendance;
        }
        
        attendance = AttendanceMapper.ToEntity(attendanceDto, lesson);
        await _attendanceRepository.CreateAsync(attendance, ct);
        await _attendanceRepository.SaveAsync(ct);
        return attendance;
    }

    public async Task<IEnumerable<Attendance>> CreateOrUpdateRangeAsync(LessonsScheduleItem lesson, Group group, AttendanceType attendanceType, CancellationToken ct)
    {
        var students = await _userRepository.GetAsync(e => e.Groups.Contains(group), ct: ct);

        var existingAttendances = new List<Attendance>(await _attendanceRepository.GetAsync(
            e => e.Lesson.Id == lesson.Id, 
            new Expression<Func<Attendance, object?>>[]
            {
                e => e.Student
            }, ct: ct));
        var attendances = new List<Attendance>();

        foreach (var student in students)
        {
            var index = existingAttendances.FindIndex(attendance => attendance.StudentId == student.Id);
            
            if (index == -1)
            {
                attendances.Add(new Attendance
                {
                    Student = student,
                    StudentId = student.Id,
                    Lesson = lesson,
                    AttendanceType = attendanceType,
                });
            }
            else
            {
                existingAttendances[index].AttendanceType = attendanceType;
            }
        }

        await _attendanceRepository.CreateRangeAsync(attendances, ct);
        await _attendanceRepository.SaveAsync(ct);
        return existingAttendances.Concat(attendances);
    }
}