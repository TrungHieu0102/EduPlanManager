using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlanManager.Migrations
{
    /// <inheritdoc />
    public partial class EditSubjectTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Semester",
                table: "Subjects");

            migrationBuilder.DropColumn(
                name: "Year",
                table: "Subjects");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Semester",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Year",
                table: "Subjects",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
