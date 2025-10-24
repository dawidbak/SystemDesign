using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Notification.Application.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class New_Column_Subject_in_Template : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Subject",
                table: "Templates",
                type: "character varying(200)",
                maxLength: 200,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Subject",
                table: "Templates");
        }
    }
}
