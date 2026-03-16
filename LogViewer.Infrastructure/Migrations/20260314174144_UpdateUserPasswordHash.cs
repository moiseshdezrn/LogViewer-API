using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogViewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateUserPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "$2a$12$diCUORXSxfHmtylUbv05YOHyJf.SNVKKKDkciiwGWiSmFH9//eqTa");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L,
                column: "Password",
                value: "test");
        }
    }
}
