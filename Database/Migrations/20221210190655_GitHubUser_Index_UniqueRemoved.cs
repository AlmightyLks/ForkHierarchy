using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class GitHubUserIndexUniqueRemoved : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GitHubUsers_Id",
                table: "GitHubUsers");

            migrationBuilder.DropColumn(
                name: "GHId",
                table: "GitHubUsers");

            migrationBuilder.CreateIndex(
                name: "IX_GitHubUsers_Id",
                table: "GitHubUsers",
                column: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GitHubUsers_Id",
                table: "GitHubUsers");

            migrationBuilder.AddColumn<int>(
                name: "GHId",
                table: "GitHubUsers",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubUsers_Id",
                table: "GitHubUsers",
                column: "Id",
                unique: true);
        }
    }
}
