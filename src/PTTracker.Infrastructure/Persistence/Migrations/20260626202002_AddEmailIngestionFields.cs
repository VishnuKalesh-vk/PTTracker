using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PTTracker.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddEmailIngestionFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Source",
                table: "Tickets",
                type: "character varying(20)",
                maxLength: 20,
                nullable: false,
                defaultValue: "Manual");

            migrationBuilder.AddColumn<string>(
                name: "ThreadKey",
                table: "Tickets",
                type: "character varying(200)",
                maxLength: 200,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ContentHash",
                table: "Messages",
                type: "character varying(64)",
                maxLength: 64,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InReplyTo",
                table: "Messages",
                type: "character varying(998)",
                maxLength: 998,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MessageIdHeader",
                table: "Messages",
                type: "character varying(998)",
                maxLength: 998,
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "SentAt",
                table: "Messages",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_CustomerId_ThreadKey",
                table: "Tickets",
                columns: new[] { "CustomerId", "ThreadKey" });

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ContentHash",
                table: "Messages",
                column: "ContentHash");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_MessageIdHeader",
                table: "Messages",
                column: "MessageIdHeader",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Tickets_CustomerId_ThreadKey",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ContentHash",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Messages_MessageIdHeader",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "Source",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ThreadKey",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "ContentHash",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "InReplyTo",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "MessageIdHeader",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "SentAt",
                table: "Messages");
        }
    }
}
