using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace WorkforceDataApi.Models.Mappings;

public class EstablishmenRawMapping : IEntityTypeConfiguration<EstablishmentRaw>
{
    public void Configure(EntityTypeBuilder<EstablishmentRaw> builder)
    {
        builder.ToTable("establishments_raw");
        builder.HasKey(e => e.Urn);
        builder.Property(e => e.Urn).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.LaCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.LaName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.EstablishmentNumber).HasColumnType("VARCHAR");
        builder.HasIndex(e => new { e.LaCode, e.EstablishmentNumber }).HasDatabaseName(EstablishmentRaw.LaCodeEstablishmentNumberIndexName);
        builder.Property(e => e.EstablishmentName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.TypeOfEstablishmentCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.TypeOfEstablishmentName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.EstablishmentTypeGroupCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.EstablishmentTypeGroupName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.EstablishmentStatusCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.EstablishmentStatusName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.ReasonEstablishmentOpenedCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.ReasonEstablishmentOpenedName).HasColumnType("VARCHAR");
        builder.Property(e => e.OpenDate).HasColumnType("VARCHAR");
        builder.Property(e => e.ReasonEstablishmentClosedCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.ReasonEstablishmentClosedName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.CloseDate).HasColumnType("VARCHAR");
        builder.Property(e => e.PhaseOfEducationCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.PhaseOfEducationName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.StatutoryLowAge).HasColumnType("VARCHAR");
        builder.Property(e => e.StatutoryHighAge).HasColumnType("VARCHAR");
        builder.Property(e => e.BoardersCode).HasColumnType("VARCHAR");
        builder.Property(e => e.BoardersName).HasColumnType("VARCHAR");
        builder.Property(e => e.NurseryProvisionName).HasColumnType("VARCHAR");
        builder.Property(e => e.OfficialSixthFormCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.OfficialSixthFormName).HasColumnType("VARCHAR");
        builder.Property(e => e.GenderCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.GenderName).HasColumnType("VARCHAR");
        builder.Property(e => e.ReligiousCharacterCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.ReligiousCharacterName).HasColumnType("VARCHAR");
        builder.Property(e => e.ReligiousEthosName).HasColumnType("VARCHAR");
        builder.Property(e => e.DioceseCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.DioceseName).HasColumnType("VARCHAR");
        builder.Property(e => e.AdmissionsPolicyCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.AdmissionsPolicyName).HasColumnType("VARCHAR");
        builder.Property(e => e.SchoolCapacity).HasColumnType("VARCHAR");
        builder.Property(e => e.SpecialClassesCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.SpecialClassesName).HasColumnType("VARCHAR");
        builder.Property(e => e.CensusDate).HasColumnType("VARCHAR");
        builder.Property(e => e.NumberOfPupils).HasColumnType("VARCHAR");
        builder.Property(e => e.NumberOfBoys).HasColumnType("VARCHAR");
        builder.Property(e => e.NumberOfGirls).HasColumnType("VARCHAR");
        builder.Property(e => e.PercentageFsm).HasColumnType("VARCHAR");
        builder.Property(e => e.TrustSchoolFlagCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.TrustSchoolFlagName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.TrustsCode).HasColumnType("VARCHAR");
        builder.Property(e => e.TrustsName).HasColumnType("VARCHAR");
        builder.Property(e => e.SchoolSponsorFlagName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.SchoolSponsorsName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.FederationFlagName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.FederationsCode).HasColumnType("VARCHAR");
        builder.Property(e => e.FederationsName).HasColumnType("VARCHAR");
        builder.Property(e => e.Ukprn).HasColumnType("VARCHAR");
        builder.Property(e => e.FeheIdentifier).HasColumnType("VARCHAR");
        builder.Property(e => e.FurtherEducationTypeName).HasColumnType("VARCHAR");
        builder.Property(e => e.OfstedLastInsp).HasColumnType("VARCHAR");
        builder.Property(e => e.OfstedSpecialMeasuresCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.OfstedSpecialMeasuresName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.LastChangedDate).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.Street).HasColumnType("VARCHAR");
        builder.Property(e => e.Locality).HasColumnType("VARCHAR");
        builder.Property(e => e.Address3).HasColumnType("VARCHAR");
        builder.Property(e => e.Town).HasColumnType("VARCHAR");
        builder.Property(e => e.CountyName).HasColumnType("VARCHAR");
        builder.Property(e => e.Postcode).HasColumnType("VARCHAR");
        builder.Property(e => e.SchoolWebsite).HasColumnType("VARCHAR");
        builder.Property(e => e.TelephoneNum).HasColumnType("VARCHAR");
        builder.Property(e => e.HeadTitleName).HasColumnType("VARCHAR");
        builder.Property(e => e.HeadFirstName).HasColumnType("VARCHAR");
        builder.Property(e => e.HeadLastName).HasColumnType("VARCHAR");
        builder.Property(e => e.HeadPreferredJobTitle).HasColumnType("VARCHAR");
        builder.Property(e => e.BsoInspectorateNameName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.InspectorateReport).HasColumnType("VARCHAR");
        builder.Property(e => e.DateOfLastInspectionVisit).HasColumnType("VARCHAR");
        builder.Property(e => e.NextInspectionVisit).HasColumnType("VARCHAR");
        builder.Property(e => e.TeenMothName).HasColumnType("VARCHAR");
        builder.Property(e => e.TeenMothPlaces).HasColumnType("VARCHAR");
        builder.Property(e => e.CcfName).HasColumnType("VARCHAR");
        builder.Property(e => e.SenPruName).HasColumnType("VARCHAR");
        builder.Property(e => e.EbdName).HasColumnType("VARCHAR");
        builder.Property(e => e.PlacesPru).HasColumnType("VARCHAR");
        builder.Property(e => e.FtProvName).HasColumnType("VARCHAR");
        builder.Property(e => e.EdByOtherName).HasColumnType("VARCHAR");
        builder.Property(e => e.Section41ApprovedName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.Sen1Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen2Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen3Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen4Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen5Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen6Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen7Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen8Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen9Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen10Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen11Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen12Name).HasColumnType("VARCHAR");
        builder.Property(e => e.Sen13Name).HasColumnType("VARCHAR");
        builder.Property(e => e.TypeOfResourcedProvisionName).HasColumnType("VARCHAR");
        builder.Property(e => e.ResourcedProvisionOnRoll).HasColumnType("VARCHAR");
        builder.Property(e => e.ResourcedProvisionCapacity).HasColumnType("VARCHAR");
        builder.Property(e => e.SenUnitOnRoll).HasColumnType("VARCHAR");
        builder.Property(e => e.SenUnitCapacity).HasColumnType("VARCHAR");
        builder.Property(e => e.GorCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.GorName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.DistrictAdministrativeCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.DistrictAdministrativeName).HasColumnType("VARCHAR");
        builder.Property(e => e.AdministrativeWardCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.AdministrativeWardName).HasColumnType("VARCHAR");
        builder.Property(e => e.ParliamentaryConstituencyCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.ParliamentaryConstituencyName).HasColumnType("VARCHAR");
        builder.Property(e => e.UrbanRuralCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.UrbanRuralName).HasColumnType("VARCHAR");
        builder.Property(e => e.GssLaCodeName).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.Easting).HasColumnType("VARCHAR");
        builder.Property(e => e.Northing).HasColumnType("VARCHAR");
        builder.Property(e => e.MsoaName).HasColumnType("VARCHAR");
        builder.Property(e => e.LsoaName).HasColumnType("VARCHAR");
        builder.Property(e => e.InspectorateNameName).HasColumnType("VARCHAR");
        builder.Property(e => e.SenStat).HasColumnType("VARCHAR");
        builder.Property(e => e.SenNoStat).HasColumnType("VARCHAR");
        builder.Property(e => e.BoardingEstablishmentName).HasColumnType("VARCHAR");
        builder.Property(e => e.PropsName).HasColumnType("VARCHAR");
        builder.Property(e => e.PreviousLaCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.PreviousLaName).HasColumnType("VARCHAR");
        builder.Property(e => e.PreviousEstablishmentNumber).HasColumnType("VARCHAR");
        builder.Property(e => e.OfstedRatingName).HasColumnType("VARCHAR");
        builder.Property(e => e.RscRegionName).HasColumnType("VARCHAR");
        builder.Property(e => e.CountryName).HasColumnType("VARCHAR");
        builder.Property(e => e.Uprn).HasColumnType("VARCHAR");
        builder.Property(e => e.SiteName).HasColumnType("VARCHAR");
        builder.Property(e => e.QabNameCode).HasColumnType("VARCHAR");
        builder.Property(e => e.QabNameName).HasColumnType("VARCHAR");
        builder.Property(e => e.EstablishmentAccreditedCode).HasColumnType("VARCHAR");
        builder.Property(e => e.EstablishmentAccreditedName).HasColumnType("VARCHAR");
        builder.Property(e => e.QabReport).HasColumnType("VARCHAR");
        builder.Property(e => e.ChNumber).HasColumnType("VARCHAR");
        builder.Property(e => e.MsoaCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.LsoaCode).HasColumnType("VARCHAR").IsRequired();
        builder.Property(e => e.Fsm).HasColumnType("VARCHAR");
    }
}
