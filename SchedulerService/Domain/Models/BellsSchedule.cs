using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class BellsSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] public string Name { get; set; } = default!;
    [Required] public virtual Institution Institution { get; set; } = default!;

    public virtual ICollection<LessonsSchedule> LessonsSchedules { get; set; } = default!;
    public virtual ICollection<BellsScheduleItem> Items { get; set; } = default!;

    public BellsSchedule()
    {
        LessonsSchedules = new HashSet<LessonsSchedule>();
        Items = new HashSet<BellsScheduleItem>();
    }
}