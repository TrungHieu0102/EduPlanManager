using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlanManager.Migrations
{
    /// <inheritdoc />
    public partial class AddSummaryGradeTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "SumaryGradeId",
                table: "Grades",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "SumaryGrade",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudentId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AcademicTermId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Status = table.Column<int>(type: "int", nullable: false),
                    Summary = table.Column<float>(type: "real", nullable: false),
                    NeedsImprovement = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SumaryGrade", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Grades_SumaryGradeId",
                table: "Grades",
                column: "SumaryGradeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Grades_SumaryGrade_SumaryGradeId",
                table: "Grades",
                column: "SumaryGradeId",
                principalTable: "SumaryGrade",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Grades_SumaryGrade_SumaryGradeId",
                table: "Grades");

            migrationBuilder.DropTable(
                name: "SumaryGrade");

            migrationBuilder.DropIndex(
                name: "IX_Grades_SumaryGradeId",
                table: "Grades");

            migrationBuilder.DropColumn(
                name: "SumaryGradeId",
                table: "Grades");
        }
    }
}
