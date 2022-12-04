using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Database.Migrations
{
    /// <inheritdoc />
    public partial class GitHubUserAddedTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubUser_OwnerId",
                table: "GitHubRepositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GitHubUser",
                table: "GitHubUser");

            migrationBuilder.RenameTable(
                name: "GitHubUser",
                newName: "GitHubUsers");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GitHubUsers",
                type: "TEXT",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "TEXT");

            migrationBuilder.AddPrimaryKey(
                name: "PK_GitHubUsers",
                table: "GitHubUsers",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubUsers_OwnerId",
                table: "GitHubRepositories",
                column: "OwnerId",
                principalTable: "GitHubUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_GitHubRepositories_GitHubUsers_OwnerId",
                table: "GitHubRepositories");

            migrationBuilder.DropPrimaryKey(
                name: "PK_GitHubUsers",
                table: "GitHubUsers");

            migrationBuilder.RenameTable(
                name: "GitHubUsers",
                newName: "GitHubUser");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "GitHubUser",
                type: "TEXT",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "TEXT",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_GitHubUser",
                table: "GitHubUser",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_GitHubRepositories_GitHubUser_OwnerId",
                table: "GitHubRepositories",
                column: "OwnerId",
                principalTable: "GitHubUser",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
