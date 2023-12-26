using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace LoanService.Migrations
{
    /// <inheritdoc />
    public partial class AddLoanOffering : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LoanOfferings",
                columns: table => new
                {
                    Name = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    Months = table.Column<int>(type: "integer", nullable: false),
                    InterestRate = table.Column<decimal>(type: "numeric", nullable: false),
                    MaxAmount = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LoanOfferings", x => x.Name);
                });

            migrationBuilder.InsertData(
                table: "LoanOfferings",
                columns: new[] { "Name", "InterestRate", "MaxAmount", "Months" },
                values: new object[,]
                {
                    { "Best Loan", 0.05m, 10000m, 12 },
                    { "Worst Loan", 0.25m, 50000m, 12 }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LoanOfferings");
        }
    }
}
