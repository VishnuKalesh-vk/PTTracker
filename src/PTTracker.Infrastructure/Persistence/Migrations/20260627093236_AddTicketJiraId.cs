using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketJiraId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "JiraId",
                table: "Tickets",
                type: "character varying(50)",
                maxLength: 50,
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "JiraId",
                table: "Tickets");
        }
    }
}
