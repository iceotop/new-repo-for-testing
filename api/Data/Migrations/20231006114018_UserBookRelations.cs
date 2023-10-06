using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Api.Data.Migrations
{
    /// <inheritdoc />
    public partial class UserBookRelations : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserModelId",
                table: "Books",
                type: "TEXT",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Books_UserModelId",
                table: "Books",
                column: "UserModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_Books_AspNetUsers_UserModelId",
                table: "Books",
                column: "UserModelId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Books_AspNetUsers_UserModelId",
                table: "Books");

            migrationBuilder.DropIndex(
                name: "IX_Books_UserModelId",
                table: "Books");

            migrationBuilder.DropColumn(
                name: "UserModelId",
                table: "Books");
        }
    }
}
