using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace ProjektAPBDs28016.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AppUser",
                columns: table => new
                {
                    Login = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Password = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Role = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Salt = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenExp = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("AppUser_pk", x => x.Login);
                });

            migrationBuilder.CreateTable(
                name: "CompanyClient",
                columns: table => new
                {
                    IdCompanyClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    KrsNumber = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Address = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("CompanyClient_pk", x => x.IdCompanyClient);
                });

            migrationBuilder.CreateTable(
                name: "Discount",
                columns: table => new
                {
                    IdDiscount = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Amount = table.Column<int>(type: "int", nullable: false),
                    DateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    DateTo = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Discount_pk", x => x.IdDiscount);
                });

            migrationBuilder.CreateTable(
                name: "PhysicalClient",
                columns: table => new
                {
                    IdPhysicalClient = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Pesel = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PhoneNumber = table.Column<int>(type: "int", nullable: false),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PhysicalClient_pk", x => x.IdPhysicalClient);
                });

            migrationBuilder.CreateTable(
                name: "Product",
                columns: table => new
                {
                    IdProduct = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    VersionInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Category = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Price = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    IdDiscount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Product_pk", x => x.IdProduct);
                });

            migrationBuilder.CreateTable(
                name: "Agreement",
                columns: table => new
                {
                    IdAgreement = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdClient = table.Column<int>(type: "int", nullable: false),
                    ClientType = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IdProduct = table.Column<int>(type: "int", nullable: false),
                    ProductVersionInfo = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AgreementDateFrom = table.Column<DateTime>(type: "datetime2", nullable: false),
                    AgreementDateTo = table.Column<DateTime>(type: "datetime2", nullable: false),
                    CalculatedPrice = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    ProductUpdatesToInYears = table.Column<int>(type: "int", nullable: false),
                    IsSigned = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Agreement_pk", x => x.IdAgreement);
                    table.ForeignKey(
                        name: "Agreement_Product_fk",
                        column: x => x.IdProduct,
                        principalTable: "Product",
                        principalColumn: "IdProduct",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Product_Discount",
                columns: table => new
                {
                    IdProduct = table.Column<int>(type: "int", nullable: false),
                    IdDiscount = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("ProductDiscount_pk", x => new { x.IdProduct, x.IdDiscount });
                    table.ForeignKey(
                        name: "ProductDiscount_Discount_fk",
                        column: x => x.IdDiscount,
                        principalTable: "Discount",
                        principalColumn: "IdDiscount",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "ProductDiscount_Product_fk",
                        column: x => x.IdProduct,
                        principalTable: "Product",
                        principalColumn: "IdProduct",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    IdPayment = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdAgreement = table.Column<int>(type: "int", nullable: false),
                    MoneyOwed = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    MoneyPaid = table.Column<decimal>(type: "decimal(18,2)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("Payment_pk", x => x.IdPayment);
                    table.ForeignKey(
                        name: "Payment_Agreement_fk",
                        column: x => x.IdAgreement,
                        principalTable: "Agreement",
                        principalColumn: "IdAgreement",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "AppUser",
                columns: new[] { "Login", "Password", "RefreshToken", "RefreshTokenExp", "Role", "Salt" },
                values: new object[,]
                {
                    { "admin", "RCIeotM4kTMfWDu3bn72qgTswxobbFllt8sP1Mm5U1s=", "OrXQIUPI69QtD5KVgI7QrFQHnwfs+8Q9mECKsLKmn80=", new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "admin", "fUYAuxqW54H8a69VlzikJg==" },
                    { "worker1", "RCIeotM4kTMfWDu3bn72qgTswxobbFllt8sP1Mm5U1s=", "OrXQIUPI69QtD5KVgI7QrFQHnwfs+8Q9mECKsLKmn80=", new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "regular", "fUYAuxqW54H8a69VlzikJg==" },
                    { "worker2", "RCIeotM4kTMfWDu3bn72qgTswxobbFllt8sP1Mm5U1s=", "OrXQIUPI69QtD5KVgI7QrFQHnwfs+8Q9mECKsLKmn80=", new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "asda231dasda", "fUYAuxqW54H8a69VlzikJg==" }
                });

            migrationBuilder.InsertData(
                table: "CompanyClient",
                columns: new[] { "IdCompanyClient", "Address", "Email", "KrsNumber", "PhoneNumber" },
                values: new object[,]
                {
                    { 1, "Koszykowa 13, Warszawa", "weloveit@weloveit.pl", "113021392", 696784867 },
                    { 2, "Kwiatowa 10, Warszawa", "itforu@itforu.pl", "443051392", 539981155 }
                });

            migrationBuilder.InsertData(
                table: "Discount",
                columns: new[] { "IdDiscount", "Amount", "DateFrom", "DateTo", "Name" },
                values: new object[,]
                {
                    { 1, 10, new DateTime(2023, 12, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2028, 12, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "General discount" },
                    { 2, 15, new DateTime(2020, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2030, 8, 24, 0, 0, 0, 0, DateTimeKind.Unspecified), "Birthday discount" }
                });

            migrationBuilder.InsertData(
                table: "PhysicalClient",
                columns: new[] { "IdPhysicalClient", "Email", "IsDeleted", "Name", "Pesel", "PhoneNumber", "Surname" },
                values: new object[,]
                {
                    { 1, "jan@kowalski.pl", false, "Jan", "99292006932", 778123432, "Kowalski" },
                    { 2, "piotr@piotrkowski.pl", false, "Piotr", "65292076931", 666999333, "Piotrkowski" }
                });

            migrationBuilder.InsertData(
                table: "Product",
                columns: new[] { "IdProduct", "Category", "Description", "IdDiscount", "Name", "Price", "VersionInfo" },
                values: new object[,]
                {
                    { 1, "Software", "Python IDE (Pycharm) by Jetbrains", 1, "Pycharm", 5000m, "1.2" },
                    { 2, "Software", "Java IDE (Intellij) by Jetbrains", 1, "Intellij", 2500m, "15.4" },
                    { 3, "Software", "C# IDE (Rider) by Jetbrains", 1, "Rider", 6500m, "10.9" }
                });

            migrationBuilder.InsertData(
                table: "Agreement",
                columns: new[] { "IdAgreement", "AgreementDateFrom", "AgreementDateTo", "CalculatedPrice", "ClientType", "IdClient", "IdProduct", "IsSigned", "ProductUpdatesToInYears", "ProductVersionInfo" },
                values: new object[,]
                {
                    { 1, new DateTime(2024, 8, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2024, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 3400m, "physical", 1, 2, false, 3, "10.9" },
                    { 2, new DateTime(2023, 8, 16, 0, 0, 0, 0, DateTimeKind.Unspecified), new DateTime(2023, 8, 30, 0, 0, 0, 0, DateTimeKind.Unspecified), 3400m, "physical", 2, 2, false, 3, "10.9" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Agreement_IdProduct",
                table: "Agreement",
                column: "IdProduct");

            migrationBuilder.CreateIndex(
                name: "IX_Payment_IdAgreement",
                table: "Payment",
                column: "IdAgreement");

            migrationBuilder.CreateIndex(
                name: "IX_Product_Discount_IdDiscount",
                table: "Product_Discount",
                column: "IdDiscount");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AppUser");

            migrationBuilder.DropTable(
                name: "CompanyClient");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "PhysicalClient");

            migrationBuilder.DropTable(
                name: "Product_Discount");

            migrationBuilder.DropTable(
                name: "Agreement");

            migrationBuilder.DropTable(
                name: "Discount");

            migrationBuilder.DropTable(
                name: "Product");
        }
    }
}
