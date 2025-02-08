using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class BellScheduleConfiguration : IEntityTypeConfiguration<BellsSchedule>
{
    public void Configure(EntityTypeBuilder<BellsSchedule> builder)
    {
        builder
            .HasMany(e => e.Items)
            .WithOne(c => c.Schedule);

        builder
            .HasOne(e => e.Institution)
            .WithMany(e => e.BellsSchedules);
    }
}