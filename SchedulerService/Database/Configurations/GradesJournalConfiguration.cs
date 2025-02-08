using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class GradesJournalConfiguration : IEntityTypeConfiguration<GradesJournal>
{
    public void Configure(EntityTypeBuilder<GradesJournal> builder)
    {
        builder
            .HasOne(e => e.Group)
            .WithMany();
        
        builder
            .HasOne(e => e.Subject)
            .WithMany();
        
        builder
            .HasOne(e => e.Teacher)
            .WithMany();
        
        builder
            .HasMany(e => e.Columns)
            .WithOne(e => e.Journal);
        
        builder
            .HasOne(e => e.Institution)
            .WithMany();
    }
}