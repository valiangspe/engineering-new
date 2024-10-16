using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace backend.Migrations
{
    /// <inheritdoc />
    public partial class multipns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "EngineeringDetailProblemItem",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    ExtItemId = table.Column<int>(type: "int", nullable: true),
                    EngineeringDetailProblemId = table.Column<int>(type: "int", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EngineeringDetailProblemItem", x => x.Id);
                    table.ForeignKey(
                        name: "FK_EngineeringDetailProblemItem_EngineeringDetailProblems_Engin~",
                        column: x => x.EngineeringDetailProblemId,
                        principalTable: "EngineeringDetailProblems",
                        principalColumn: "Id");
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_EngineeringDetailProblemItem_EngineeringDetailProblemId",
                table: "EngineeringDetailProblemItem",
                column: "EngineeringDetailProblemId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "EngineeringDetailProblemItem");
        }
    }
}
