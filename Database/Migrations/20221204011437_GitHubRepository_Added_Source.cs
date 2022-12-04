using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class GitHubRepositoryAddedSource : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SourceId",
                table: "GitHubRepositories",
                type: "INTEGER",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_SourceId",
                table: "GitHubRepositories",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_SourceId",
                table: "GitHubRepositories",
                column: "SourceId",
                principalTable: "GitHubRepositories",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_SourceId",
                table: "GitHubRepositories");

            migrationBuilder.DropIndex(
                name: "IX_GitHubRepositories_SourceId",
                table: "GitHubRepositories");

            migrationBuilder.DropColumn(
                name: "SourceId",
                table: "GitHubRepositories");
        }
    }
}
