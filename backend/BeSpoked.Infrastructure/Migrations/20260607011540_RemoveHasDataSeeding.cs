using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace BeSpoked.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RemoveHasDataSeeding : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Discounts",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Sales",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Salespersons",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 4);

            migrationBuilder.DeleteData(
                table: "Products",
                keyColumn: "Id",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "Salespersons",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Salespersons",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Salespersons",
                keyColumn: "Id",
                keyValue: 3);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Address", "FirstName", "LastName", "Phone", "StartDate" },
                values: new object[,]
                {
                    { 1, "1 River St, Austin TX", "Tom", "Walker", "512-200-0001", new DateTime(2022, 1, 5, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 2, "9 Summit Dr, Denver CO", "Lisa", "Park", "303-200-0002", new DateTime(2022, 3, 14, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 3, "44 Creek Ln, Portland OR", "Carlos", "Mendez", "503-200-0003", new DateTime(2023, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified) },
                    { 4, "77 Lake Ave, Seattle WA", "Nina", "Patel", "206-200-0004", new DateTime(2021, 11, 20, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Products",
                columns: new[] { "Id", "CommissionPercentage", "Manufacturer", "Name", "PurchasePrice", "QtyOnHand", "SalePrice", "Style" },
                values: new object[,]
                {
                    { 1, 5m, "Trek", "Apex Racer", 800m, 10, 1499m, "Road" },
                    { 2, 6m, "Specialized", "TrailBlazer Pro", 1200m, 7, 2199m, "Mountain" },
                    { 3, 4m, "Cannondale", "UrbanGlide 7", 500m, 15, 899m, "Hybrid" },
                    { 4, 5.5m, "Trek", "GravelKing X", 950m, 5, 1799m, "Gravel" },
                    { 5, 7m, "Giant", "SpeedDemon 900", 1500m, 3, 2799m, "Road" }
                });

            migrationBuilder.InsertData(
                table: "Salespersons",
                columns: new[] { "Id", "Address", "FirstName", "LastName", "Manager", "Phone", "StartDate", "TerminationDate" },
                values: new object[,]
                {
                    { 1, "12 Elm St, Austin TX", "Alice", "Chen", "Bob Kraft", "512-111-0001", new DateTime(2020, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 2, "88 Oak Ave, Denver CO", "Marcus", "Reyes", "Bob Kraft", "303-111-0002", new DateTime(2019, 7, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 3, "5 Maple Rd, Portland OR", "Priya", "Sharma", "Bob Kraft", "503-111-0003", new DateTime(2021, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), null },
                    { 4, "200 Pine Blvd, Seattle WA", "Jake", "Novak", "Bob Kraft", "206-111-0004", new DateTime(2018, 5, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 12, 31, 0, 0, 0, 0, DateTimeKind.Unspecified) }
                });

            migrationBuilder.InsertData(
                table: "Discounts",
                columns: new[] { "Id", "BeginDate", "DiscountPercentage", "EndDate", "ProductId" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 10m, new DateTime(2024, 1, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, new DateTime(2024, 3, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 15m, new DateTime(2024, 3, 31, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 3, new DateTime(2024, 6, 1, 0, 0, 0, 0, DateTimeKind.Unspecified), 8m, new DateTime(2024, 6, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 }
                });

            migrationBuilder.InsertData(
                table: "Sales",
                columns: new[] { "Id", "CommissionPercentage", "CustomerId", "DiscountApplied", "ProductId", "SalePrice", "SalesDate", "SalespersonId" },
                values: new object[,]
                {
                    { 1, 5m, 1, 10m, 1, 1499m, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 2, 6m, 2, 0m, 2, 2199m, new DateTime(2024, 2, 14, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 3, 4m, 3, 15m, 3, 899m, new DateTime(2024, 3, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1 },
                    { 4, 5.5m, 4, 0m, 4, 1799m, new DateTime(2024, 4, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 },
                    { 5, 7m, 1, 0m, 5, 2799m, new DateTime(2024, 5, 3, 0, 0, 0, 0, DateTimeKind.Unspecified), 2 },
                    { 6, 5m, 2, 8m, 1, 1499m, new DateTime(2024, 6, 18, 0, 0, 0, 0, DateTimeKind.Unspecified), 3 }
                });
        }
    }
}
