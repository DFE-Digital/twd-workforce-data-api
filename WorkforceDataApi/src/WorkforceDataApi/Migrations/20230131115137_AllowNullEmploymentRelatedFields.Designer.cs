﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;
using WorkforceDataApi.Models;

#nullable disable

namespace WorkforceDataApi.Migrations
{
    [DbContext(typeof(WorkforceDbContext))]
    [Migration("20230131115137_AllowNullEmploymentRelatedFields")]
    partial class AllowNullEmploymentRelatedFields
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Npgsql:CollationDefinition:case_insensitive", "und-u-ks-level2,und-u-ks-level2,icu,False")
                .HasAnnotation("ProductVersion", "7.0.2")
                .HasAnnotation("Relational:MaxIdentifierLength", 63);

            NpgsqlModelBuilderExtensions.UseIdentityByDefaultColumns(modelBuilder);

            modelBuilder.Entity("WorkforceDataApi.Models.EstablishmentRaw", b =>
                {
                    b.Property<string>("Urn")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("urn");

                    b.Property<string>("Address3")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("address3");

                    b.Property<string>("AdministrativeWardCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("administrative_ward_code");

                    b.Property<string>("AdministrativeWardName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("administrative_ward_name");

                    b.Property<string>("AdmissionsPolicyCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("admissions_policy_code");

                    b.Property<string>("AdmissionsPolicyName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("admissions_policy_name");

                    b.Property<string>("BoardersCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("boarders_code");

                    b.Property<string>("BoardersName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("boarders_name");

                    b.Property<string>("BoardingEstablishmentName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("boarding_establishment_name");

                    b.Property<string>("BsoInspectorateNameName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("bso_inspectorate_name_name");

                    b.Property<string>("CcfName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ccf_name");

                    b.Property<string>("CensusDate")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("census_date");

                    b.Property<string>("ChNumber")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ch_number");

                    b.Property<string>("CloseDate")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("close_date");

                    b.Property<string>("CountryName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("country_name");

                    b.Property<string>("CountyName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("county_name");

                    b.Property<string>("DateOfLastInspectionVisit")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("date_of_last_inspection_visit");

                    b.Property<string>("DioceseCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("diocese_code");

                    b.Property<string>("DioceseName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("diocese_name");

                    b.Property<string>("DistrictAdministrativeCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("district_administrative_code");

                    b.Property<string>("DistrictAdministrativeName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("district_administrative_name");

                    b.Property<string>("Easting")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("easting");

                    b.Property<string>("EbdName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ebd_name");

                    b.Property<string>("EdByOtherName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ed_by_other_name");

                    b.Property<string>("EstablishmentAccreditedCode")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_accredited_code");

                    b.Property<string>("EstablishmentAccreditedName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_accredited_name");

                    b.Property<string>("EstablishmentName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_name");

                    b.Property<string>("EstablishmentNumber")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_number");

                    b.Property<string>("EstablishmentStatusCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_status_code");

                    b.Property<string>("EstablishmentStatusName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_status_name");

                    b.Property<string>("EstablishmentTypeGroupCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_type_group_code");

                    b.Property<string>("EstablishmentTypeGroupName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("establishment_type_group_name");

                    b.Property<string>("FederationFlagName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("federation_flag_name");

                    b.Property<string>("FederationsCode")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("federations_code");

                    b.Property<string>("FederationsName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("federations_name");

                    b.Property<string>("FeheIdentifier")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("fehe_identifier");

                    b.Property<string>("Fsm")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("fsm");

                    b.Property<string>("FtProvName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ft_prov_name");

                    b.Property<string>("FurtherEducationTypeName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("further_education_type_name");

                    b.Property<string>("GenderCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("gender_code");

                    b.Property<string>("GenderName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("gender_name");

                    b.Property<string>("GorCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("gor_code");

                    b.Property<string>("GorName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("gor_name");

                    b.Property<string>("GssLaCodeName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("gss_la_code_name");

                    b.Property<string>("HeadFirstName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("head_first_name");

                    b.Property<string>("HeadLastName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("head_last_name");

                    b.Property<string>("HeadPreferredJobTitle")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("head_preferred_job_title");

                    b.Property<string>("HeadTitleName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("head_title_name");

                    b.Property<string>("InspectorateNameName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("inspectorate_name_name");

                    b.Property<string>("InspectorateReport")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("inspectorate_report");

                    b.Property<string>("LaCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("la_code");

                    b.Property<string>("LaName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("la_name");

                    b.Property<string>("LastChangedDate")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("last_changed_date");

                    b.Property<string>("Locality")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("locality");

                    b.Property<string>("LsoaCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("lsoa_code");

                    b.Property<string>("LsoaName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("lsoa_name");

                    b.Property<string>("MsoaCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("msoa_code");

                    b.Property<string>("MsoaName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("msoa_name");

                    b.Property<string>("NextInspectionVisit")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("next_inspection_visit");

                    b.Property<string>("Northing")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("northing");

                    b.Property<string>("NumberOfBoys")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("number_of_boys");

                    b.Property<string>("NumberOfGirls")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("number_of_girls");

                    b.Property<string>("NumberOfPupils")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("number_of_pupils");

                    b.Property<string>("NurseryProvisionName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("nursery_provision_name");

                    b.Property<string>("OfficialSixthFormCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("official_sixth_form_code");

                    b.Property<string>("OfficialSixthFormName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("official_sixth_form_name");

                    b.Property<string>("OfstedLastInsp")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ofsted_last_insp");

                    b.Property<string>("OfstedRatingName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ofsted_rating_name");

                    b.Property<string>("OfstedSpecialMeasuresCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ofsted_special_measures_code");

                    b.Property<string>("OfstedSpecialMeasuresName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ofsted_special_measures_name");

                    b.Property<string>("OpenDate")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("open_date");

                    b.Property<string>("ParliamentaryConstituencyCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("parliamentary_constituency_code");

                    b.Property<string>("ParliamentaryConstituencyName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("parliamentary_constituency_name");

                    b.Property<string>("PercentageFsm")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("percentage_fsm");

                    b.Property<string>("PhaseOfEducationCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("phase_of_education_code");

                    b.Property<string>("PhaseOfEducationName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("phase_of_education_name");

                    b.Property<string>("PlacesPru")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("places_pru");

                    b.Property<string>("Postcode")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("postcode");

                    b.Property<string>("PreviousEstablishmentNumber")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("previous_establishment_number");

                    b.Property<string>("PreviousLaCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("previous_la_code");

                    b.Property<string>("PreviousLaName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("previous_la_name");

                    b.Property<string>("PropsName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("props_name");

                    b.Property<string>("QabNameCode")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("qab_name_code");

                    b.Property<string>("QabNameName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("qab_name_name");

                    b.Property<string>("QabReport")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("qab_report");

                    b.Property<string>("ReasonEstablishmentClosedCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("reason_establishment_closed_code");

                    b.Property<string>("ReasonEstablishmentClosedName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("reason_establishment_closed_name");

                    b.Property<string>("ReasonEstablishmentOpenedCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("reason_establishment_opened_code");

                    b.Property<string>("ReasonEstablishmentOpenedName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("reason_establishment_opened_name");

                    b.Property<string>("ReligiousCharacterCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("religious_character_code");

                    b.Property<string>("ReligiousCharacterName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("religious_character_name");

                    b.Property<string>("ReligiousEthosName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("religious_ethos_name");

                    b.Property<string>("ResourcedProvisionCapacity")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("resourced_provision_capacity");

                    b.Property<string>("ResourcedProvisionOnRoll")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("resourced_provision_on_roll");

                    b.Property<string>("RscRegionName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("rsc_region_name");

                    b.Property<string>("SchoolCapacity")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("school_capacity");

                    b.Property<string>("SchoolSponsorFlagName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("school_sponsor_flag_name");

                    b.Property<string>("SchoolSponsorsName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("school_sponsors_name");

                    b.Property<string>("SchoolWebsite")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("school_website");

                    b.Property<string>("Section41ApprovedName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("section41approved_name");

                    b.Property<string>("Sen10Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen10name");

                    b.Property<string>("Sen11Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen11name");

                    b.Property<string>("Sen12Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen12name");

                    b.Property<string>("Sen13Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen13name");

                    b.Property<string>("Sen1Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen1name");

                    b.Property<string>("Sen2Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen2name");

                    b.Property<string>("Sen3Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen3name");

                    b.Property<string>("Sen4Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen4name");

                    b.Property<string>("Sen5Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen5name");

                    b.Property<string>("Sen6Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen6name");

                    b.Property<string>("Sen7Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen7name");

                    b.Property<string>("Sen8Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen8name");

                    b.Property<string>("Sen9Name")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen9name");

                    b.Property<string>("SenNoStat")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen_no_stat");

                    b.Property<string>("SenPruName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen_pru_name");

                    b.Property<string>("SenStat")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen_stat");

                    b.Property<string>("SenUnitCapacity")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen_unit_capacity");

                    b.Property<string>("SenUnitOnRoll")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("sen_unit_on_roll");

                    b.Property<string>("SiteName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("site_name");

                    b.Property<string>("SpecialClassesCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("special_classes_code");

                    b.Property<string>("SpecialClassesName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("special_classes_name");

                    b.Property<string>("StatutoryHighAge")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("statutory_high_age");

                    b.Property<string>("StatutoryLowAge")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("statutory_low_age");

                    b.Property<string>("Street")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("street");

                    b.Property<string>("TeenMothName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("teen_moth_name");

                    b.Property<string>("TeenMothPlaces")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("teen_moth_places");

                    b.Property<string>("TelephoneNum")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("telephone_num");

                    b.Property<string>("Town")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("town");

                    b.Property<string>("TrustSchoolFlagCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("trust_school_flag_code");

                    b.Property<string>("TrustSchoolFlagName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("trust_school_flag_name");

                    b.Property<string>("TrustsCode")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("trusts_code");

                    b.Property<string>("TrustsName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("trusts_name");

                    b.Property<string>("TypeOfEstablishmentCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("type_of_establishment_code");

                    b.Property<string>("TypeOfEstablishmentName")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("type_of_establishment_name");

                    b.Property<string>("TypeOfResourcedProvisionName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("type_of_resourced_provision_name");

                    b.Property<string>("Ukprn")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("ukprn");

                    b.Property<string>("Uprn")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("uprn");

                    b.Property<string>("UrbanRuralCode")
                        .IsRequired()
                        .HasColumnType("VARCHAR")
                        .HasColumnName("urban_rural_code");

                    b.Property<string>("UrbanRuralName")
                        .HasColumnType("VARCHAR")
                        .HasColumnName("urban_rural_name");

                    b.HasKey("Urn")
                        .HasName("pk_establishments_raw");

                    b.HasIndex("LaCode", "EstablishmentNumber")
                        .HasDatabaseName("ix_establishment_raw_la_code_establishment_number");

                    b.ToTable("establishments_raw", (string)null);
                });

            modelBuilder.Entity("WorkforceDataApi.Models.TpsExtractDataItem", b =>
                {
                    b.Property<string>("TpsExtractDataItemId")
                        .HasColumnType("text")
                        .HasColumnName("tps_extract_data_item_id");

                    b.Property<DateTime>("Created")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("created");

                    b.Property<DateOnly>("DateOfBirth")
                        .HasColumnType("date")
                        .HasColumnName("date_of_birth");

                    b.Property<string>("EmailAddress")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("email_address")
                        .UseCollation("case_insensitive");

                    b.Property<DateOnly?>("EmploymentPeriodEndDate")
                        .HasColumnType("date")
                        .HasColumnName("employment_period_end_date");

                    b.Property<DateOnly?>("EmploymentPeriodStartDate")
                        .HasColumnType("date")
                        .HasColumnName("employment_period_start_date");

                    b.Property<string>("EstablishmentNumber")
                        .HasMaxLength(4)
                        .HasColumnType("character(4)")
                        .HasColumnName("establishment_number")
                        .IsFixedLength();

                    b.Property<string>("EstablishmentPostcode")
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("establishment_postcode");

                    b.Property<string>("FirstName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("first_name");

                    b.Property<int?>("FullOrPartTimeIndicator")
                        .HasColumnType("integer")
                        .HasColumnName("full_or_part_time_indicator");

                    b.Property<string>("LastName")
                        .IsRequired()
                        .HasMaxLength(200)
                        .HasColumnType("character varying(200)")
                        .HasColumnName("last_name");

                    b.Property<string>("LocalAuthorityNumber")
                        .HasMaxLength(3)
                        .HasColumnType("character(3)")
                        .HasColumnName("local_authority_number")
                        .IsFixedLength();

                    b.Property<string>("MemberId")
                        .IsRequired()
                        .HasColumnType("text")
                        .HasColumnName("member_id");

                    b.Property<string>("MemberPostcode")
                        .IsRequired()
                        .HasMaxLength(10)
                        .HasColumnType("character varying(10)")
                        .HasColumnName("member_postcode");

                    b.Property<string>("Nino")
                        .IsRequired()
                        .HasMaxLength(9)
                        .HasColumnType("character(9)")
                        .HasColumnName("nino")
                        .IsFixedLength();

                    b.Property<string>("TeachingStatus")
                        .IsRequired()
                        .HasMaxLength(1)
                        .HasColumnType("character(1)")
                        .HasColumnName("teaching_status")
                        .IsFixedLength();

                    b.Property<string>("Trn")
                        .IsRequired()
                        .HasMaxLength(7)
                        .HasColumnType("character(7)")
                        .HasColumnName("trn")
                        .IsFixedLength();

                    b.Property<DateTime>("Updated")
                        .HasColumnType("timestamp with time zone")
                        .HasColumnName("updated");

                    b.HasKey("TpsExtractDataItemId")
                        .HasName("pk_tps_extract_data_item");

                    b.HasIndex("MemberId")
                        .HasDatabaseName("ix_tps_extract_data_item_member_id");

                    b.HasIndex("Trn")
                        .HasDatabaseName("ix_tps_extract_data_item_trn");

                    b.ToTable("tps_extract_data_item", (string)null);
                });
#pragma warning restore 612, 618
        }
    }
}
