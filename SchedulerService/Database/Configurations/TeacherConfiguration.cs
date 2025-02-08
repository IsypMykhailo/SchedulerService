using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class TeacherConfiguration : IEntityTypeConfiguration<User>
{
    public void Configure(EntityTypeBuilder<User> builder)
    {
        builder
            .HasMany(e => e.Subjects)
            .WithMany(e => e.TeachingSubjects)
            .UsingEntity(t => t.ToTable("TeachingSubjects"));
    }
}