using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ExplicitIndex : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateIndex(
                name: "IX_GitHubUsers_Id",
                table: "GitHubUsers",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_Id",
                table: "GitHubRepositories",
                column: "Id",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_GitHubUsers_Id",
                table: "GitHubUsers");

            migrationBuilder.DropIndex(
                name: "IX_GitHubRepositories_Id",
                table: "GitHubRepositories");
        }
    }
}
