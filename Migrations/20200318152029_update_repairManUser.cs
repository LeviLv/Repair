using Microsoft.EntityFrameworkCore.Migrations;

namespace Repair.Migrations
{
    public partial class update_repairManUser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityName",
                table: "RepairMan");

            migrationBuilder.AddColumn<string>(
                name: "RepairManName",
                table: "RepairMan",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RepairManName",
                table: "RepairMan");

            migrationBuilder.AddColumn<string>(
                name: "CommunityName",
                table: "RepairMan",
                type: "longtext CHARACTER SET utf8mb4",
                nullable: true);
        }
    }
}
