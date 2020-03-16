using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpentBook.Migrations.MySql.Migrations
{
    public partial class MIGRATE_20200308_2229 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    IdImport = table.Column<Guid>(nullable: false),
                    BankId = table.Column<string>(maxLength: 10, nullable: false),
                    AccountAgency = table.Column<string>(maxLength: 10, nullable: false),
                    AccountId = table.Column<string>(maxLength: 10, nullable: false),
                    Name = table.Column<string>(nullable: false),
                    Date = table.Column<DateTime>(nullable: false),
                    Value = table.Column<decimal>(nullable: false),
                    Category = table.Column<string>(nullable: true),
                    SubCategory = table.Column<string>(nullable: true),
                    CheckNum = table.Column<string>(nullable: true),
                    FitId = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Updated = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                    table.UniqueConstraint("AK_Transactions_BankId_AccountAgency_AccountId_Date_Name_Value", x => new { x.BankId, x.AccountAgency, x.AccountId, x.Date, x.Name, x.Value });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
