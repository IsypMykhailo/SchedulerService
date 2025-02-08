﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using SchedulerService.Database;

#nullable disable

namespace SchedulerService.Migrations
{
    [DbContext(typeof(SchedulerContext))]
    [Migration("20230124170804_InitMigration")]
    partial class InitMigration
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.1")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.Property<long>("GroupsId")
                        .HasColumnType("bigint");

                    b.Property<string>("StudentsId")
                        .HasColumnType("text");

                    b.HasKey("GroupsId", "StudentsId");

                    b.HasIndex("StudentsId");

                    b.ToTable("StudentGroups", (string)null);
                });

            modelBuilder.Entity("InstitutionUser", b =>
                {
                    b.Property<long>("AdministratingInstitutionsId")
                        .HasColumnType("bigint");

                    b.Property<string>("AdministratorsId")
                        .HasColumnType("text");

                    b.HasKey("AdministratingInstitutionsId", "AdministratorsId");

                    b.HasIndex("AdministratorsId");

                    b.ToTable("AdministratorInstitutions", (string)null);
                });

            modelBuilder.Entity("InstitutionUser1", b =>
                {
                    b.Property<string>("TeachersId")
                        .HasColumnType("text");

                    b.Property<long>("TeachingInstitutionsId")
                        .HasColumnType("bigint");

                    b.HasKey("TeachersId", "TeachingInstitutionsId");

                    b.HasIndex("TeachingInstitutionsId");

                    b.ToTable("TeacherInstitutions", (string)null);
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Attendance", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("AttendanceType")
                        .HasColumnType("integer");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<long>("LessonId")
                        .HasColumnType("bigint");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LessonId");

                    b.HasIndex("StudentId");

                    b.ToTable("Attendances");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.BellsSchedule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("InstitutionId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("ScheduleTermId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("InstitutionId");

                    b.HasIndex("ScheduleTermId");

                    b.ToTable("BellsSchedules");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.BellsScheduleItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<int>("DayOfWeek")
                        .HasColumnType("integer");

                    b.Property<TimeOnly>("LessonEndTime")
                        .HasColumnType("time without time zone");

                    b.Property<short>("LessonIndex")
                        .HasColumnType("smallint");

                    b.Property<TimeOnly>("LessonStartTime")
                        .HasColumnType("time without time zone");

                    b.Property<long>("ScheduleId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("ScheduleId");

                    b.ToTable("BellsScheduleItems");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Grade", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<long>("JournalColumnId")
                        .HasColumnType("bigint");

                    b.Property<short>("Points")
                        .HasColumnType("smallint");

                    b.Property<string>("StudentId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("JournalColumnId");

                    b.HasIndex("StudentId");

                    b.ToTable("Grades");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.GradesJournal", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("GroupId")
                        .HasColumnType("bigint");

                    b.Property<long>("InstitutionId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.Property<long>("SubjectId")
                        .HasColumnType("bigint");

                    b.Property<string>("TeacherId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("GroupId");

                    b.HasIndex("InstitutionId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("TeacherId");

                    b.ToTable("GradesJournals");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.GradesJournalColumn", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("ColumnHeader")
                        .HasColumnType("text");

                    b.Property<DateOnly?>("Date")
                        .HasColumnType("date");

                    b.Property<string>("HomeworkId")
                        .HasColumnType("text");

                    b.Property<long>("JournalId")
                        .HasColumnType("bigint");

                    b.Property<long?>("LessonId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("JournalId");

                    b.HasIndex("LessonId");

                    b.ToTable("GradesJournalColumns");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Group", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("HeadTeacherId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long>("InstitutionId")
                        .HasColumnType("bigint");

                    b.Property<long?>("LessonsScheduleId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<long?>("ParentGroupId")
                        .HasColumnType("bigint");

                    b.HasKey("Id");

                    b.HasIndex("HeadTeacherId");

                    b.HasIndex("InstitutionId");

                    b.HasIndex("LessonsScheduleId")
                        .IsUnique();

                    b.HasIndex("ParentGroupId");

                    b.ToTable("Groups");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Institution", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<string>("CreatorId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("Description")
                        .HasColumnType("text");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.Property<string>("OwnerId")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("CreatorId");

                    b.HasIndex("OwnerId");

                    b.ToTable("Institutions");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.LessonsSchedule", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("BellsScheduleId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("BellsScheduleId");

                    b.ToTable("LessonsSchedules");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.LessonsScheduleItem", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<DateOnly>("Date")
                        .HasColumnType("date");

                    b.Property<string>("HomeworkDescription")
                        .HasColumnType("text");

                    b.Property<int?>("LessonIndex")
                        .HasColumnType("integer");

                    b.Property<long>("LessonsScheduleId")
                        .HasColumnType("bigint");

                    b.Property<long?>("SubGroupId")
                        .HasColumnType("bigint");

                    b.Property<long>("SubjectId")
                        .HasColumnType("bigint");

                    b.Property<string>("TeacherId")
                        .HasColumnType("text");

                    b.Property<long?>("TeachingSubGroupId")
                        .HasColumnType("bigint");

                    b.Property<string>("Theme")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("LessonsScheduleId");

                    b.HasIndex("SubGroupId");

                    b.HasIndex("SubjectId");

                    b.HasIndex("TeacherId");

                    b.ToTable("LessonsScheduleItems");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Subject", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long>("InstitutionId")
                        .HasColumnType("bigint");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.HasIndex("InstitutionId");

                    b.ToTable("Subjects");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Term", b =>
                {
                    b.Property<long>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("bigint");

                    NpgsqlPropertyBuilderExtensions.UseIdentityByDefaultColumn(b.Property<long>("Id"));

                    b.Property<long?>("BellsScheduleId")
                        .HasColumnType("bigint");

                    b.Property<DateTime>("EndOfTerm")
                        .HasColumnType("timestamp with time zone");

                    b.Property<DateTime>("StartOfTerm")
                        .HasColumnType("timestamp with time zone");

                    b.HasKey("Id");

                    b.HasIndex("BellsScheduleId");

                    b.ToTable("Terms");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.User", b =>
                {
                    b.Property<string>("Id")
                        .HasColumnType("text");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("SubjectUser", b =>
                {
                    b.Property<long>("SubjectsId")
                        .HasColumnType("bigint");

                    b.Property<string>("TeachingSubjectsId")
                        .HasColumnType("text");

                    b.HasKey("SubjectsId", "TeachingSubjectsId");

                    b.HasIndex("TeachingSubjectsId");

                    b.ToTable("TeachingSubjects", (string)null);
                });

            modelBuilder.Entity("GroupUser", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.Group", null)
                        .WithMany()
                        .HasForeignKey("GroupsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", null)
                        .WithMany()
                        .HasForeignKey("StudentsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InstitutionUser", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.Institution", null)
                        .WithMany()
                        .HasForeignKey("AdministratingInstitutionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", null)
                        .WithMany()
                        .HasForeignKey("AdministratorsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("InstitutionUser1", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.User", null)
                        .WithMany()
                        .HasForeignKey("TeachersId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.Institution", null)
                        .WithMany()
                        .HasForeignKey("TeachingInstitutionsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Attendance", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.LessonsScheduleItem", "Lesson")
                        .WithMany()
                        .HasForeignKey("LessonId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Lesson");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.BellsSchedule", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.Institution", "Institution")
                        .WithMany("BellsSchedules")
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.Term", "ScheduleTerm")
                        .WithMany()
                        .HasForeignKey("ScheduleTermId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Institution");

                    b.Navigation("ScheduleTerm");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.BellsScheduleItem", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.BellsSchedule", "Schedule")
                        .WithMany("Items")
                        .HasForeignKey("ScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Schedule");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Grade", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.GradesJournalColumn", "JournalColumn")
                        .WithMany("Grades")
                        .HasForeignKey("JournalColumnId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", "Student")
                        .WithMany()
                        .HasForeignKey("StudentId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("JournalColumn");

                    b.Navigation("Student");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.GradesJournal", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.Group", "Group")
                        .WithMany()
                        .HasForeignKey("GroupId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.Institution", "Institution")
                        .WithMany()
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.Subject", "Subject")
                        .WithMany()
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", "Teacher")
                        .WithMany()
                        .HasForeignKey("TeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Group");

                    b.Navigation("Institution");

                    b.Navigation("Subject");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.GradesJournalColumn", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.GradesJournal", "Journal")
                        .WithMany("Columns")
                        .HasForeignKey("JournalId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.LessonsScheduleItem", "Lesson")
                        .WithMany()
                        .HasForeignKey("LessonId");

                    b.Navigation("Journal");

                    b.Navigation("Lesson");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Group", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.User", "HeadTeacher")
                        .WithMany("HeadTeachingGroups")
                        .HasForeignKey("HeadTeacherId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.Institution", "Institution")
                        .WithMany("Groups")
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.LessonsSchedule", "LessonsSchedule")
                        .WithOne("Group")
                        .HasForeignKey("SchedulerService.Domain.Models.Group", "LessonsScheduleId");

                    b.HasOne("SchedulerService.Domain.Models.Group", "ParentGroup")
                        .WithMany("SubGroups")
                        .HasForeignKey("ParentGroupId");

                    b.Navigation("HeadTeacher");

                    b.Navigation("Institution");

                    b.Navigation("LessonsSchedule");

                    b.Navigation("ParentGroup");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Institution", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.User", "Creator")
                        .WithMany("CreatingInstitutions")
                        .HasForeignKey("CreatorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", "Owner")
                        .WithMany("OwningInstitutions")
                        .HasForeignKey("OwnerId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Creator");

                    b.Navigation("Owner");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.LessonsSchedule", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.BellsSchedule", "BellsSchedule")
                        .WithMany("LessonsSchedules")
                        .HasForeignKey("BellsScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("BellsSchedule");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.LessonsScheduleItem", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.LessonsSchedule", "LessonsSchedule")
                        .WithMany("Items")
                        .HasForeignKey("LessonsScheduleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.Group", "SubGroup")
                        .WithMany("SubGroupLessons")
                        .HasForeignKey("SubGroupId");

                    b.HasOne("SchedulerService.Domain.Models.Subject", "Subject")
                        .WithMany("Lessons")
                        .HasForeignKey("SubjectId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", "Teacher")
                        .WithMany("Lessons")
                        .HasForeignKey("TeacherId");

                    b.Navigation("LessonsSchedule");

                    b.Navigation("SubGroup");

                    b.Navigation("Subject");

                    b.Navigation("Teacher");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Subject", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.Institution", "Institution")
                        .WithMany("Subjects")
                        .HasForeignKey("InstitutionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Institution");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Term", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.BellsSchedule", null)
                        .WithMany("Holidays")
                        .HasForeignKey("BellsScheduleId");
                });

            modelBuilder.Entity("SubjectUser", b =>
                {
                    b.HasOne("SchedulerService.Domain.Models.Subject", null)
                        .WithMany()
                        .HasForeignKey("SubjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("SchedulerService.Domain.Models.User", null)
                        .WithMany()
                        .HasForeignKey("TeachingSubjectsId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.BellsSchedule", b =>
                {
                    b.Navigation("Holidays");

                    b.Navigation("Items");

                    b.Navigation("LessonsSchedules");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.GradesJournal", b =>
                {
                    b.Navigation("Columns");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.GradesJournalColumn", b =>
                {
                    b.Navigation("Grades");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Group", b =>
                {
                    b.Navigation("SubGroupLessons");

                    b.Navigation("SubGroups");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Institution", b =>
                {
                    b.Navigation("BellsSchedules");

                    b.Navigation("Groups");

                    b.Navigation("Subjects");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.LessonsSchedule", b =>
                {
                    b.Navigation("Group")
                        .IsRequired();

                    b.Navigation("Items");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.Subject", b =>
                {
                    b.Navigation("Lessons");
                });

            modelBuilder.Entity("SchedulerService.Domain.Models.User", b =>
                {
                    b.Navigation("CreatingInstitutions");

                    b.Navigation("HeadTeachingGroups");

                    b.Navigation("Lessons");

                    b.Navigation("OwningInstitutions");
                });
#pragma warning restore 612, 618
        }
    }
}
