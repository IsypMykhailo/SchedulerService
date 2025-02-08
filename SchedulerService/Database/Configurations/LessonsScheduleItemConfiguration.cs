using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class LessonsScheduleItemConfiguration : IEntityTypeConfiguration<LessonsScheduleItem>
{
    public void Configure(EntityTypeBuilder<LessonsScheduleItem> builder)
    {
        builder
            .HasOne(e => e.Subject)
            .WithMany(e => e.Lessons);

        builder
            .HasOne(e => e.Teacher)
            .WithMany(e => e.Lessons);
        
        builder
            .HasOne(e => e.SubGroup)
            .WithMany(e => e.SubGroupLessons);
    }
}