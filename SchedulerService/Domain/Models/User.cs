using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class User
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.None)]
    public string Id { get; set; } = default!;

    public virtual ICollection<Group> Groups { get; set; }

    public virtual ICollection<Institution> CreatingInstitutions { get; set; }
    public virtual ICollection<Institution> OwningInstitutions { get; set; }
    
    public virtual ICollection<Institution> AdministratingInstitutions { get; set; }
    public virtual ICollection<Institution> TeachingInstitutions { get; set; }
    public virtual ICollection<Group> HeadTeachingGroups { get; set; }
    public virtual ICollection<Subject> Subjects { get; set; }
    public virtual ICollection<LessonsScheduleItem> Lessons { get; set; }

    public virtual ICollection<Homeworks> Homeworks { get; set; }
    public virtual ICollection<HomeworkItems> HomeworkItems { get; set; }

    public User()
    {
        Groups = new HashSet<Group>();
        CreatingInstitutions = new HashSet<Institution>();
        OwningInstitutions = new HashSet<Institution>();
        AdministratingInstitutions = new HashSet<Institution>();
        TeachingInstitutions = new HashSet<Institution>();
        HeadTeachingGroups = new HashSet<Group>();
        Lessons = new HashSet<LessonsScheduleItem>();
        Subjects = new HashSet<Subject>();
        Homeworks = new HashSet<Homeworks>();
        HomeworkItems = new HashSet<HomeworkItems>();
    }
}