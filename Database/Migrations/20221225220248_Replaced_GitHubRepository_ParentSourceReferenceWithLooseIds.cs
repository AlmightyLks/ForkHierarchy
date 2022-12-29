using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class ReplacedGitHubRepositoryParentSourceReferenceWithLooseIds : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_ParentId",
                table: "GitHubRepositories");

            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_SourceId",
                table: "GitHubRepositories");

            migrationBuilder.DropIndex(
                name: "IX_GitHubRepositories_ParentId",
                table: "GitHubRepositories");

            migrationBuilder.DropIndex(
                name: "IX_GitHubRepositories_SourceId",
                table: "GitHubRepositories");

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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
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

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_ParentId",
                table: "GitHubRepositories",
                column: "ParentId");

            migrationBuilder.CreateIndex(
                name: "IX_GitHubRepositories_SourceId",
                table: "GitHubRepositories",
                column: "SourceId");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_ParentId",
                table: "GitHubRepositories",
                column: "ParentId",
                principalTable: "GitHubRepositories",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_SourceId",
                table: "GitHubRepositories",
                column: "SourceId",
                principalTable: "GitHubRepositories",
                principalColumn: "Id");
        }
    }
}
