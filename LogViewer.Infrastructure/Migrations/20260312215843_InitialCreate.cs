using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LogViewer.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Logs",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Timestamp = table.Column<DateTime>(type: "datetime2(7)", nullable: false),
                    Level = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Message = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Source = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ExceptionMessage = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    StackTrace = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    UserId = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: true),
                    MachineName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false),
                    RequestPath = table.Column<string>(type: "nvarchar(512)", maxLength: 512, nullable: true),
                    RequestMethod = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: true),
                    CorrelationId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    ThreadId = table.Column<int>(type: "int", nullable: true),
                    ApplicationName = table.Column<string>(type: "nvarchar(128)", maxLength: 128, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Logs", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Level",
                schema: "dbo",
                table: "Logs",
                column: "Level");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Source",
                schema: "dbo",
                table: "Logs",
                column: "Source");

            migrationBuilder.CreateIndex(
                name: "IX_Logs_Timestamp",
                schema: "dbo",
                table: "Logs",
                column: "Timestamp",
                descending: new[] { true });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Logs",
                schema: "dbo");
        }
    }
}
