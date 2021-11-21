using Microsoft.EntityFrameworkCore.Migrations;

namespace ExchangeRates.Migrations
{
    public partial class AddPreparedData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
/*            migrationBuilder.DropColumn(
                name: "Key",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "Items",
                nullable: true);
*/
            migrationBuilder.CreateTable(
                name: "PreparedData",
                columns: table => new
                {
                    Id = table.Column<string>(nullable: false),
                    Key = table.Column<string>(nullable: true),
                    Data = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PreparedData", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PreparedData");

/*            migrationBuilder.DropColumn(
                name: "Name",
                table: "Items");

            migrationBuilder.AddColumn<string>(
                name: "Key",
                table: "Items",
                type: "nvarchar(max)",
                nullable: true);
*/        }
    }
}
