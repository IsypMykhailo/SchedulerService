using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class HomeworkItemsConfiguration : IEntityTypeConfiguration<HomeworkItems>
{
    public void Configure(EntityTypeBuilder<HomeworkItems> builder)
    {
        builder
            .HasOne(e => e.Student)
            .WithMany(e => e.HomeworkItems);
    }
}