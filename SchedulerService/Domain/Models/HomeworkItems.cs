using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace SchedulerService.Domain.Models;

public class HomeworkItems
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }

    public long HomeworkId { get; set; } = default!;

    [Required] public virtual Homeworks Homework { get; set; } = default!;

    public string StudentId { get; set; } = default!;
    [Required] public virtual User Student { get; set; } = default!;

    public List<string> CompletedHomework { get; set; } = default!;
    
    [Required] public DateTime HomeworkUploaded { get; set; }
    
    public DateTime? HomeworkUpdated { get; set; }

    public string? Comment { get; set; }
    
    public bool BeforeDueDate { get; set; }

    public HomeworkItems()
    {
        CompletedHomework = new List<string>();
    }
}