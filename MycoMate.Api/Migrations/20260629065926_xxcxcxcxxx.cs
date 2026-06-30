using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MycoMate.Api.Migrations
{
    /// <inheritdoc />
    public partial class xxcxcxcxxx : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DryAmountPercent",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "DryMatter",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "WetAmount",
                table: "RecipeIngredients");

            migrationBuilder.DropColumn(
                name: "WetMatter",
                table: "RecipeIngredients");

            migrationBuilder.RenameColumn(
                name: "WetAmountPercent",
                table: "RecipeIngredients",
                newName: "DryPercent");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "DryPercent",
                table: "RecipeIngredients",
                newName: "WetAmountPercent");

            migrationBuilder.AddColumn<decimal>(
                name: "DryAmountPercent",
                table: "RecipeIngredients",
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

            migrationBuilder.AddColumn<decimal>(
                name: "WetAmount",
                table: "RecipeIngredients",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "WetMatter",
                table: "RecipeIngredients",
                type: "decimal(10,3)",
                precision: 10,
                scale: 3,
                nullable: false,
                defaultValue: 0m);
        }
    }
}
