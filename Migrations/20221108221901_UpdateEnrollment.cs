using Microsoft.EntityFrameworkCore.Migrations;

namespace CS3750_PlanetExpressLMS.Migrations
{
    public partial class UpdateEnrollment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "CumulativeGrade",
                table: "Enrollment");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPointsEarned",
                table: "Enrollment",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalPointsPossible",
                table: "Enrollment",
                nullable: false,
                defaultValue: 0m);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalPointsEarned",
                table: "Enrollment");

            migrationBuilder.DropColumn(
                name: "TotalPointsPossible",
                table: "Enrollment");

            migrationBuilder.AddColumn<decimal>(
                name: "CumulativeGrade",
                table: "Enrollment",
                type: "decimal(18,2)",
                nullable: true);
        }
    }
}
