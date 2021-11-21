using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeRates.Migrations
{
    public partial class AddVisitorIp : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Ip",
                table: "VisitorStatistics",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Ip",
                table: "VisitorStatistics");
        }
    }
}
