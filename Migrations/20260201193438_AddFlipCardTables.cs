using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nafes.API.Migrations
{
    /// <inheritdoc />
    public partial class AddFlipCardTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FlipCardQuestions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<int>(type: "int", nullable: false),
                    GameTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    GameMode = table.Column<int>(type: "int", nullable: false),
                    NumberOfPairs = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    TimerMode = table.Column<int>(type: "int", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "int", nullable: true),
                    PointsPerMatch = table.Column<int>(type: "int", nullable: false),
                    MovePenalty = table.Column<int>(type: "int", nullable: false),
                    ShowHints = table.Column<bool>(type: "bit", nullable: false),
                    MaxHints = table.Column<int>(type: "int", nullable: false),
                    UITheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CardBackDesign = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    CustomCardBackUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    EnableAudio = table.Column<bool>(type: "bit", nullable: false),
                    EnableExplanations = table.Column<bool>(type: "bit", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    CreatedBy = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LastModifiedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardQuestions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FlipCardGameSessions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    StudentId = table.Column<long>(type: "bigint", nullable: false),
                    FlipCardQuestionId = table.Column<int>(type: "int", nullable: false),
                    Grade = table.Column<int>(type: "int", nullable: false),
                    Subject = table.Column<int>(type: "int", nullable: false),
                    GameMode = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: true),
                    TotalPairs = table.Column<int>(type: "int", nullable: false),
                    MatchedPairs = table.Column<int>(type: "int", nullable: false),
                    TotalMoves = table.Column<int>(type: "int", nullable: false),
                    WrongAttempts = table.Column<int>(type: "int", nullable: false),
                    TotalScore = table.Column<int>(type: "int", nullable: false),
                    TimeSpentSeconds = table.Column<int>(type: "int", nullable: false),
                    HintsUsed = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    StarRating = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardGameSessions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlipCardGameSessions_FlipCardQuestions_FlipCardQuestionId",
                        column: x => x.FlipCardQuestionId,
                        principalTable: "FlipCardQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FlipCardGameSessions_Students_StudentId",
                        column: x => x.StudentId,
                        principalTable: "Students",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlipCardPairs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    FlipCardQuestionId = table.Column<int>(type: "int", nullable: false),
                    Card1Type = table.Column<int>(type: "int", nullable: false),
                    Card1Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card1ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card1AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card2Type = table.Column<int>(type: "int", nullable: false),
                    Card2Text = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card2ImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Card2AudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Explanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PairOrder = table.Column<int>(type: "int", nullable: false),
                    DifficultyWeight = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardPairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlipCardPairs_FlipCardQuestions_FlipCardQuestionId",
                        column: x => x.FlipCardQuestionId,
                        principalTable: "FlipCardQuestions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "FlipCardAttempts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<int>(type: "int", nullable: false),
                    PairId = table.Column<int>(type: "int", nullable: false),
                    Card1FlippedAtMs = table.Column<int>(type: "int", nullable: false),
                    Card2FlippedAtMs = table.Column<int>(type: "int", nullable: false),
                    IsCorrectMatch = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    AttemptsBeforeMatch = table.Column<int>(type: "int", nullable: false),
                    AttemptTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FlipCardAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FlipCardAttempts_FlipCardGameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "FlipCardGameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardAttempts_SessionId",
                table: "FlipCardAttempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardGameSessions_FlipCardQuestionId",
                table: "FlipCardGameSessions",
                column: "FlipCardQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardGameSessions_StudentId",
                table: "FlipCardGameSessions",
                column: "StudentId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardPairs_FlipCardQuestionId",
                table: "FlipCardPairs",
                column: "FlipCardQuestionId");

            migrationBuilder.CreateIndex(
                name: "IX_FlipCardQuestions_Grade_Subject_IsActive",
                table: "FlipCardQuestions",
                columns: new[] { "Grade", "Subject", "IsActive" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FlipCardAttempts");

            migrationBuilder.DropTable(
                name: "FlipCardPairs");

            migrationBuilder.DropTable(
                name: "FlipCardGameSessions");

            migrationBuilder.DropTable(
                name: "FlipCardQuestions");
        }
    }
}
