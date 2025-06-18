using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class amrgin : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "MarginAfter",
                table: "EngineeringDetailProblems",
                type: "double",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "MarginBefore",
                table: "EngineeringDetailProblems",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MarginAfter",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "MarginBefore",
                table: "EngineeringDetailProblems");
        }
    }
}
