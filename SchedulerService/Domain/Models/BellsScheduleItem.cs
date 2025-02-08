using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class BellsScheduleItem
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    [Required] public virtual BellsSchedule Schedule { get; set; } = default!;
    [Required] public short LessonIndex { get; set; }
    [Required] public DayOfWeek DayOfWeek { get; set; }
    [Required] public TimeOnly LessonStartTime { get; set; }
    [Required] public TimeOnly LessonEndTime { get; set; }
}