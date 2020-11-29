using Microsoft.EntityFrameworkCore.Migrations;

namespace DatabaseConnection.Migrations
{
    public partial class CustomerEmailAsKeyAndFirstANdLastnameAdded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerName",
                table: "Sales");

            migrationBuilder.RenameColumn(
                name: "CustomerName",
                table: "Sales",
                newName: "CustomerUserEmail");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_CustomerName",
                table: "Sales",
                newName: "IX_Sales_CustomerUserEmail");

            migrationBuilder.RenameColumn(
                name: "Name",
                table: "Customers",
                newName: "UserEmail");

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Customers",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerUserEmail",
                table: "Sales",
                column: "CustomerUserEmail",
                principalTable: "Customers",
                principalColumn: "UserEmail",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Sales_Customers_CustomerUserEmail",
                table: "Sales");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "CustomerUserEmail",
                table: "Sales",
                newName: "CustomerName");

            migrationBuilder.RenameIndex(
                name: "IX_Sales_CustomerUserEmail",
                table: "Sales",
                newName: "IX_Sales_CustomerName");

            migrationBuilder.RenameColumn(
                name: "UserEmail",
                table: "Customers",
                newName: "Name");

            migrationBuilder.AddForeignKey(
                name: "FK_Sales_Customers_CustomerName",
                table: "Sales",
                column: "CustomerName",
                principalTable: "Customers",
                principalColumn: "Name",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
