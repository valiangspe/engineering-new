using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class ecnfields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ExtItemId",
                table: "EngineeringDetailProblems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtJobId",
                table: "EngineeringDetailProblems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtPurchaseOrderId",
                table: "EngineeringDetailProblems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ExtUserId",
                table: "EngineeringDetailProblems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Type",
                table: "EngineeringDetailProblems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ExtItemId",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "ExtJobId",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "ExtPurchaseOrderId",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "ExtUserId",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "Type",
                table: "EngineeringDetailProblems");
        }
    }
}
