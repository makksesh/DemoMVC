using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DemoMVC.Migrations
{
    public partial class AddFieldMeasurment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Measurements_UnitOfMeasureId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "UnitOfMeasureId",
                table: "Products",
                newName: "MeasurementId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_UnitOfMeasureId",
                table: "Products",
                newName: "IX_Products_MeasurementId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Measurements_MeasurementId",
                table: "Products",
                column: "MeasurementId",
                principalTable: "Measurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Products_Measurements_MeasurementId",
                table: "Products");

            migrationBuilder.RenameColumn(
                name: "MeasurementId",
                table: "Products",
                newName: "UnitOfMeasureId");

            migrationBuilder.RenameIndex(
                name: "IX_Products_MeasurementId",
                table: "Products",
                newName: "IX_Products_UnitOfMeasureId");

            migrationBuilder.AddForeignKey(
                name: "FK_Products_Measurements_UnitOfMeasureId",
                table: "Products",
                column: "UnitOfMeasureId",
                principalTable: "Measurements",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
