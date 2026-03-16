using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogViewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class SeedSampleUser : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "dbo",
                table: "Users",
                columns: new[] { "Id", "Email", "Password", "Username" },
                values: new object[] { 1L, "sample@test.com", "$2a$12$diCUORXSxfHmtylUbv05YOHyJf.SNVKKKDkciiwGWiSmFH9//eqTa", "Sample username" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "dbo",
                table: "Users",
                keyColumn: "Id",
                keyValue: 1L);
        }
    }
}
