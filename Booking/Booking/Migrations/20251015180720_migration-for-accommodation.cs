using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Booking.Migrations
{
    /// <inheritdoc />
    public partial class migrationforaccommodation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "booking");

            migrationBuilder.CreateTable(
                name: "Accommodations",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: false),
                    Location = table.Column<string>(type: "text", nullable: false),
                    Conveniences = table.Column<int[]>(type: "integer[]", nullable: false),
                    Photos = table.Column<List<string>>(type: "text[]", nullable: false),
                    MinGuestNumber = table.Column<int>(type: "integer", nullable: false),
                    MaxGuestNumber = table.Column<int>(type: "integer", nullable: false),
                    GlobalPrice = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accommodations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Accommodation_Availability",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccommodationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Duration_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration_To = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accommodation_Availability", x => new { x.AccommodationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Accommodation_Availability_Accommodations_AccommodationId",
                        column: x => x.AccommodationId,
                        principalSchema: "booking",
                        principalTable: "Accommodations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Accommodation_Prices",
                schema: "booking",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    AccommodationId = table.Column<Guid>(type: "uuid", nullable: false),
                    Amount = table.Column<decimal>(type: "numeric", nullable: false),
                    Duration_From = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    Duration_To = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    PriceType = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Accommodation_Prices", x => new { x.AccommodationId, x.Id });
                    table.ForeignKey(
                        name: "FK_Accommodation_Prices_Accommodations_AccommodationId",
                        column: x => x.AccommodationId,
                        principalSchema: "booking",
                        principalTable: "Accommodations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Accommodation_Availability",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Accommodation_Prices",
                schema: "booking");

            migrationBuilder.DropTable(
                name: "Accommodations",
                schema: "booking");
        }
    }
}
