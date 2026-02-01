using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nafes.API.Migrations
{
    /// <inheritdoc />
    public partial class AddTestTypeToWheelQuestions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TestType",
                table: "WheelQuestions",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TestType",
                table: "WheelQuestions");
        }
    }
}
