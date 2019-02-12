using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CA2_Ultima.Data.Migrations
{
    public partial class CreateCommentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<int>(
                name: "Id",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(string))
                .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);

            migrationBuilder.AddColumn<string>(
                name: "Content",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TalentId",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TimeStamp",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedById",
                table: "Comments",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UploadedByName",
                table: "Comments",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Content",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TalentId",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "TimeStamp",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UploadedById",
                table: "Comments");

            migrationBuilder.DropColumn(
                name: "UploadedByName",
                table: "Comments");

            migrationBuilder.AlterColumn<string>(
                name: "Id",
                table: "Comments",
                nullable: false,
                oldClrType: typeof(int))
                .OldAnnotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn);
        }
    }
}
