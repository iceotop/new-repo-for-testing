using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class BookEventRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EventId",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_EventId",
                table: "Books",
                column: "EventId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_Events_EventId",
                table: "Books",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_Events_EventId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_EventId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "EventId",
                table: "Books");
        }
    }
}
