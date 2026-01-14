using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoraCollection.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddDisplayOrderAndIsActıve : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Slug",
                table: "StoneTypes");

            migrationBuilder.DropColumn(
                name: "Slug",
                table: "Colors");

            migrationBuilder.RenameColumn(
                name: "SKU",
                table: "ProductVariants",
                newName: "Sku");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "StoneTypes",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "StoneTypes",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsAvailable",
                table: "ProductVariants",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "Colors",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "Colors",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 761, DateTimeKind.Unspecified).AddTicks(8360), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 761, DateTimeKind.Unspecified).AddTicks(8360), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 761, DateTimeKind.Unspecified).AddTicks(8360), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 762, DateTimeKind.Unspecified).AddTicks(2460), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 762, DateTimeKind.Unspecified).AddTicks(2470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 762, DateTimeKind.Unspecified).AddTicks(2470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 762, DateTimeKind.Unspecified).AddTicks(2470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 10, 23, 29, 53, 762, DateTimeKind.Unspecified).AddTicks(2470), new TimeSpan(0, 0, 0, 0, 0)));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "StoneTypes");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "StoneTypes");

            migrationBuilder.DropColumn(
                name: "IsAvailable",
                table: "ProductVariants");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "Colors");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "Colors");

            migrationBuilder.RenameColumn(
                name: "Sku",
                table: "ProductVariants",
                newName: "SKU");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "StoneTypes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Slug",
                table: "Colors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 359, DateTimeKind.Unspecified).AddTicks(8860), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 359, DateTimeKind.Unspecified).AddTicks(8860), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 359, DateTimeKind.Unspecified).AddTicks(8860), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 360, DateTimeKind.Unspecified).AddTicks(2850), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 360, DateTimeKind.Unspecified).AddTicks(2860), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 360, DateTimeKind.Unspecified).AddTicks(2860), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 360, DateTimeKind.Unspecified).AddTicks(2860), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2025, 12, 18, 21, 53, 27, 360, DateTimeKind.Unspecified).AddTicks(2860), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
