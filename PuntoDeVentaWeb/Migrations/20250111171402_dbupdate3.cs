using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PuntoDeVentaWeb.Migrations
{
    /// <inheritdoc />
    public partial class dbupdate3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetail_Products_ProductId",
                table: "PurchaseDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetail_Purchases_PurchaseId",
                table: "PurchaseDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleDetail_Products_ProductId",
                table: "SaleDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleDetail_Sales_SaleId",
                table: "SaleDetail");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRole_UserRoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaleDetail",
                table: "SaleDetail");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseDetail",
                table: "PurchaseDetail");

            migrationBuilder.RenameTable(
                name: "UserRole",
                newName: "UserRoles");

            migrationBuilder.RenameTable(
                name: "SaleDetail",
                newName: "SaleDetails");

            migrationBuilder.RenameTable(
                name: "PurchaseDetail",
                newName: "PurchaseDetails");

            migrationBuilder.RenameIndex(
                name: "IX_SaleDetail_SaleId",
                table: "SaleDetails",
                newName: "IX_SaleDetails_SaleId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleDetail_ProductId",
                table: "SaleDetails",
                newName: "IX_SaleDetails_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseDetail_PurchaseId",
                table: "PurchaseDetails",
                newName: "IX_PurchaseDetails_PurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseDetail_ProductId",
                table: "PurchaseDetails",
                newName: "IX_PurchaseDetails_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaleDetails",
                table: "SaleDetails",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseDetails",
                table: "PurchaseDetails",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Products_ProductId",
                table: "PurchaseDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetails_Purchases_PurchaseId",
                table: "PurchaseDetails",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleDetails_Products_ProductId",
                table: "SaleDetails",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleDetails_Sales_SaleId",
                table: "SaleDetails",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRoles_UserRoleId",
                table: "Users",
                column: "UserRoleId",
                principalTable: "UserRoles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Products_ProductId",
                table: "PurchaseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_PurchaseDetails_Purchases_PurchaseId",
                table: "PurchaseDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleDetails_Products_ProductId",
                table: "SaleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_SaleDetails_Sales_SaleId",
                table: "SaleDetails");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_UserRoles_UserRoleId",
                table: "Users");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserRoles",
                table: "UserRoles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_SaleDetails",
                table: "SaleDetails");

            migrationBuilder.DropPrimaryKey(
                name: "PK_PurchaseDetails",
                table: "PurchaseDetails");

            migrationBuilder.RenameTable(
                name: "UserRoles",
                newName: "UserRole");

            migrationBuilder.RenameTable(
                name: "SaleDetails",
                newName: "SaleDetail");

            migrationBuilder.RenameTable(
                name: "PurchaseDetails",
                newName: "PurchaseDetail");

            migrationBuilder.RenameIndex(
                name: "IX_SaleDetails_SaleId",
                table: "SaleDetail",
                newName: "IX_SaleDetail_SaleId");

            migrationBuilder.RenameIndex(
                name: "IX_SaleDetails_ProductId",
                table: "SaleDetail",
                newName: "IX_SaleDetail_ProductId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseDetails_PurchaseId",
                table: "PurchaseDetail",
                newName: "IX_PurchaseDetail_PurchaseId");

            migrationBuilder.RenameIndex(
                name: "IX_PurchaseDetails_ProductId",
                table: "PurchaseDetail",
                newName: "IX_PurchaseDetail_ProductId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserRole",
                table: "UserRole",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_SaleDetail",
                table: "SaleDetail",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_PurchaseDetail",
                table: "PurchaseDetail",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetail_Products_ProductId",
                table: "PurchaseDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PurchaseDetail_Purchases_PurchaseId",
                table: "PurchaseDetail",
                column: "PurchaseId",
                principalTable: "Purchases",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleDetail_Products_ProductId",
                table: "SaleDetail",
                column: "ProductId",
                principalTable: "Products",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SaleDetail_Sales_SaleId",
                table: "SaleDetail",
                column: "SaleId",
                principalTable: "Sales",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_UserRole_UserRoleId",
                table: "Users",
                column: "UserRoleId",
                principalTable: "UserRole",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
