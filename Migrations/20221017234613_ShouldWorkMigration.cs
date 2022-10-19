using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CS3750_PlanetExpressLMS.Migrations
{
    public partial class ShouldWorkMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Invoice",
                columns: table => new
                {
                    InvoiceId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID = table.Column<int>(nullable: false),
                    FullBalance = table.Column<decimal>(nullable: false),
                    AmountPaid = table.Column<decimal>(nullable: false),
                    PaymentDate = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Invoice", x => x.InvoiceId);
                });

            migrationBuilder.CreateTable(
                name: "Payment",
                columns: table => new
                {
                    PaymentID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID = table.Column<int>(nullable: false),
                    FirstName = table.Column<string>(maxLength: 20, nullable: false),
                    LastName = table.Column<string>(maxLength: 20, nullable: false),
                    CardNumber = table.Column<string>(maxLength: 16, nullable: false),
                    ExpDate = table.Column<DateTime>(nullable: false),
                    Cvv = table.Column<string>(maxLength: 3, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Payment", x => x.PaymentID);
                });

            migrationBuilder.CreateTable(
                name: "Submission",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Path = table.Column<string>(nullable: false),
                    UserID = table.Column<int>(nullable: false),
                    AssignmentID = table.Column<int>(nullable: false),
                    SubmissionTime = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Submission", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Invoice");

            migrationBuilder.DropTable(
                name: "Payment");

            migrationBuilder.DropTable(
                name: "Submission");
        }
    }
}
