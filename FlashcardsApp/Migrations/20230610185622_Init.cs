using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FlashcardsApp.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false),
                    Password = table.Column<string>(type: "nvarchar(25)", maxLength: 25, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Decks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CreatorId = table.Column<int>(type: "int", nullable: true),
                    Title = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    IsPrivate = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Decks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Decks_Users_CreatorId",
                        column: x => x.CreatorId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Statistics",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalGuesses = table.Column<int>(type: "int", nullable: false),
                    CorrectGuesses = table.Column<int>(type: "int", nullable: false),
                    WrongGuesses = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Statistics", x => new { x.Date, x.UserId });
                    table.CheckConstraint("CK_TotalGuesses", "[TotalGuesses] = [CorrectGuesses] + [WrongGuesses]");
                    table.ForeignKey(
                        name: "FK_Statistics_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Cards",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeckId = table.Column<int>(type: "int", nullable: false),
                    Front = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Reverse = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cards", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Cards_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "UserDecks",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DeckId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDecks", x => new { x.UserId, x.DeckId });
                    table.ForeignKey(
                        name: "FK_UserDecks_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_UserDecks_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "RevisionLog",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    DeckId = table.Column<int>(type: "int", nullable: false),
                    CardId = table.Column<int>(type: "int", nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Ease = table.Column<int>(type: "int", nullable: false),
                    Stage = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RevisionLog", x => new { x.UserId, x.DeckId, x.CardId });
                    table.ForeignKey(
                        name: "FK_RevisionLog_Cards_CardId",
                        column: x => x.CardId,
                        principalTable: "Cards",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_RevisionLog_Decks_DeckId",
                        column: x => x.DeckId,
                        principalTable: "Decks",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_RevisionLog_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "Password", "Username" },
                values: new object[,]
                {
                    { 1, "baaaaaa", "Michal15" },
                    { 2, "lolllol", "Krzychu7" },
                    { 3, "admin", "admin" }
                });

            migrationBuilder.InsertData(
                table: "Decks",
                columns: new[] { "Id", "CreatorId", "Description", "IsPrivate", "Title" },
                values: new object[,]
                {
                    { 1, 3, "Test123", false, "Polish to English vocab" },
                    { 2, 3, "Test123", false, "English to Polish vocab" },
                    { 3, 3, "Test123", false, "German to English vocab" }
                });

            migrationBuilder.InsertData(
                table: "Statistics",
                columns: new[] { "Date", "UserId", "CorrectGuesses", "TotalGuesses", "WrongGuesses" },
                values: new object[,]
                {
                    { new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 0, 0, 0 },
                    { new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, 1, 1 },
                    { new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, 0, 0 },
                    { new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, 1, 1, 0 },
                    { new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 0, 0, 0 },
                    { new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 3, 0, 1, 1 }
                });

            migrationBuilder.InsertData(
                table: "Cards",
                columns: new[] { "Id", "DeckId", "Description", "Front", "Reverse" },
                values: new object[,]
                {
                    { 1, 1, "Meat is neat", "Kiełbasa", "Sausage" },
                    { 2, 1, "Sky is the limit", "Niebo", "Sky" },
                    { 3, 2, "Mięso jest spoko", "Sausage", "Kiełbasa" },
                    { 4, 2, "Limitem jest niebo", "Sky", "Niebo" },
                    { 5, 3, "Ja voll!", "Ja", "Yes" }
                });

            migrationBuilder.InsertData(
                table: "UserDecks",
                columns: new[] { "DeckId", "UserId" },
                values: new object[,]
                {
                    { 1, 1 },
                    { 1, 2 },
                    { 1, 3 },
                    { 2, 3 }
                });

            migrationBuilder.InsertData(
                table: "RevisionLog",
                columns: new[] { "CardId", "DeckId", "UserId", "Date", "Ease", "Stage" },
                values: new object[,]
                {
                    { 1, 1, 1, new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 1, 1, 2, new DateTime(2008, 5, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 },
                    { 2, 1, 3, new DateTime(2008, 5, 2, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, 1 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Cards_DeckId",
                table: "Cards",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Decks_CreatorId",
                table: "Decks",
                column: "CreatorId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionLog_CardId",
                table: "RevisionLog",
                column: "CardId");

            migrationBuilder.CreateIndex(
                name: "IX_RevisionLog_DeckId",
                table: "RevisionLog",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Statistics_UserId",
                table: "Statistics",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_UserDecks_DeckId",
                table: "UserDecks",
                column: "DeckId");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "RevisionLog");

            migrationBuilder.DropTable(
                name: "Statistics");

            migrationBuilder.DropTable(
                name: "UserDecks");

            migrationBuilder.DropTable(
                name: "Cards");

            migrationBuilder.DropTable(
                name: "Decks");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}
