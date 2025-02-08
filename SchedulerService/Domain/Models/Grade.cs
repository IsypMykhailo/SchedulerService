using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class Grade
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public string StudentId { get; set; } = default!;
    public virtual User Student { get; set; } = default!;
    
    public short Points { get; set; }
    public string? Description { get; set; }

    public virtual GradesJournalColumn JournalColumn { get; set; } = default!;
}