using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RetailSphere.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddUnitsAndProductFlags : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BarcodeType",
                table: "ProductVariants",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "C128")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "CostPrice",
                table: "ProductVariants",
                type: "decimal(18,2)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Height",
                table: "ProductVariants",
                type: "decimal(10,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Length",
                table: "ProductVariants",
                type: "decimal(10,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "TaxRate",
                table: "ProductVariants",
                type: "decimal(5,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "TaxType",
                table: "ProductVariants",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Exclusive")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<string>(
                name: "UnitOfMeasure",
                table: "ProductVariants",
                type: "varchar(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Each")
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AddColumn<decimal>(
                name: "Weight",
                table: "ProductVariants",
                type: "decimal(10,3)",
                nullable: true);

            migrationBuilder.AddColumn<decimal>(
                name: "Width",
                table: "ProductVariants",
                type: "decimal(10,3)",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "ManageStock",
                table: "Products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: true);

            migrationBuilder.AddColumn<bool>(
                name: "NotForSelling",
                table: "Products",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<long>(
                name: "UnitId",
                table: "Products",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Units",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    Name = table.Column<string>(type: "varchar(100)", maxLength: 100, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    ShortCode = table.Column<string>(type: "varchar(20)", maxLength: 20, nullable: false)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    AllowDecimal = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    IsActive = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: true),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: false),
                    CreatedBy = table.Column<long>(type: "bigint", nullable: true),
                    ModifiedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    ModifiedBy = table.Column<long>(type: "bigint", nullable: true),
                    IsDeleted = table.Column<bool>(type: "tinyint(1)", nullable: false, defaultValue: false),
                    DeletedAtUtc = table.Column<DateTime>(type: "datetime(6)", nullable: true),
                    DeletedBy = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Units", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.CreateIndex(
                name: "IX_Units_Name",
                table: "Units",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Units");

            migrationBuilder.DropColumn(
                name: "BarcodeType",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "CostPrice",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Height",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Length",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "TaxRate",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "TaxType",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "UnitOfMeasure",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Weight",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "Width",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "ManageStock",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "NotForSelling",
                table: "Products");

            migrationBuilder.DropColumn(
                name: "UnitId",
                table: "Products");
        }
    }
}
