using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemovedGitHubRepositoryChildren : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_GitHubRepositoryId",
                table: "GitHubRepositories");

            migrationBuilder.DropIndex(
                name: "IX_GitHubRepositories_GitHubRepositoryId",
                table: "GitHubRepositories");

            migrationBuilder.DropColumn(
                name: "GitHubRepositoryId",
                table: "GitHubRepositories");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "GitHubRepositoryId",
                table: "GitHubRepositories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_GitHubRepositoryId",
                table: "GitHubRepositories",
                column: "GitHubRepositoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_GitHubRepositoryId",
                table: "GitHubRepositories",
                column: "GitHubRepositoryId",
                principalTable: "GitHubRepositories",
                principalColumn: "Id");
        }
    }
}
