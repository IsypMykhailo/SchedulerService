using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class LessonsScheduleConfiguration : IEntityTypeConfiguration<LessonsSchedule>
{
    public void Configure(EntityTypeBuilder<LessonsSchedule> builder)
    {
        builder
            .HasMany(e => e.Items)
            .WithOne(e => e.LessonsSchedule);

        builder
            .HasOne(e => e.BellsSchedule)
            .WithMany(e => e.LessonsSchedules);

        builder
            .HasOne(e => e.Group)
            .WithOne(e => e.LessonsSchedule)
            .HasForeignKey<Group>(e => e.LessonsScheduleId);
        
        builder
            .HasMany(e => e.Holidays)
            .WithOne();

        builder
            .HasOne(e => e.ScheduleTerm)
            .WithMany();
    }
}