using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace NexusERP.Migrations
{
    /// <inheritdoc />
    public partial class AddHasCommentcolumntoOrder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HasComment",
                table: "Orders",
                type: "tinyint(1)",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HasComment",
                table: "Orders");
        }
    }
}
