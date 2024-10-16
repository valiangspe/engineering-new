using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class apprvoal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "ApprovalDate",
                table: "EngineeringDetailProblems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ApprovalExtUserId",
                table: "EngineeringDetailProblems",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ApprovalRemark",
                table: "EngineeringDetailProblems",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EngineeringDetailProblems",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ApprovalDate",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "ApprovalExtUserId",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "ApprovalRemark",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EngineeringDetailProblems");
        }
    }
}
