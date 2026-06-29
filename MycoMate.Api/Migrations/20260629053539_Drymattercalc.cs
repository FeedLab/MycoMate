using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MycoMate.Api.Migrations
{
    /// <inheritdoc />
    public partial class Drymattercalc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "DryAmount",
                table: "RecipeIngredients",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "DryAmountPercent",
                table: "RecipeIngredients",
                type: "decimal(7,4)",
                precision: 7,
                scale: 4,
                nullable: false,
                defaultValue: 0m);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DryAmount",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "DryAmountPercent",
                table: "RecipeIngredients");
        }
    }
}
