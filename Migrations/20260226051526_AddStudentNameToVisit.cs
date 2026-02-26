using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nafes.API.Migrations
{
    /// <inheritdoc />
    public partial class AddStudentNameToVisit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "StudentName",
                table: "Visits",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "StudentName",
                table: "Visits");
        }
    }
}
