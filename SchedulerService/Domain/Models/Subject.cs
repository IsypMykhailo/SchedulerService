using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class Subject
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] public string Name { get; set; } = default!;
    [Required] public virtual Institution Institution { get; set; } = default!;

    public virtual ICollection<LessonsScheduleItem> Lessons { get; set; } = default!;
    public virtual ICollection<User> TeachingSubjects { get; set; } = default!;

    public Subject()
    {
        Lessons = new HashSet<LessonsScheduleItem>();
        TeachingSubjects = new HashSet<User>();
    }
}