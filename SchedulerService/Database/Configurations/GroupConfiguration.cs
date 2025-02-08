using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class GroupConfiguration : IEntityTypeConfiguration<Group>
{
    public void Configure(EntityTypeBuilder<Group> builder)
    {
        builder
            .HasMany(e => e.Students)
            .WithMany(e => e.Groups)
            .UsingEntity(t => t.ToTable("StudentGroups"));

        builder
            .HasOne(e => e.HeadTeacher)
            .WithMany(e => e.HeadTeachingGroups);

        builder
            .HasMany(e => e.SubGroups)
            .WithOne(e => e.ParentGroup);
    }
}