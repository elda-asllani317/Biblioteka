using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Biblioteka.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddRoleColumnToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Add Password column if it doesn't exist
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.Users', 'Password') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Users]
                    ADD [Password] NVARCHAR(MAX) NOT NULL DEFAULT '';
                END
            ");

            // Add Role column if it doesn't exist
            migrationBuilder.Sql(@"
                IF COL_LENGTH('dbo.Users', 'Role') IS NULL
                BEGIN
                    ALTER TABLE [dbo].[Users]
                    ADD [Role] NVARCHAR(50) NOT NULL DEFAULT 'User';
                END
            ");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Publishers",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_Email",
                table: "Publishers",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Publishers_Name",
                table: "Publishers",
                column: "Name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Publishers_Email",
                table: "Publishers");

            migrationBuilder.DropIndex(
                name: "IX_Publishers_Name",
                table: "Publishers");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Role",
                table: "Users");

            migrationBuilder.AlterColumn<string>(
                name: "Email",
                table: "Publishers",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(255)",
                oldMaxLength: 255);
        }
    }
}
