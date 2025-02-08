using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SchedulerService.Domain.Models;

public class GradesJournalColumn
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public long Id { get; set; }
    
    public string? ColumnHeader { get; set; }
    public virtual LessonsScheduleItem? Lesson { get; set; }
    public Homeworks? Homework { get; set; }
    public DateOnly? Date { get; set; }

    public virtual GradesJournal Journal { get; set; } = default!;
    public virtual ICollection<Grade> Grades { get; set; }

    public GradesJournalColumn()
    {
        Grades = new HashSet<Grade>();
    }
}