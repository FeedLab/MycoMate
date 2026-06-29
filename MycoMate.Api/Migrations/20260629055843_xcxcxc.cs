using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MycoMate.Api.Migrations
{
    /// <inheritdoc />
    public partial class xcxcxc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DryAmount",
                table: "RecipeIngredients",
                newName: "WetMatter");

            migrationBuilder.AddColumn<decimal>(
                name: "WaterAdjustmentPercent",
                table: "SubstrateRecipes",
                type: "decimal(7,4)",
                precision: 7,
                scale: 4,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DryMatter",
                table: "RecipeIngredients",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaterAdjustmentPercent",
                table: "SubstrateRecipes");

            migrationBuilder.DropColumn(
                name: "DryMatter",
                table: "RecipeIngredients");

            migrationBuilder.RenameColumn(
                name: "WetMatter",
                table: "RecipeIngredients",
                newName: "DryAmount");
        }
    }
}
