using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulerService.Migrations
{
    /// <inheritdoc />
    public partial class SubGroupId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TeachingSubGroupId",
                table: "LessonsScheduleItems");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "TeachingSubGroupId",
                table: "LessonsScheduleItems",
                type: "bigint",
                nullable: true);
        }
    }
}
