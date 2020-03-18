using Microsoft.EntityFrameworkCore.Migrations;

namespace Repair.Migrations
{
    public partial class update_repairMan : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityIds",
                table: "RepairMan");

            migrationBuilder.AddColumn<int>(
                name: "CommunityId",
                table: "RepairMan",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "CommunityName",
                table: "RepairMan",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "RepairMan");

            migrationBuilder.DropColumn(
                name: "CommunityName",
                table: "RepairMan");

            migrationBuilder.AddColumn<string>(
                name: "CommunityIds",
                table: "RepairMan",
                type: "varchar(20) CHARACTER SET utf8mb4",
                maxLength: 20,
                nullable: true);
        }
    }
}
