using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ExpenseTracker.Migrations
{
    /// <inheritdoc />
    public partial class AddVatSupport : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Amount",
                table: "Expenses",
                newName: "VatAmount");

            migrationBuilder.AddColumn<decimal>(
                name: "NetAmount",
                table: "Expenses",
                type: "numeric",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "VatRateId",
                table: "Expenses",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateTable(
                name: "VatRates",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Rate = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VatRates", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "VatRates",
                columns: new[] { "Id", "Name", "Rate" },
                values: new object[,]
                {
                    { 1, "Standard Rate", 20m },
                    { 2, "Reduced Rate", 5m },
                    { 3, "Zero Rate", 0m }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Expenses_VatRateId",
                table: "Expenses",
                column: "VatRateId");

            migrationBuilder.AddForeignKey(
                name: "FK_Expenses_VatRates_VatRateId",
                table: "Expenses",
                column: "VatRateId",
                principalTable: "VatRates",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Expenses_VatRates_VatRateId",
                table: "Expenses");

            migrationBuilder.DropTable(
                name: "VatRates");

            migrationBuilder.DropIndex(
                name: "IX_Expenses_VatRateId",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "NetAmount",
                table: "Expenses");

            migrationBuilder.DropColumn(
                name: "VatRateId",
                table: "Expenses");

            migrationBuilder.RenameColumn(
                name: "VatAmount",
                table: "Expenses",
                newName: "Amount");
        }
    }
}
