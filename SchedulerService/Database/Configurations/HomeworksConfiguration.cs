using SchedulerService.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace SchedulerService.Database.Configurations;

public class HomeworksConfiguration : IEntityTypeConfiguration<Homeworks>
{
    public void Configure(EntityTypeBuilder<Homeworks> builder)
    {
        builder
            .HasMany(e => e.Items)
            .WithOne(c => c.Homework);

        builder
            .HasOne(e => e.Group)
            .WithMany(e => e.Homeworks);

        builder
            .HasOne(e => e.Creator)
            .WithMany(e => e.Homeworks);

        builder
            .HasMany<GradesJournalColumn>()
            .WithOne(e => e.Homework);
    }
}