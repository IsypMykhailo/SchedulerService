using System.Reflection;
using Microsoft.EntityFrameworkCore;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database;

public class SchedulerContext : DbContext
{
    public SchedulerContext(DbContextOptions<SchedulerContext> options) : base(options) { }

    public DbSet<Institution> Institutions { get; set; } = null!;
    public DbSet<BellsSchedule> BellsSchedules { get; set; } = null!;
    public DbSet<BellsScheduleItem> BellsScheduleItems { get; set; } = null!;
    public DbSet<User> Users { get; set; } = null!;
    public DbSet<Subject> Subjects { get; set; } = null!;
    public DbSet<Group> Groups { get; set; } = null!;
    public DbSet<Term> Terms { get; set; } = null!;
    public DbSet<LessonsSchedule> LessonsSchedules { get; set; } = null!;
    public DbSet<LessonsScheduleItem> LessonsScheduleItems { get; set; } = null!;
    public DbSet<GradesJournal> GradesJournals { get; set; } = null!;
    public DbSet<GradesJournalColumn> GradesJournalColumns { get; set; } = null!;
    public DbSet<Grade> Grades { get; set; } = null!;
    public DbSet<Attendance> Attendances { get; set; } = null!;
    public DbSet<Homeworks> Homeworks { get; set; } = null!;
    public DbSet<HomeworkItems> HomeworkItems { get; set; } = null!;

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(Assembly.GetExecutingAssembly());
        
        modelBuilder
            .Entity<GradesJournalColumn>()
            .HasMany(e => e.Grades)
            .WithOne(e => e.JournalColumn);

        modelBuilder
            .Entity<GradesJournalColumn>()
            .HasOne(e => e.Lesson)
            .WithMany();

        modelBuilder
            .Entity<Grade>()
            .HasOne(e => e.Student)
            .WithMany();

        modelBuilder
            .Entity<Attendance>()
            .HasOne(e => e.Lesson)
            .WithMany();

        modelBuilder
            .Entity<Attendance>()
            .HasOne(e => e.Student)
            .WithMany();
    }
}