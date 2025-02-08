using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class GradesJournal
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public string? Name { get; set; }

    public virtual Group Group { get; set; } = default!;
    public virtual Subject Subject { get; set; } = default!;
    
    public string TeacherId { get; set; } = default!;
    public virtual User Teacher { get; set; } = default!;

    public virtual Institution Institution { get; set; } = default!;
    
    public virtual ICollection<GradesJournalColumn> Columns { get; set; }

    public GradesJournal()
    {
        Columns = new HashSet<GradesJournalColumn>();
    }
}