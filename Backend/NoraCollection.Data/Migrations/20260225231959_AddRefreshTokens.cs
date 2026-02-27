using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NoraCollection.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddRefreshTokens : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Token = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    IsRevoked = table.Column<bool>(type: "bit", nullable: false),
                    ReplacedByTokenId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                    table.ForeignKey(
                        name: "FK_RefreshTokens_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id");
                });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2535fc28-1f63-4389-837d-7c5c16bcbdea",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RegisterionDate", "SecurityStamp" },
                values: new object[] { "9c7c6d5c-0c0c-426d-a66b-ac54d2b1b956", "AQAAAAIAAYagAAAAEPtHzasaHP+aIwKWsjlxsbaHrqeSQwlwd/SK8/G+JcplMaXJqhwc2LBfekKUFFkdZA==", new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 843, DateTimeKind.Unspecified).AddTicks(7820), new TimeSpan(0, 0, 0, 0, 0)), "5f338bf8-b2c2-4639-b006-d754dcb4a14d" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "af7be952-7e42-4025-ae5b-efa3b3a9a728",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RegisterionDate", "SecurityStamp" },
                values: new object[] { "dca7b9ea-9721-4366-90c0-3d841fbc6511", "AQAAAAIAAYagAAAAEF3ta1LZSxpPzewMtPRGF3Wutkuijwz3ZgeSynJfq9YsvA6PhLlIBmAw9ExLfskP5Q==", new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(9830), new TimeSpan(0, 0, 0, 0, 0)), "0c854849-7219-4b3d-a80e-e6193815130e" });

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 879, DateTimeKind.Unspecified).AddTicks(3240), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 879, DateTimeKind.Unspecified).AddTicks(3240), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(5880), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(5880), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(5880), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(9710), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(9720), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(9720), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(9720), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 25, 23, 19, 58, 807, DateTimeKind.Unspecified).AddTicks(9720), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.CreateIndex(
                name: "IX_RefreshTokens_UserId",
                table: "RefreshTokens",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2535fc28-1f63-4389-837d-7c5c16bcbdea",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RegisterionDate", "SecurityStamp" },
                values: new object[] { "01f6cab5-4a98-4517-b9d6-49823fae7691", "AQAAAAIAAYagAAAAEMtw7zid17PaEsT2M8e1tmArCc7mOgEO6swI+LS0O9HGkZspDCKR3RHUOSI37A99MQ==", new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 701, DateTimeKind.Unspecified).AddTicks(1860), new TimeSpan(0, 0, 0, 0, 0)), "7ec952b1-4532-45aa-9d45-af558c2cf191" });

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "af7be952-7e42-4025-ae5b-efa3b3a9a728",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "RegisterionDate", "SecurityStamp" },
                values: new object[] { "927b275f-86f1-4233-b6ed-66ff756558cf", "AQAAAAIAAYagAAAAEO0l3g3c+E1PTsF3DGa3MFZbO+UZlPnDKdlh0Fa1LGs4yxEUj+rTLRRUy4QO/ZC6gw==", new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(9290), new TimeSpan(0, 0, 0, 0, 0)), "af86fce6-a966-4fff-8a6a-801fff0dedd3" });

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 737, DateTimeKind.Unspecified).AddTicks(8430), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 737, DateTimeKind.Unspecified).AddTicks(8430), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(4870), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(4870), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(4870), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(9180), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(9190), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(9190), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(9190), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(9190), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
