using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NotificationApp.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class NotEntityupt : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "ForceSend",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsCanceled",
                table: "Notifications",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ForceSend",
                table: "Notifications");

            migrationBuilder.DropColumn(
                name: "IsCanceled",
                table: "Notifications");
        }
    }
}
