using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CS3750_PlanetExpressLMS.Migrations
{
    public partial class Assignment : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Assignment",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(nullable: false),
                    SubmissionType = table.Column<string>(maxLength: 15, nullable: false),
                    PointsPossible = table.Column<int>(nullable: false),
                    OpenDateTime = table.Column<DateTime>(nullable: false),
                    CloseDateTime = table.Column<DateTime>(nullable: false),
                    CourseID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Assignment", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Assignment");
        }
    }
}
