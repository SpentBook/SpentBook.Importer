using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace SpentBook.Migrations.SqlServer.Migrations
{
    public partial class MIGRATE_20200308_2238 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Transactions_BankName_Date_Name_Value",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BankName",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FormatFile",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "IdExternal",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "AccountAgency",
                table: "Transactions",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "AccountId",
                table: "Transactions",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "BankId",
                table: "Transactions",
                maxLength: 10,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "CheckNum",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "FitId",
                table: "Transactions",
                nullable: true);

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Transactions_BankId_AccountAgency_AccountId_Date_Name_Value",
                table: "Transactions",
                columns: new[] { "BankId", "AccountAgency", "AccountId", "Date", "Name", "Value" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropUniqueConstraint(
                name: "AK_Transactions_BankId_AccountAgency_AccountId_Date_Name_Value",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AccountAgency",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "AccountId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "BankId",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "CheckNum",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "FitId",
                table: "Transactions");

            migrationBuilder.AddColumn<string>(
                name: "BankName",
                table: "Transactions",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "FormatFile",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "IdExternal",
                table: "Transactions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Transactions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddUniqueConstraint(
                name: "AK_Transactions_BankName_Date_Name_Value",
                table: "Transactions",
                columns: new[] { "BankName", "Date", "Name", "Value" });
        }
    }
}
