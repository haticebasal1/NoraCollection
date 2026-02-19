using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace NoraCollection.Data.Migrations
{
    /// <inheritdoc />
    public partial class SeedUsersAndRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CouponId",
                table: "Carts",
                type: "int",
                nullable: true);

            migrationBuilder.InsertData(
                table: "AspNetRoles",
                columns: new[] { "Id", "ConcurrencyStamp", "Name", "NormalizedName" },
                values: new object[,]
                {
                    { "1a401768-5c01-4c34-bf0d-547f0b8b4ab8", null, "Admin", "ADMIN" },
                    { "49afd76c-933f-4579-8346-d6c5d8b1a799", null, "User", "USER" }
                });

            migrationBuilder.InsertData(
                table: "AspNetUsers",
                columns: new[] { "Id", "AccessFailedCount", "Address", "BirthDate", "City", "ConcurrencyStamp", "Email", "EmailConfirmed", "FirstName", "Gender", "LastName", "LockoutEnabled", "LockoutEnd", "NormalizedEmail", "NormalizedUserName", "PasswordHash", "PhoneNumber", "PhoneNumberConfirmed", "RegisterionDate", "SecurityStamp", "TwoFactorEnabled", "UserName" },
                values: new object[,]
                {
                    { "2535fc28-1f63-4389-837d-7c5c16bcbdea", 0, "Üsküdar", null, "İstanbul", "01f6cab5-4a98-4517-b9d6-49823fae7691", "testuser@example.com", true, "Esin", 1, "Çelik", false, null, "TESTUSER@EXAMPLE.COM", "TESTUSER", "AQAAAAIAAYagAAAAEMtw7zid17PaEsT2M8e1tmArCc7mOgEO6swI+LS0O9HGkZspDCKR3RHUOSI37A99MQ==", null, false, new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 701, DateTimeKind.Unspecified).AddTicks(1860), new TimeSpan(0, 0, 0, 0, 0)), "7ec952b1-4532-45aa-9d45-af558c2cf191", false, "testuser" },
                    { "af7be952-7e42-4025-ae5b-efa3b3a9a728", 0, "Kadıköy", null, "İstanbul", "927b275f-86f1-4233-b6ed-66ff756558cf", "testadmin@example.com", true, "Hatice", 2, "Başal", false, null, "TESTADMIN@EXAMPLE.COM", "TESTADMIN", "AQAAAAIAAYagAAAAEO0l3g3c+E1PTsF3DGa3MFZbO+UZlPnDKdlh0Fa1LGs4yxEUj+rTLRRUy4QO/ZC6gw==", null, false, new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 663, DateTimeKind.Unspecified).AddTicks(9290), new TimeSpan(0, 0, 0, 0, 0)), "af86fce6-a966-4fff-8a6a-801fff0dedd3", false, "testadmin" }
                });

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

            migrationBuilder.InsertData(
                table: "AspNetUserRoles",
                columns: new[] { "RoleId", "UserId" },
                values: new object[,]
                {
                    { "49afd76c-933f-4579-8346-d6c5d8b1a799", "2535fc28-1f63-4389-837d-7c5c16bcbdea" },
                    { "1a401768-5c01-4c34-bf0d-547f0b8b4ab8", "af7be952-7e42-4025-ae5b-efa3b3a9a728" }
                });

            migrationBuilder.InsertData(
                table: "Carts",
                columns: new[] { "Id", "CouponId", "CreatedAt", "DeletedAt", "GiftNote", "GiftOptionId", "IsActive", "IsDeleted", "IsGiftPackage", "UpdatedAt", "UserId" },
                values: new object[,]
                {
                    { 1, null, new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 737, DateTimeKind.Unspecified).AddTicks(8430), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "af7be952-7e42-4025-ae5b-efa3b3a9a728" },
                    { 2, null, new DateTimeOffset(new DateTime(2026, 2, 15, 15, 2, 9, 737, DateTimeKind.Unspecified).AddTicks(8430), new TimeSpan(0, 0, 0, 0, 0)), new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), null, null, true, false, false, new DateTimeOffset(new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), new TimeSpan(0, 0, 0, 0, 0)), "2535fc28-1f63-4389-837d-7c5c16bcbdea" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Carts_CouponId",
                table: "Carts",
                column: "CouponId");

            migrationBuilder.AddForeignKey(
                name: "FK_Carts_Coupons_CouponId",
                table: "Carts",
                column: "CouponId",
                principalTable: "Coupons",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Carts_Coupons_CouponId",
                table: "Carts");

            migrationBuilder.DropIndex(
                name: "IX_Carts_CouponId",
                table: "Carts");

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "49afd76c-933f-4579-8346-d6c5d8b1a799", "2535fc28-1f63-4389-837d-7c5c16bcbdea" });

            migrationBuilder.DeleteData(
                table: "AspNetUserRoles",
                keyColumns: new[] { "RoleId", "UserId" },
                keyValues: new object[] { "1a401768-5c01-4c34-bf0d-547f0b8b4ab8", "af7be952-7e42-4025-ae5b-efa3b3a9a728" });

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Carts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "1a401768-5c01-4c34-bf0d-547f0b8b4ab8");

            migrationBuilder.DeleteData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "49afd76c-933f-4579-8346-d6c5d8b1a799");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "2535fc28-1f63-4389-837d-7c5c16bcbdea");

            migrationBuilder.DeleteData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "af7be952-7e42-4025-ae5b-efa3b3a9a728");

            migrationBuilder.DropColumn(
                name: "CouponId",
                table: "Carts");

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(2470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(2470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Categories",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(2470), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(6140), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(6150), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(6150), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(6150), new TimeSpan(0, 0, 0, 0, 0)));

            migrationBuilder.UpdateData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5,
                column: "CreatedAt",
                value: new DateTimeOffset(new DateTime(2026, 1, 29, 21, 52, 39, 412, DateTimeKind.Unspecified).AddTicks(6150), new TimeSpan(0, 0, 0, 0, 0)));
        }
    }
}
