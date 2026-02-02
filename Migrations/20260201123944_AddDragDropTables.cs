using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nafes.API.Migrations
{
    /// <inheritdoc />
    public partial class AddDragDropTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DragDropQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<int>(type: "int", nullable: false),
                    GameTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    NumberOfZones = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    TimeLimit = table.Column<int>(type: "int", nullable: true),
                    PointsPerCorrectItem = table.Column<int>(type: "int", nullable: false),
                    ShowImmediateFeedback = table.Column<bool>(type: "bit", nullable: false),
                    UITheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "DragDropGameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<int>(type: "int", nullable: false),
                    DragDropQuestionId = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalItems = table.Column<int>(type: "int", nullable: false),
                    CorrectPlacements = table.Column<int>(type: "int", nullable: false),
                    WrongPlacements = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropGameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropGameSessions_DragDropQuestions_DragDropQuestionId",
                        column: x => x.DragDropQuestionId,
                        principalTable: "DragDropQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DragDropZones",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DragDropQuestionId = table.Column<int>(type: "int", nullable: false),
                    Label = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ColorCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    ZoneOrder = table.Column<int>(type: "int", nullable: false),
                    IconUrl = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropZones", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropZones_DragDropQuestions_DragDropQuestionId",
                        column: x => x.DragDropQuestionId,
                        principalTable: "DragDropQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DragDropItems",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DragDropQuestionId = table.Column<int>(type: "int", nullable: false),
                    Text = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    CorrectZoneId = table.Column<int>(type: "int", nullable: false),
                    ItemOrder = table.Column<int>(type: "int", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropItems_DragDropQuestions_DragDropQuestionId",
                        column: x => x.DragDropQuestionId,
                        principalTable: "DragDropQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DragDropItems_DragDropZones_CorrectZoneId",
                        column: x => x.CorrectZoneId,
                        principalTable: "DragDropZones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "DragDropAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    ItemId = table.Column<int>(type: "int", nullable: false),
                    PlacedInZoneId = table.Column<int>(type: "int", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    AttemptTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DragDropAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_DragDropAttempts_DragDropGameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "DragDropGameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DragDropAttempts_DragDropItems_ItemId",
                        column: x => x.ItemId,
                        principalTable: "DragDropItems",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_DragDropAttempts_DragDropZones_PlacedInZoneId",
                        column: x => x.PlacedInZoneId,
                        principalTable: "DragDropZones",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_DragDropAttempts_ItemId",
                table: "DragDropAttempts",
                column: "ItemId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropAttempts_PlacedInZoneId",
                table: "DragDropAttempts",
                column: "PlacedInZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropAttempts_SessionId_ItemId",
                table: "DragDropAttempts",
                columns: new[] { "SessionId", "ItemId" });

            migrationBuilder.CreateIndex(
                name: "IX_DragDropGameSessions_DragDropQuestionId",
                table: "DragDropGameSessions",
                column: "DragDropQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropGameSessions_StudentId_IsCompleted",
                table: "DragDropGameSessions",
                columns: new[] { "StudentId", "IsCompleted" });

            migrationBuilder.CreateIndex(
                name: "IX_DragDropItems_CorrectZoneId",
                table: "DragDropItems",
                column: "CorrectZoneId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropItems_DragDropQuestionId",
                table: "DragDropItems",
                column: "DragDropQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_DragDropQuestions_Grade_Subject_IsActive",
                table: "DragDropQuestions",
                columns: new[] { "Grade", "Subject", "IsActive" });

            migrationBuilder.CreateIndex(
                name: "IX_DragDropZones_DragDropQuestionId",
                table: "DragDropZones",
                column: "DragDropQuestionId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DragDropAttempts");

            migrationBuilder.DropTable(
                name: "DragDropGameSessions");

            migrationBuilder.DropTable(
                name: "DragDropItems");

            migrationBuilder.DropTable(
                name: "DragDropZones");

            migrationBuilder.DropTable(
                name: "DragDropQuestions");
        }
    }
}
