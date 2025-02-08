using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulerService.Migrations
{
    /// <inheritdoc />
    public partial class HomeworkIdLong : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            /*migrationBuilder.AlterColumn<long>(
                name: "HomeworkId",
                table: "GradesJournalColumns",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "text",
                oldNullable: true);*/

            migrationBuilder.Sql(
                "ALTER TABLE \"GradesJournalColumns\" ALTER COLUMN \"HomeworkId\" TYPE bigint USING (\"HomeworkId\"::bigint);");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "HomeworkId",
                table: "GradesJournalColumns",
                type: "text",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);
        }
    }
}
