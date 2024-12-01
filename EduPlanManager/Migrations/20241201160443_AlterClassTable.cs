using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlanManager.Migrations
{
    /// <inheritdoc />
    public partial class AlterClassTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSubjects_Class_ClassesId",
                table: "ClassSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClasses_Class_ClassesId",
                table: "UserClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Class",
                table: "Class");

            migrationBuilder.RenameTable(
                name: "Class",
                newName: "Classes");

            migrationBuilder.AddColumn<string>(
                name: "Code",
                table: "Classes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Classes",
                table: "Classes",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSubjects_Classes_ClassesId",
                table: "ClassSubjects",
                column: "ClassesId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClasses_Classes_ClassesId",
                table: "UserClasses",
                column: "ClassesId",
                principalTable: "Classes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ClassSubjects_Classes_ClassesId",
                table: "ClassSubjects");

            migrationBuilder.DropForeignKey(
                name: "FK_UserClasses_Classes_ClassesId",
                table: "UserClasses");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Classes",
                table: "Classes");

            migrationBuilder.DropColumn(
                name: "Code",
                table: "Classes");

            migrationBuilder.RenameTable(
                name: "Classes",
                newName: "Class");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Class",
                table: "Class",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_ClassSubjects_Class_ClassesId",
                table: "ClassSubjects",
                column: "ClassesId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserClasses_Class_ClassesId",
                table: "UserClasses",
                column: "ClassesId",
                principalTable: "Class",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
