using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlanManager.Migrations
{
    /// <inheritdoc />
    public partial class UpdateClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "StudentCount",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TeacherCount",
                table: "Class",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentCount",
                table: "Class");

            migrationBuilder.DropColumn(
                name: "TeacherCount",
                table: "Class");
        }
    }
}
