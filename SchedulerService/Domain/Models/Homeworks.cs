using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class Homeworks
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public string Title { get; set; } = default!;

    public string CreatorId { get; set; } = default!;
    [Required] public virtual User Creator { get; set; } = default!;

    public long GroupId { get; set; } = default!;
    [Required] public virtual Group Group { get; set; } = default!;

    public List<string> HomeworkPath { get; set; } = default!;
    
    public string? Description { get; set; }
    
    [Required] public DateTime UploadedDate { get; set; }
    
    public DateTime DueDate { get; set; }

    public virtual ICollection<HomeworkItems> Items { get; set; } = default!;

    public Homeworks()
    {
        Items = new HashSet<HomeworkItems>();
        HomeworkPath = new List<string>();
    }
}