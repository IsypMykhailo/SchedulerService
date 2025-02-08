using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class Institution
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    [Required] public string Name { get; set; } = default!;

    public string CreatorId { get; set; } = default!;
    public string OwnerId { get; set; } = default!;
    [Required] public virtual User Creator { get; set; } = default!;
    [Required] public virtual User Owner { get; set; } = default!;
    public string? Description { get; set; }

    public virtual ICollection<User> Administrators { get; set; } = default!;
    public virtual ICollection<User> Teachers { get; set; } = default!;
    public virtual ICollection<BellsSchedule> BellsSchedules { get; set; } = default!;
    public virtual ICollection<Subject> Subjects { get; set; } = default!;
    public virtual ICollection<Group> Groups { get; set; } = default!;

    public Institution()
    {
        Administrators = new HashSet<User>();
        Teachers = new HashSet<User>();
        BellsSchedules = new HashSet<BellsSchedule>();
        Subjects = new HashSet<Subject>();
        Groups = new HashSet<Group>();
    }
}