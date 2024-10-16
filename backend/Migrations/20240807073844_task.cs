using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class task : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InCharge_Task_TaskId",
                table: "InCharge");

            migrationBuilder.DropForeignKey(
                name: "FK_Task_EngineeringActivities_EngineeringActivityId",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Task",
                table: "Task");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InCharge",
                table: "InCharge");

            migrationBuilder.RenameTable(
                name: "Task",
                newName: "Tasks");

            migrationBuilder.RenameTable(
                name: "InCharge",
                newName: "InCharges");

            migrationBuilder.RenameIndex(
                name: "IX_Task_EngineeringActivityId",
                table: "Tasks",
                newName: "IX_Tasks_EngineeringActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_InCharge_TaskId",
                table: "InCharges",
                newName: "IX_InCharges_TaskId");

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "SupportDetails",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "SupportDetails",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "OutstandingPostPOs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "OutstandingPostPOs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EngineerSupports",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EngineerSupports",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EngineeringDetailProblems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EngineeringDetailProblems",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EngineeringActivities",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EngineeringActivities",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "EngDeptConfigs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "EngDeptConfigs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ECNData",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ECNData",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DonePOs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DonePOs",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "DashboardMetrics",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "DashboardMetrics",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Configurations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Configurations",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "CCNData",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "CCNData",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "Tasks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "Tasks",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "InCharges",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "InCharges",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InCharges",
                table: "InCharges",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InCharges_Tasks_TaskId",
                table: "InCharges",
                column: "TaskId",
                principalTable: "Tasks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Tasks_EngineeringActivities_EngineeringActivityId",
                table: "Tasks",
                column: "EngineeringActivityId",
                principalTable: "EngineeringActivities",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_InCharges_Tasks_TaskId",
                table: "InCharges");

            migrationBuilder.DropForeignKey(
                name: "FK_Tasks_EngineeringActivities_EngineeringActivityId",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Tasks",
                table: "Tasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_InCharges",
                table: "InCharges");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "SupportDetails");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "SupportDetails");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "OutstandingPostPOs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "OutstandingPostPOs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EngineerSupports");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EngineerSupports");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EngineeringDetailProblems");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EngineeringActivities");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EngineeringActivities");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "EngDeptConfigs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "EngDeptConfigs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ECNData");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ECNData");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DonePOs");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DonePOs");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "DashboardMetrics");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "DashboardMetrics");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Configurations");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "CCNData");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "CCNData");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "Tasks");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "InCharges");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "InCharges");

            migrationBuilder.RenameTable(
                name: "Tasks",
                newName: "Task");

            migrationBuilder.RenameTable(
                name: "InCharges",
                newName: "InCharge");

            migrationBuilder.RenameIndex(
                name: "IX_Tasks_EngineeringActivityId",
                table: "Task",
                newName: "IX_Task_EngineeringActivityId");

            migrationBuilder.RenameIndex(
                name: "IX_InCharges_TaskId",
                table: "InCharge",
                newName: "IX_InCharge_TaskId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Task",
                table: "Task",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_InCharge",
                table: "InCharge",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InCharge_Task_TaskId",
                table: "InCharge",
                column: "TaskId",
                principalTable: "Task",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Task_EngineeringActivities_EngineeringActivityId",
                table: "Task",
                column: "EngineeringActivityId",
                principalTable: "EngineeringActivities",
                principalColumn: "Id");
        }
    }
}
