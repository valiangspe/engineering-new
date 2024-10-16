using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class deletedat : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Tasks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "SupportDetails",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "OutstandingPostPOs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "InCharges",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EngineerSupports",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EngineeringDetailProblems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EngineeringDetailProblemItem",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EngineeringActivities",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "EngDeptConfigs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "ECNData",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DonePOs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "DashboardMetrics",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "Configurations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DeletedAt",
                table: "CCNData",
                type: "datetime(6)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "SupportDetails");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "OutstandingPostPOs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "InCharges");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EngineerSupports");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EngineeringDetailProblemItem");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EngineeringActivities");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "EngDeptConfigs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "ECNData");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DonePOs");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "DashboardMetrics");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "DeletedAt",
                table: "CCNData");
        }
    }
}
