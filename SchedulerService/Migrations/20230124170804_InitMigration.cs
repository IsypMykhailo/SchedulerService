using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace SchedulerService.Migrations
{
    /// <inheritdoc />
    public partial class InitMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Institutions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    CreatorId = table.Column<string>(type: "text", nullable: false),
                    OwnerId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Institutions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Institutions_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Institutions_Users_OwnerId",
                        column: x => x.OwnerId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AdministratorInstitutions",
                columns: table => new
                {
                    AdministratingInstitutionsId = table.Column<long>(type: "bigint", nullable: false),
                    AdministratorsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AdministratorInstitutions", x => new { x.AdministratingInstitutionsId, x.AdministratorsId });
                    table.ForeignKey(
                        name: "FK_AdministratorInstitutions_Institutions_AdministratingInstit~",
                        column: x => x.AdministratingInstitutionsId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AdministratorInstitutions_Users_AdministratorsId",
                        column: x => x.AdministratorsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Subjects",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    InstitutionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Subjects", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Subjects_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeacherInstitutions",
                columns: table => new
                {
                    TeachersId = table.Column<string>(type: "text", nullable: false),
                    TeachingInstitutionsId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeacherInstitutions", x => new { x.TeachersId, x.TeachingInstitutionsId });
                    table.ForeignKey(
                        name: "FK_TeacherInstitutions_Institutions_TeachingInstitutionsId",
                        column: x => x.TeachingInstitutionsId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeacherInstitutions_Users_TeachersId",
                        column: x => x.TeachersId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "TeachingSubjects",
                columns: table => new
                {
                    SubjectsId = table.Column<long>(type: "bigint", nullable: false),
                    TeachingSubjectsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TeachingSubjects", x => new { x.SubjectsId, x.TeachingSubjectsId });
                    table.ForeignKey(
                        name: "FK_TeachingSubjects_Subjects_SubjectsId",
                        column: x => x.SubjectsId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TeachingSubjects_Users_TeachingSubjectsId",
                        column: x => x.TeachingSubjectsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Attendances",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LessonId = table.Column<long>(type: "bigint", nullable: false),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    AttendanceType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Attendances", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Attendances_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "BellsScheduleItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    LessonIndex = table.Column<short>(type: "smallint", nullable: false),
                    DayOfWeek = table.Column<int>(type: "integer", nullable: false),
                    LessonStartTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false),
                    LessonEndTime = table.Column<TimeOnly>(type: "time without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BellsScheduleItems", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "BellsSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    InstitutionId = table.Column<long>(type: "bigint", nullable: false),
                    ScheduleTermId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BellsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_BellsSchedules_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonsSchedules",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    BellsScheduleId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonsSchedules", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonsSchedules_BellsSchedules_BellsScheduleId",
                        column: x => x.BellsScheduleId,
                        principalTable: "BellsSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Terms",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StartOfTerm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndOfTerm = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    BellsScheduleId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Terms", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Terms_BellsSchedules_BellsScheduleId",
                        column: x => x.BellsScheduleId,
                        principalTable: "BellsSchedules",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Groups",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    InstitutionId = table.Column<long>(type: "bigint", nullable: false),
                    HeadTeacherId = table.Column<string>(type: "text", nullable: false),
                    LessonsScheduleId = table.Column<long>(type: "bigint", nullable: true),
                    ParentGroupId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Groups", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Groups_Groups_ParentGroupId",
                        column: x => x.ParentGroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Groups_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Groups_LessonsSchedules_LessonsScheduleId",
                        column: x => x.LessonsScheduleId,
                        principalTable: "LessonsSchedules",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Groups_Users_HeadTeacherId",
                        column: x => x.HeadTeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradesJournals",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: true),
                    GroupId = table.Column<long>(type: "bigint", nullable: false),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    TeacherId = table.Column<string>(type: "text", nullable: false),
                    InstitutionId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradesJournals", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradesJournals_Groups_GroupId",
                        column: x => x.GroupId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradesJournals_Institutions_InstitutionId",
                        column: x => x.InstitutionId,
                        principalTable: "Institutions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradesJournals_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradesJournals_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "LessonsScheduleItems",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    SubjectId = table.Column<long>(type: "bigint", nullable: false),
                    LessonsScheduleId = table.Column<long>(type: "bigint", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    TeacherId = table.Column<string>(type: "text", nullable: true),
                    TeachingSubGroupId = table.Column<long>(type: "bigint", nullable: true),
                    SubGroupId = table.Column<long>(type: "bigint", nullable: true),
                    Theme = table.Column<string>(type: "text", nullable: true),
                    HomeworkDescription = table.Column<string>(type: "text", nullable: true),
                    LessonIndex = table.Column<int>(type: "integer", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LessonsScheduleItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LessonsScheduleItems_Groups_SubGroupId",
                        column: x => x.SubGroupId,
                        principalTable: "Groups",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_LessonsScheduleItems_LessonsSchedules_LessonsScheduleId",
                        column: x => x.LessonsScheduleId,
                        principalTable: "LessonsSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonsScheduleItems_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_LessonsScheduleItems_Users_TeacherId",
                        column: x => x.TeacherId,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "StudentGroups",
                columns: table => new
                {
                    GroupsId = table.Column<long>(type: "bigint", nullable: false),
                    StudentsId = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudentGroups", x => new { x.GroupsId, x.StudentsId });
                    table.ForeignKey(
                        name: "FK_StudentGroups_Groups_GroupsId",
                        column: x => x.GroupsId,
                        principalTable: "Groups",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_StudentGroups_Users_StudentsId",
                        column: x => x.StudentsId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "GradesJournalColumns",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    ColumnHeader = table.Column<string>(type: "text", nullable: true),
                    LessonId = table.Column<long>(type: "bigint", nullable: true),
                    HomeworkId = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: true),
                    JournalId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GradesJournalColumns", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GradesJournalColumns_GradesJournals_JournalId",
                        column: x => x.JournalId,
                        principalTable: "GradesJournals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_GradesJournalColumns_LessonsScheduleItems_LessonId",
                        column: x => x.LessonId,
                        principalTable: "LessonsScheduleItems",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Grades",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    StudentId = table.Column<string>(type: "text", nullable: false),
                    Points = table.Column<short>(type: "smallint", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: true),
                    JournalColumnId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Grades", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Grades_GradesJournalColumns_JournalColumnId",
                        column: x => x.JournalColumnId,
                        principalTable: "GradesJournalColumns",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_Grades_Users_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AdministratorInstitutions_AdministratorsId",
                table: "AdministratorInstitutions",
                column: "AdministratorsId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_LessonId",
                table: "Attendances",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_Attendances_StudentId",
                table: "Attendances",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_BellsScheduleItems_ScheduleId",
                table: "BellsScheduleItems",
                column: "ScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_BellsSchedules_InstitutionId",
                table: "BellsSchedules",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_BellsSchedules_ScheduleTermId",
                table: "BellsSchedules",
                column: "ScheduleTermId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_JournalColumnId",
                table: "Grades",
                column: "JournalColumnId");

            migrationBuilder.CreateIndex(
                name: "IX_Grades_StudentId",
                table: "Grades",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_GradesJournalColumns_JournalId",
                table: "GradesJournalColumns",
                column: "JournalId");

            migrationBuilder.CreateIndex(
                name: "IX_GradesJournalColumns_LessonId",
                table: "GradesJournalColumns",
                column: "LessonId");

            migrationBuilder.CreateIndex(
                name: "IX_GradesJournals_GroupId",
                table: "GradesJournals",
                column: "GroupId");

            migrationBuilder.CreateIndex(
                name: "IX_GradesJournals_InstitutionId",
                table: "GradesJournals",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_GradesJournals_SubjectId",
                table: "GradesJournals",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_GradesJournals_TeacherId",
                table: "GradesJournals",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_HeadTeacherId",
                table: "Groups",
                column: "HeadTeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_InstitutionId",
                table: "Groups",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_Groups_LessonsScheduleId",
                table: "Groups",
                column: "LessonsScheduleId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Groups_ParentGroupId",
                table: "Groups",
                column: "ParentGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_CreatorId",
                table: "Institutions",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_Institutions_OwnerId",
                table: "Institutions",
                column: "OwnerId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonsScheduleItems_LessonsScheduleId",
                table: "LessonsScheduleItems",
                column: "LessonsScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonsScheduleItems_SubGroupId",
                table: "LessonsScheduleItems",
                column: "SubGroupId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonsScheduleItems_SubjectId",
                table: "LessonsScheduleItems",
                column: "SubjectId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonsScheduleItems_TeacherId",
                table: "LessonsScheduleItems",
                column: "TeacherId");

            migrationBuilder.CreateIndex(
                name: "IX_LessonsSchedules_BellsScheduleId",
                table: "LessonsSchedules",
                column: "BellsScheduleId");

            migrationBuilder.CreateIndex(
                name: "IX_StudentGroups_StudentsId",
                table: "StudentGroups",
                column: "StudentsId");

            migrationBuilder.CreateIndex(
                name: "IX_Subjects_InstitutionId",
                table: "Subjects",
                column: "InstitutionId");

            migrationBuilder.CreateIndex(
                name: "IX_TeacherInstitutions_TeachingInstitutionsId",
                table: "TeacherInstitutions",
                column: "TeachingInstitutionsId");

            migrationBuilder.CreateIndex(
                name: "IX_TeachingSubjects_TeachingSubjectsId",
                table: "TeachingSubjects",
                column: "TeachingSubjectsId");

            migrationBuilder.CreateIndex(
                name: "IX_Terms_BellsScheduleId",
                table: "Terms",
                column: "BellsScheduleId");

            migrationBuilder.AddForeignKey(
                name: "FK_Attendances_LessonsScheduleItems_LessonId",
                table: "Attendances",
                column: "LessonId",
                principalTable: "LessonsScheduleItems",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BellsScheduleItems_BellsSchedules_ScheduleId",
                table: "BellsScheduleItems",
                column: "ScheduleId",
                principalTable: "BellsSchedules",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_BellsSchedules_Terms_ScheduleTermId",
                table: "BellsSchedules",
                column: "ScheduleTermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BellsSchedules_Institutions_InstitutionId",
                table: "BellsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Terms_BellsSchedules_BellsScheduleId",
                table: "Terms");

            migrationBuilder.DropTable(
                name: "AdministratorInstitutions");

            migrationBuilder.DropTable(
                name: "Attendances");

            migrationBuilder.DropTable(
                name: "BellsScheduleItems");

            migrationBuilder.DropTable(
                name: "Grades");

            migrationBuilder.DropTable(
                name: "StudentGroups");

            migrationBuilder.DropTable(
                name: "TeacherInstitutions");

            migrationBuilder.DropTable(
                name: "TeachingSubjects");

            migrationBuilder.DropTable(
                name: "GradesJournalColumns");

            migrationBuilder.DropTable(
                name: "GradesJournals");

            migrationBuilder.DropTable(
                name: "LessonsScheduleItems");

            migrationBuilder.DropTable(
                name: "Groups");

            migrationBuilder.DropTable(
                name: "Subjects");

            migrationBuilder.DropTable(
                name: "LessonsSchedules");

            migrationBuilder.DropTable(
                name: "Institutions");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "BellsSchedules");

            migrationBuilder.DropTable(
                name: "Terms");
        }
    }
}
