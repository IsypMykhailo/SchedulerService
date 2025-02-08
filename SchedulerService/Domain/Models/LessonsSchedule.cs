using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class LessonsSchedule
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public string? Name { get; set; }

    [Required] public virtual Group Group { get; set; } = default!;
    [Required] public virtual BellsSchedule BellsSchedule { get; set; } = default!;
    public virtual Term? ScheduleTerm { get; set; }
    
    public virtual ICollection<Term> Holidays { get; set; } = default!;
    public virtual ICollection<LessonsScheduleItem> Items { get; set; } = default!;

    public LessonsSchedule()
    {
        Items = new HashSet<LessonsScheduleItem>();
        Holidays = new HashSet<Term>();
    }
}