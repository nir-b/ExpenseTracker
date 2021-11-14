using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace ExpenseTracker.WebApi.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                "Expenses",
                table => new
                {
                    Id = table.Column<int>()
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Title = table.Column<string>(maxLength: 512, nullable: true),
                    Category = table.Column<string>(maxLength: 128, nullable: true),
                    ExpenseDate = table.Column<DateTime>(),
                    Amount = table.Column<decimal>("decimal(8, 2)"),
                    Description = table.Column<string>(maxLength: 1024, nullable: true),
                    DateAdded = table.Column<DateTime>(),
                    DateUpdated = table.Column<DateTime>(),
                    DateDeleted = table.Column<DateTime>(nullable: true)
                },
                constraints: table => { table.PrimaryKey("PK_Expenses", x => x.Id); });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                "Expenses");
        }
    }
}