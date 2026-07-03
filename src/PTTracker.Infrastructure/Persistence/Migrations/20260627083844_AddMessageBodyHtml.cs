using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddMessageBodyHtml : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "BodyHtml",
                table: "Messages",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "BodyHtml",
                table: "Messages");
        }
    }
}
