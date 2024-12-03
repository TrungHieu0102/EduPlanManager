using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace EduPlanManager.Migrations
{
    /// <inheritdoc />
    public partial class ConfigureEnumsForSubjectSchedule : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SubjectSchedules",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    Session = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    EndTime = table.Column<TimeSpan>(type: "time", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectSchedules", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SubjectSubjectSchedule",
                columns: table => new
                {
                    SubjectId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SubjectScheduleId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SubjectSubjectSchedule", x => new { x.SubjectId, x.SubjectScheduleId });
                    table.ForeignKey(
                        name: "FK_SubjectSubjectSchedule_SubjectSchedules_SubjectScheduleId",
                        column: x => x.SubjectScheduleId,
                        principalTable: "SubjectSchedules",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SubjectSubjectSchedule_Subjects_SubjectId",
                        column: x => x.SubjectId,
                        principalTable: "Subjects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SubjectSubjectSchedule_SubjectScheduleId",
                table: "SubjectSubjectSchedule",
                column: "SubjectScheduleId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "SubjectSubjectSchedule");

            migrationBuilder.DropTable(
                name: "SubjectSchedules");
        }
    }
}
