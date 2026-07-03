using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketDescriptionHtml : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DescriptionHtml",
                table: "Tickets",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DescriptionHtml",
                table: "Tickets");
        }
    }
}
