using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class moreFixes : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Movies_MovieId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Customers",
                newName: "FirstName");

            migrationBuilder.AlterColumn<int>(
                name: "MovieId",
                table: "Sales",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Movies_MovieId",
                table: "Sales",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Movies_MovieId",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "FirstName",
                table: "Customers",
                newName: "Name");

            migrationBuilder.AlterColumn<int>(
                name: "MovieId",
                table: "Sales",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Movies_MovieId",
                table: "Sales",
                column: "MovieId",
                principalTable: "Movies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
