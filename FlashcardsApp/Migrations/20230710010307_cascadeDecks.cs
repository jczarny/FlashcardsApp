using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class cascadeDecks : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RevisionLog_Decks_DeckId",
                table: "RevisionLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDecks_Decks_DeckId",
                table: "UserDecks");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RefreshToken",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "RefreshToken",
                value: null);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "RefreshToken",
                value: null);

            migrationBuilder.AddForeignKey(
                name: "FK_RevisionLog_Decks_DeckId",
                table: "RevisionLog",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_UserDecks_Decks_DeckId",
                table: "UserDecks",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RevisionLog_Decks_DeckId",
                table: "RevisionLog");

            migrationBuilder.DropForeignKey(
                name: "FK_UserDecks_Decks_DeckId",
                table: "UserDecks");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "RefreshToken",
                value: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "RefreshToken",
                value: "");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 3,
                column: "RefreshToken",
                value: "");

            migrationBuilder.AddForeignKey(
                name: "FK_RevisionLog_Decks_DeckId",
                table: "RevisionLog",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_UserDecks_Decks_DeckId",
                table: "UserDecks",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id");
        }
    }
}
