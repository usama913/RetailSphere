using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailSphere.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemoveVariantUnitOfMeasure : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "ProductVariants");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "ProductVariants",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Each")
                .Annotation("MySql:CharSet", "utf8mb4");
        }
    }
}
