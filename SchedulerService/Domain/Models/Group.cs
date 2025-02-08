using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class Group
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Name { get; set; } = default!;
    public virtual Institution Institution { get; set; } = default!;
    
    public string HeadTeacherId { get; set; } = default!;
    public virtual User HeadTeacher { get; set; } = default!;
    public virtual LessonsSchedule? LessonsSchedule { get; set; }
    public long? LessonsScheduleId { get; set; }
    
    public long? ParentGroupId { get; set; }
    public virtual Group? ParentGroup { get; set; }

    public virtual ICollection<User> Students { get; set; }
    public virtual ICollection<Group> SubGroups { get; set; }
    public virtual ICollection<LessonsScheduleItem> SubGroupLessons { get; set; }
    public virtual ICollection<Homeworks> Homeworks { get; set; }

    public Group()
    {
        Students = new HashSet<User>();
        SubGroups = new HashSet<Group>();
        SubGroupLessons = new HashSet<LessonsScheduleItem>();
        Homeworks = new HashSet<Homeworks>();
    }
}