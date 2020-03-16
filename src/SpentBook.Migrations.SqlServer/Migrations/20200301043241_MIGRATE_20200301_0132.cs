using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpentBook.Migrations.SqlServer.Migrations
{
    public partial class MIGRATE_20200301_0132 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdImport = table.Column<Guid>(nullable: false),
                    UserId = table.Column<Guid>(nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    SubCategory = table.Column<string>(nullable: true),
                    BankName = table.Column<string>(nullable: false),
                    FormatFile = table.Column<string>(nullable: true),
                    IdExternal = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.UniqueConstraint("AK_Transactions_BankName_Date_Name_Value", x => new { x.BankName, x.Date, x.Name, x.Value });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
