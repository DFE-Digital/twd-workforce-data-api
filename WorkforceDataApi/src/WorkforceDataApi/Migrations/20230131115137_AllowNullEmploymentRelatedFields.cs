using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkforceDataApi.Migrations
{
    /// <inheritdoc />
    public partial class AllowNullEmploymentRelatedFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "local_authority_number",
                table: "tps_extract_data_item",
                type: "character(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(3)",
                oldFixedLength: true,
                oldMaxLength: 3);

            migrationBuilder.AlterColumn<int>(
                name: "full_or_part_time_indicator",
                table: "tps_extract_data_item",
                type: "integer",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "integer");

            migrationBuilder.AlterColumn<string>(
                name: "establishment_postcode",
                table: "tps_extract_data_item",
                type: "character varying(10)",
                maxLength: 10,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10);

            migrationBuilder.AlterColumn<string>(
                name: "establishment_number",
                table: "tps_extract_data_item",
                type: "character(4)",
                fixedLength: true,
                maxLength: 4,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "character(4)",
                oldFixedLength: true,
                oldMaxLength: 4);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "employment_period_start_date",
                table: "tps_extract_data_item",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");

            migrationBuilder.AlterColumn<DateOnly>(
                name: "employment_period_end_date",
                table: "tps_extract_data_item",
                type: "date",
                nullable: true,
                oldClrType: typeof(DateOnly),
                oldType: "date");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "local_authority_number",
                table: "tps_extract_data_item",
                type: "character(3)",
                fixedLength: true,
                maxLength: 3,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(3)",
                oldFixedLength: true,
                oldMaxLength: 3,
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "full_or_part_time_indicator",
                table: "tps_extract_data_item",
                type: "integer",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "integer",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "establishment_postcode",
                table: "tps_extract_data_item",
                type: "character varying(10)",
                maxLength: 10,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character varying(10)",
                oldMaxLength: 10,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "establishment_number",
                table: "tps_extract_data_item",
                type: "character(4)",
                fixedLength: true,
                maxLength: 4,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "character(4)",
                oldFixedLength: true,
                oldMaxLength: 4,
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "employment_period_start_date",
                table: "tps_extract_data_item",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);

            migrationBuilder.AlterColumn<DateOnly>(
                name: "employment_period_end_date",
                table: "tps_extract_data_item",
                type: "date",
                nullable: false,
                defaultValue: new DateOnly(1, 1, 1),
                oldClrType: typeof(DateOnly),
                oldType: "date",
                oldNullable: true);
        }
    }
}
