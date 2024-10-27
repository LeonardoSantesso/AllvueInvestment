using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Investment.DAL.Migrations
{
    /// <inheritdoc />
    public partial class InitialDatabase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "sale_record",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    total_cost = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total_sale_value = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    total_profit = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    cost_basis_per_sold_share = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_sale_record", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_lot",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    original_shares = table.Column<int>(type: "int", nullable: false),
                    shares = table.Column<int>(type: "int", nullable: false),
                    price_per_share = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    purchase_date = table.Column<DateTime>(type: "datetime", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_lot", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "stock_lot_sale",
                columns: table => new
                {
                    id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    sale_record_id = table.Column<int>(type: "int", nullable: false),
                    stock_lot_id = table.Column<int>(type: "int", nullable: false),
                    shares_sold = table.Column<int>(type: "int", nullable: false),
                    cost_basis = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_stock_lot_sale", x => x.id);
                    table.ForeignKey(
                        name: "FK_stock_lot_sale_sale_record_sale_record_id",
                        column: x => x.sale_record_id,
                        principalTable: "sale_record",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_stock_lot_sale_stock_lot_stock_lot_id",
                        column: x => x.stock_lot_id,
                        principalTable: "stock_lot",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "stock_lot",
                columns: new[] { "id", "original_shares", "price_per_share", "purchase_date", "shares" },
                values: new object[,]
                {
                    { 1, 100, 20m, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 100 },
                    { 2, 150, 30m, new DateTime(2024, 2, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 150 },
                    { 3, 120, 10m, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 120 }
                });

            migrationBuilder.CreateIndex(
                name: "IX_stock_lot_sale_sale_record_id",
                table: "stock_lot_sale",
                column: "sale_record_id");

            migrationBuilder.CreateIndex(
                name: "IX_stock_lot_sale_stock_lot_id",
                table: "stock_lot_sale",
                column: "stock_lot_id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "stock_lot_sale");

            migrationBuilder.DropTable(
                name: "sale_record");

            migrationBuilder.DropTable(
                name: "stock_lot");
        }
    }
}
