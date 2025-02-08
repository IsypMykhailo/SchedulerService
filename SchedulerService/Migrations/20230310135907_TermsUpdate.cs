using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulerService.Migrations
{
    /// <inheritdoc />
    public partial class TermsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BellsSchedules_Terms_ScheduleTermId",
                table: "BellsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Terms_BellsSchedules_BellsScheduleId",
                table: "Terms");

            migrationBuilder.DropIndex(
                name: "IX_BellsSchedules_ScheduleTermId",
                table: "BellsSchedules");

            migrationBuilder.DropColumn(
                name: "ScheduleTermId",
                table: "BellsSchedules");

            migrationBuilder.RenameColumn(
                name: "BellsScheduleId",
                table: "Terms",
                newName: "LessonsScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Terms_BellsScheduleId",
                table: "Terms",
                newName: "IX_Terms_LessonsScheduleId");

            migrationBuilder.AddColumn<long>(
                name: "ScheduleTermId",
                table: "LessonsSchedules",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LessonsSchedules_ScheduleTermId",
                table: "LessonsSchedules",
                column: "ScheduleTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_LessonsSchedules_Terms_ScheduleTermId",
                table: "LessonsSchedules",
                column: "ScheduleTermId",
                principalTable: "Terms",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Terms_LessonsSchedules_LessonsScheduleId",
                table: "Terms",
                column: "LessonsScheduleId",
                principalTable: "LessonsSchedules",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LessonsSchedules_Terms_ScheduleTermId",
                table: "LessonsSchedules");

            migrationBuilder.DropForeignKey(
                name: "FK_Terms_LessonsSchedules_LessonsScheduleId",
                table: "Terms");

            migrationBuilder.DropIndex(
                name: "IX_LessonsSchedules_ScheduleTermId",
                table: "LessonsSchedules");

            migrationBuilder.DropColumn(
                name: "ScheduleTermId",
                table: "LessonsSchedules");

            migrationBuilder.RenameColumn(
                name: "LessonsScheduleId",
                table: "Terms",
                newName: "BellsScheduleId");

            migrationBuilder.RenameIndex(
                name: "IX_Terms_LessonsScheduleId",
                table: "Terms",
                newName: "IX_Terms_BellsScheduleId");

            migrationBuilder.AddColumn<long>(
                name: "ScheduleTermId",
                table: "BellsSchedules",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_BellsSchedules_ScheduleTermId",
                table: "BellsSchedules",
                column: "ScheduleTermId");

            migrationBuilder.AddForeignKey(
                name: "FK_BellsSchedules_Terms_ScheduleTermId",
                table: "BellsSchedules",
                column: "ScheduleTermId",
                principalTable: "Terms",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Terms_BellsSchedules_BellsScheduleId",
                table: "Terms",
                column: "BellsScheduleId",
                principalTable: "BellsSchedules",
                principalColumn: "Id");
        }
    }
}
