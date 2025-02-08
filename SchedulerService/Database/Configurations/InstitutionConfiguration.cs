using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SchedulerService.Domain.Models;

namespace SchedulerService.Database.Configurations;

public class InstitutionConfiguration : IEntityTypeConfiguration<Institution>
{
    public void Configure(EntityTypeBuilder<Institution> builder)
    {
        builder
            .HasOne(e => e.Owner)
            .WithMany(e => e.OwningInstitutions);

        builder
            .HasOne(e => e.Creator)
            .WithMany(e => e.CreatingInstitutions);
        
        builder
            .HasMany(e => e.Groups)
            .WithOne(c => c.Institution);

        builder
            .HasMany(e => e.Subjects)
            .WithOne(e => e.Institution);

        builder
            .HasMany(e => e.Administrators)
            .WithMany(e => e.AdministratingInstitutions)
            .UsingEntity(t => t.ToTable("AdministratorInstitutions"));

        builder
            .HasMany(e => e.Teachers)
            .WithMany(e => e.TeachingInstitutions)
            .UsingEntity(t => t.ToTable("TeacherInstitutions"));
    }
}