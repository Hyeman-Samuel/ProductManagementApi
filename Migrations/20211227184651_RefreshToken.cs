using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ProductManagementApi.Migrations
{
    public partial class RefreshToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokens",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IsSoftDelete = table.Column<bool>(nullable: false),
                    CreatedAt = table.Column<DateTime>(nullable: false),
                    Token = table.Column<string>(nullable: true),
                    JwtId = table.Column<string>(nullable: true),
                    ExpiryDate = table.Column<DateTime>(nullable: false),
                    IsUsed = table.Column<bool>(nullable: false),
                    Invalidated = table.Column<bool>(nullable: false),
                    UserId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RefreshTokens", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                column: "ConcurrencyStamp",
                value: "619e53ed-f9de-4be1-8d09-e2517a32254a");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "3d15a5e2-3921-41e8-a40d-ac9276a72889", "AQAAAAEAACcQAAAAEHy1kRuyTK++5Yyb9OSaxGOpQ9BEEB7UncXivNvxxkFFrLKNVEcOuDlwODVz1kwdjA==", "6c8c27f5-1df9-445a-849a-f77e64116150" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RefreshTokens");

            migrationBuilder.UpdateData(
                table: "AspNetRoles",
                keyColumn: "Id",
                keyValue: "341743f0-asd2–42de-afbf-59kmkkmk72cf6",
                column: "ConcurrencyStamp",
                value: "6c737d57-a3ed-4e95-90b7-5ab5d8af61f6");

            migrationBuilder.UpdateData(
                table: "AspNetUsers",
                keyColumn: "Id",
                keyValue: "02174cf0–9412–4cfe-afbf-59f706d72cf6",
                columns: new[] { "ConcurrencyStamp", "PasswordHash", "SecurityStamp" },
                values: new object[] { "9d19ed0b-571f-43c2-bde2-bc36bc722649", "AQAAAAEAACcQAAAAEGfkyIgf3/kukkwitfJlg15v4Jc87Ua+A8Xwls99+4T7aQdG24QvzEKM6yNe3jSTtg==", "cf22abb8-07c7-44dd-b29f-686eda006f47" });
        }
    }
}
