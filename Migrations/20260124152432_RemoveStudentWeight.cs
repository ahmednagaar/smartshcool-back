using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Nafes.API.Migrations
{
    /// <inheritdoc />
    public partial class RemoveStudentWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Weight",
                table: "Students");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "Weight",
                table: "Students",
                type: "float",
                nullable: true);
        }
    }
}
