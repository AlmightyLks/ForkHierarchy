using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class RemoveGitHubRepositorySelfReferencingCascading : Migration
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_ParentId",
                table: "GitHubRepositories");

            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_SourceId",
                table: "GitHubRepositories");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_ParentId",
                table: "GitHubRepositories",
                column: "ParentId",
                principalTable: "GitHubRepositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubRepositories_SourceId",
                table: "GitHubRepositories",
                column: "SourceId",
                principalTable: "GitHubRepositories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
