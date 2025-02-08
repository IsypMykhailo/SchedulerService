using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class LessonsScheduleItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long SubjectId { get; set; }
    public virtual Subject Subject { get; set; } = default!;
    public virtual LessonsSchedule LessonsSchedule { get; set; } = default!;
    public virtual DateOnly Date { get; set; }
    
    public string? TeacherId { get; set; }
    public virtual User? Teacher { get; set; }
    
    public long? SubGroupId { get; set; }
    public virtual Group? SubGroup { get; set; }
    
    public string? Theme { get; set; }
    public string? HomeworkDescription { get; set; }
    public int? LessonIndex { get; set; }
}