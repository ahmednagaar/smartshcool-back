using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nafes.API.Migrations
{
    /// <inheritdoc />
    public partial class ModifyMatchingGameStructure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MatchingQuestions");

            migrationBuilder.RenameColumn(
                name: "TotalQuestions",
                table: "MatchingGameSessions",
                newName: "WrongAttempts");

            migrationBuilder.RenameColumn(
                name: "Score",
                table: "MatchingGameSessions",
                newName: "TotalScore");

            migrationBuilder.RenameColumn(
                name: "CorrectMatches",
                table: "MatchingGameSessions",
                newName: "TotalPairs");

            migrationBuilder.AddColumn<int>(
                name: "HintsUsed",
                table: "MatchingGameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "MatchingGameSessions",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "MatchedPairs",
                table: "MatchingGameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<long>(
                name: "MatchingGameId",
                table: "MatchingGameSessions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.AddColumn<int>(
                name: "StarRating",
                table: "MatchingGameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "TotalMoves",
                table: "MatchingGameSessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "MatchingAttempts",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SessionId = table.Column<long>(type: "bigint", nullable: false),
                    PairId = table.Column<long>(type: "bigint", nullable: false),
                    IsCorrect = table.Column<bool>(type: "bit", nullable: false),
                    PointsEarned = table.Column<int>(type: "int", nullable: false),
                    TimeToMatchMs = table.Column<int>(type: "int", nullable: false),
                    AttemptTime = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingAttempts_MatchingGameSessions_SessionId",
                        column: x => x.SessionId,
                        principalTable: "MatchingGameSessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "MatchingGames",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GameTitle = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Instructions = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    NumberOfPairs = table.Column<int>(type: "int", nullable: false),
                    MatchingMode = table.Column<int>(type: "int", nullable: false),
                    UITheme = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    ShowConnectingLines = table.Column<bool>(type: "bit", nullable: false),
                    EnableAudio = table.Column<bool>(type: "bit", nullable: false),
                    EnableHints = table.Column<bool>(type: "bit", nullable: false),
                    MaxHints = table.Column<int>(type: "int", nullable: false),
                    TimerMode = table.Column<int>(type: "int", nullable: false),
                    TimeLimitSeconds = table.Column<int>(type: "int", nullable: true),
                    PointsPerMatch = table.Column<int>(type: "int", nullable: false),
                    WrongMatchPenalty = table.Column<int>(type: "int", nullable: false),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingGames", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MatchingGamePairs",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    MatchingGameId = table.Column<long>(type: "bigint", nullable: false),
                    QuestionText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    QuestionImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionAudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    QuestionType = table.Column<int>(type: "int", nullable: false),
                    AnswerText = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    AnswerImageUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnswerAudioUrl = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AnswerType = table.Column<int>(type: "int", nullable: false),
                    Explanation = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: true),
                    PairOrder = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingGamePairs", x => x.Id);
                    table.ForeignKey(
                        name: "FK_MatchingGamePairs_MatchingGames_MatchingGameId",
                        column: x => x.MatchingGameId,
                        principalTable: "MatchingGames",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MatchingGameSessions_MatchingGameId",
                table: "MatchingGameSessions",
                column: "MatchingGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingAttempts_SessionId",
                table: "MatchingAttempts",
                column: "SessionId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingGamePairs_MatchingGameId",
                table: "MatchingGamePairs",
                column: "MatchingGameId");

            migrationBuilder.CreateIndex(
                name: "IX_MatchingGames_GradeId_SubjectId_IsActive",
                table: "MatchingGames",
                columns: new[] { "GradeId", "SubjectId", "IsActive" });

            migrationBuilder.AddForeignKey(
                name: "FK_MatchingGameSessions_MatchingGames_MatchingGameId",
                table: "MatchingGameSessions",
                column: "MatchingGameId",
                principalTable: "MatchingGames",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MatchingGameSessions_MatchingGames_MatchingGameId",
                table: "MatchingGameSessions");

            migrationBuilder.DropTable(
                name: "MatchingAttempts");

            migrationBuilder.DropTable(
                name: "MatchingGamePairs");

            migrationBuilder.DropTable(
                name: "MatchingGames");

            migrationBuilder.DropIndex(
                name: "IX_MatchingGameSessions_MatchingGameId",
                table: "MatchingGameSessions");

            migrationBuilder.DropColumn(
                name: "HintsUsed",
                table: "MatchingGameSessions");

            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "MatchingGameSessions");

            migrationBuilder.DropColumn(
                name: "MatchedPairs",
                table: "MatchingGameSessions");

            migrationBuilder.DropColumn(
                name: "MatchingGameId",
                table: "MatchingGameSessions");

            migrationBuilder.DropColumn(
                name: "StarRating",
                table: "MatchingGameSessions");

            migrationBuilder.DropColumn(
                name: "TotalMoves",
                table: "MatchingGameSessions");

            migrationBuilder.RenameColumn(
                name: "WrongAttempts",
                table: "MatchingGameSessions",
                newName: "TotalQuestions");

            migrationBuilder.RenameColumn(
                name: "TotalScore",
                table: "MatchingGameSessions",
                newName: "Score");

            migrationBuilder.RenameColumn(
                name: "TotalPairs",
                table: "MatchingGameSessions",
                newName: "CorrectMatches");

            migrationBuilder.CreateTable(
                name: "MatchingQuestions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    DifficultyLevel = table.Column<int>(type: "int", nullable: false),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    DistractorItems = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GradeId = table.Column<int>(type: "int", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false),
                    LeftItemText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    RightItemText = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    SubjectId = table.Column<int>(type: "int", nullable: false),
                    UpdatedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UpdatedDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MatchingQuestions", x => x.Id);
                });
        }
    }
}
