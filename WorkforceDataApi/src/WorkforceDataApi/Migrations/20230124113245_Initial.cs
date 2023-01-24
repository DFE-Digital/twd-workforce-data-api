using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkforceDataApi.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterDatabase()
                .Annotation("Npgsql:CollationDefinition:case_insensitive", "und-u-ks-level2,und-u-ks-level2,icu,False");

            migrationBuilder.CreateTable(
                name: "tps_extract_data_item",
                columns: table => new
                {
                    tpsextractdataitemid = table.Column<string>(name: "tps_extract_data_item_id", type: "text", nullable: false),
                    memberid = table.Column<string>(name: "member_id", type: "text", nullable: false),
                    teachingstatus = table.Column<string>(name: "teaching_status", type: "character(1)", fixedLength: true, maxLength: 1, nullable: false),
                    trn = table.Column<string>(type: "character(7)", fixedLength: true, maxLength: 7, nullable: false),
                    firstname = table.Column<string>(name: "first_name", type: "character varying(200)", maxLength: 200, nullable: false),
                    lastname = table.Column<string>(name: "last_name", type: "character varying(200)", maxLength: 200, nullable: false),
                    nino = table.Column<string>(type: "character(9)", fixedLength: true, maxLength: 9, nullable: false),
                    dateofbirth = table.Column<DateOnly>(name: "date_of_birth", type: "date", nullable: false),
                    emailaddress = table.Column<string>(name: "email_address", type: "character varying(200)", maxLength: 200, nullable: false, collation: "case_insensitive"),
                    memberpostcode = table.Column<string>(name: "member_postcode", type: "character varying(10)", maxLength: 10, nullable: false),
                    fullorparttimeindicator = table.Column<int>(name: "full_or_part_time_indicator", type: "integer", nullable: false),
                    localauthoritynumber = table.Column<string>(name: "local_authority_number", type: "character(3)", fixedLength: true, maxLength: 3, nullable: false),
                    establishmentnumber = table.Column<string>(name: "establishment_number", type: "character(4)", fixedLength: true, maxLength: 4, nullable: false),
                    establishmentpostcode = table.Column<string>(name: "establishment_postcode", type: "character varying(10)", maxLength: 10, nullable: false),
                    employmentperiodstartdate = table.Column<DateOnly>(name: "employment_period_start_date", type: "date", nullable: false),
                    employmentperiodenddate = table.Column<DateOnly>(name: "employment_period_end_date", type: "date", nullable: false),
                    created = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    updated = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_tps_extract_data_item", x => x.tpsextractdataitemid);
                });

            migrationBuilder.CreateIndex(
                name: "ix_tps_extract_data_item_member_id",
                table: "tps_extract_data_item",
                column: "member_id");

            migrationBuilder.CreateIndex(
                name: "ix_tps_extract_data_item_trn",
                table: "tps_extract_data_item",
                column: "trn");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "tps_extract_data_item");
        }
    }
}
