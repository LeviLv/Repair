using Microsoft.EntityFrameworkCore.Migrations;

namespace Repair.Migrations
{
    public partial class addCommunityId : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CommunityId",
                table: "Users",
                maxLength: 20,
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CommunityId",
                table: "Users");
        }
    }
}
