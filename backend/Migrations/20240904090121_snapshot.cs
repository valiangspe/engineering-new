using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class snapshot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "SnapshotDate",
                table: "EngineeringDetailProblemItem",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<double>(
                name: "SnapshotPrice",
                table: "EngineeringDetailProblemItem",
                type: "double",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SnapshotDate",
                table: "EngineeringDetailProblemItem");

            migrationBuilder.DropColumn(
                name: "SnapshotPrice",
                table: "EngineeringDetailProblemItem");
        }
    }
}
