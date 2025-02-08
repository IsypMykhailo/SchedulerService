using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class Attendance
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public virtual LessonsScheduleItem Lesson { get; set; } = default!;

    public string StudentId { get; set; } = default!;
    public virtual User Student { get; set; } = default!;
    
    public string? Description { get; set; }
    
    public AttendanceType AttendanceType { get; set; }
}

