using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace MySuperDuperPetProject.Migrations
{
    /// <inheritdoc />
    public partial class my_first_migration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "transfers");

            migrationBuilder.EnsureSchema(
                name: "users");

            migrationBuilder.CreateTable(
                name: "hub_transfers_statistic",
                schema: "transfers",
                columns: table => new
                {
                    HashId = table.Column<string>(type: "text", nullable: false),
                    From = table.Column<string>(type: "text", nullable: false),
                    To = table.Column<string>(type: "text", nullable: false),
                    Count = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hub_transfers_statistic", x => x.HashId);
                });

            migrationBuilder.CreateTable(
                name: "hub_users",
                schema: "users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hub_users", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "hub_transfers",
                schema: "transfers",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(type: "integer", nullable: false),
                    PageFrom = table.Column<string>(type: "text", nullable: false),
                    PageTo = table.Column<string>(type: "text", nullable: false),
                    TransferUTC = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_hub_transfers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_hub_transfers_hub_users_UserId",
                        column: x => x.UserId,
                        principalSchema: "users",
                        principalTable: "hub_users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_hub_transfers_UserId",
                schema: "transfers",
                table: "hub_transfers",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "hub_transfers",
                schema: "transfers");

            migrationBuilder.DropTable(
                name: "hub_transfers_statistic",
                schema: "transfers");

            migrationBuilder.DropTable(
                name: "hub_users",
                schema: "users");
        }
    }
}
