using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchedulerService.Migrations
{
    /// <inheritdoc />
    public partial class HomeworkItemsUpdate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "BeforeDueDate",
                table: "HomeworkItems",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BeforeDueDate",
                table: "HomeworkItems");
        }
    }
}
