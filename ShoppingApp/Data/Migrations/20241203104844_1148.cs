using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ShoppingApp.Data.Migrations
{
    /// <inheritdoc />
    public partial class _1148 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserID",
                table: "ShoppingList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_ShoppingList_UserID",
                table: "ShoppingList",
                column: "UserID");

            migrationBuilder.AddForeignKey(
                name: "FK_ShoppingList_AspNetUsers_UserID",
                table: "ShoppingList",
                column: "UserID",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_ShoppingList_AspNetUsers_UserID",
                table: "ShoppingList");

            migrationBuilder.DropIndex(
                name: "IX_ShoppingList_UserID",
                table: "ShoppingList");

            migrationBuilder.DropColumn(
                name: "UserID",
                table: "ShoppingList");
        }
    }
}
