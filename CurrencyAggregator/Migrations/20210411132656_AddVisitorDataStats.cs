using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CurrencyAggregator.Migrations
{
    public partial class AddVisitorDataStats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VisitorStatistics",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Created = table.Column<DateTime>(nullable: false),
                    LastVisit = table.Column<DateTime>(nullable: false),
                    Count = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VisitorStatistics", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FingerPrintData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    c = table.Column<int>(nullable: true),
                    d = table.Column<DateTime>(nullable: true),
                    i = table.Column<DateTime>(nullable: true),
                    VisitorDataItemId = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FingerPrintData", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FingerPrintData_VisitorStatistics_VisitorDataItemId",
                        column: x => x.VisitorDataItemId,
                        principalTable: "VisitorStatistics",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FingerPrintData_VisitorDataItemId",
                table: "FingerPrintData",
                column: "VisitorDataItemId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FingerPrintData");

            migrationBuilder.DropTable(
                name: "VisitorStatistics");
        }
    }
}
