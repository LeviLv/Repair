using Microsoft.EntityFrameworkCore.Migrations;

namespace Repair.Migrations
{
    public partial class update_repairManname : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommunityName",
                table: "RepairMan",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityName",
                table: "RepairMan");
        }
    }
}
