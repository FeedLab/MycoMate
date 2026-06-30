using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MycoMate.Api.Migrations
{
    /// <inheritdoc />
    public partial class AddNutrientTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "BulkDensityKgPerM3",
                table: "Ingredients",
                type: "decimal(7,2)",
                precision: 7,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "CarbonToNitrogenRatio",
                table: "Ingredients",
                type: "decimal(7,2)",
                precision: 7,
                scale: 2,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Function",
                table: "Ingredients",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "PhLevel",
                table: "Ingredients",
                type: "decimal(4,2)",
                precision: 4,
                scale: 2,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AminoAcids",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AminoAcids", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Minerals",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Minerals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Vitamins",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    ShortName = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    Unit = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Vitamins", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "IngredientAminoAcids",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AminoAcidId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientAminoAcids", x => new { x.IngredientId, x.AminoAcidId });
                    table.ForeignKey(
                        name: "FK_IngredientAminoAcids_AminoAcids_AminoAcidId",
                        column: x => x.AminoAcidId,
                        principalTable: "AminoAcids",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_IngredientAminoAcids_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "IngredientMinerals",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    MineralId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientMinerals", x => new { x.IngredientId, x.MineralId });
                    table.ForeignKey(
                        name: "FK_IngredientMinerals_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientMinerals_Minerals_MineralId",
                        column: x => x.MineralId,
                        principalTable: "Minerals",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "IngredientVitamins",
                columns: table => new
                {
                    IngredientId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    VitaminId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Value = table.Column<decimal>(type: "decimal(10,4)", precision: 10, scale: 4, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_IngredientVitamins", x => new { x.IngredientId, x.VitaminId });
                    table.ForeignKey(
                        name: "FK_IngredientVitamins_Ingredients_IngredientId",
                        column: x => x.IngredientId,
                        principalTable: "Ingredients",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_IngredientVitamins_Vitamins_VitaminId",
                        column: x => x.VitaminId,
                        principalTable: "Vitamins",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AminoAcids_ShortName",
                table: "AminoAcids",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_IngredientAminoAcids_AminoAcidId",
                table: "IngredientAminoAcids",
                column: "AminoAcidId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientMinerals_MineralId",
                table: "IngredientMinerals",
                column: "MineralId");

            migrationBuilder.CreateIndex(
                name: "IX_IngredientVitamins_VitaminId",
                table: "IngredientVitamins",
                column: "VitaminId");

            migrationBuilder.CreateIndex(
                name: "IX_Minerals_ShortName",
                table: "Minerals",
                column: "ShortName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Vitamins_ShortName",
                table: "Vitamins",
                column: "ShortName",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "IngredientAminoAcids");

            migrationBuilder.DropTable(
                name: "IngredientMinerals");

            migrationBuilder.DropTable(
                name: "IngredientVitamins");

            migrationBuilder.DropTable(
                name: "AminoAcids");

            migrationBuilder.DropTable(
                name: "Minerals");

            migrationBuilder.DropTable(
                name: "Vitamins");

            migrationBuilder.DropColumn(
                name: "BulkDensityKgPerM3",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "CarbonToNitrogenRatio",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "Function",
                table: "Ingredients");

            migrationBuilder.DropColumn(
                name: "PhLevel",
                table: "Ingredients");
        }
    }
}
