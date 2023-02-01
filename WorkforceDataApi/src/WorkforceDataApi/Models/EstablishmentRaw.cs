namespace WorkforceDataApi.Models;

public class EstablishmentRaw
{
    public const string LaCodeEstablishmentNumberIndexName = "ix_establishment_raw_la_code_establishment_number";    

    public required string Urn { get; init; }
    public required string LaCode { get; init; }
    public required string LaName { get; init; }
    public string? EstablishmentNumber { get; init; }
    public required string EstablishmentName { get; init; }
    public required string TypeOfEstablishmentCode { get; init; }
    public required string TypeOfEstablishmentName { get; init; }
    public required string EstablishmentTypeGroupCode { get; init; }
    public required string EstablishmentTypeGroupName { get; init; }
    public required string EstablishmentStatusCode { get; init; }
    public required string EstablishmentStatusName { get; init; }
    public required string ReasonEstablishmentOpenedCode { get; init; }
    public string? ReasonEstablishmentOpenedName { get; init; }
    public string? OpenDate { get; init; }
    public required string ReasonEstablishmentClosedCode { get; init; }
    public string? ReasonEstablishmentClosedName { get; init; }
    public string? CloseDate { get; init; }
    public required string PhaseOfEducationCode { get; init; }
    public required string PhaseOfEducationName { get; init; }
    public string? StatutoryLowAge { get; init; }
    public string? StatutoryHighAge { get; init; }
    public required string BoardersCode { get; init; }
    public string? BoardersName { get; init; }
    public string? NurseryProvisionName { get; init; }
    public required string OfficialSixthFormCode { get; init; }
    public string? OfficialSixthFormName { get; init; }
    public required string GenderCode { get; init; }
    public string? GenderName { get; init; }
    public required string ReligiousCharacterCode { get; init; }
    public string? ReligiousCharacterName { get; init; }
    public string? ReligiousEthosName { get; init; }
    public required string DioceseCode { get; init; }
    public string? DioceseName { get; init; }
    public required string AdmissionsPolicyCode { get; init; }
    public string? AdmissionsPolicyName { get; init; }
    public string? SchoolCapacity { get; init; }
    public required string SpecialClassesCode { get; init; }
    public string? SpecialClassesName { get; init; }
    public string? CensusDate { get; init; }
    public string? NumberOfPupils { get; init; }
    public string? NumberOfBoys { get; init; }
    public string? NumberOfGirls { get; init; }
    public string? PercentageFsm { get; init; }
    public required string TrustSchoolFlagCode { get; init; }
    public required string TrustSchoolFlagName { get; init; }
    public string? TrustsCode { get; init; }
    public string? TrustsName { get; init; }
    public required string SchoolSponsorFlagName { get; init; }
    public required string SchoolSponsorsName { get; init; }
    public required string FederationFlagName { get; init; }
    public string? FederationsCode { get; init; }
    public string? FederationsName { get; init; }
    public string? Ukprn { get; init; }
    public string? FeheIdentifier { get; init; }
    public string? FurtherEducationTypeName { get; init; }
    public string? OfstedLastInsp { get; init; }
    public required string OfstedSpecialMeasuresCode { get; init; }
    public required string OfstedSpecialMeasuresName { get; init; }
    public required string LastChangedDate { get; init; }
    public string? Street { get; init; }
    public string? Locality { get; init; }
    public string? Address3 { get; init; }
    public string? Town { get; init; }
    public string? CountyName { get; init; }
    public string? Postcode { get; init; }
    public string? SchoolWebsite { get; init; }
    public string? TelephoneNum { get; init; }
    public string? HeadTitleName { get; init; }
    public string? HeadFirstName { get; init; }
    public string? HeadLastName { get; init; }
    public string? HeadPreferredJobTitle { get; init; }
    public required string BsoInspectorateNameName { get; init; }
    public string? InspectorateReport { get; init; }
    public string? DateOfLastInspectionVisit { get; init; }
    public string? NextInspectionVisit { get; init; }
    public string? TeenMothName { get; init; }
    public string? TeenMothPlaces { get; init; }
    public string? CcfName { get; init; }
    public string? SenPruName { get; init; }
    public string? EbdName { get; init; }
    public string? PlacesPru { get; init; }
    public string? FtProvName { get; init; }
    public string? EdByOtherName { get; init; }
    public required string Section41ApprovedName { get; init; }
    public string? Sen1Name { get; init; }
    public string? Sen2Name { get; init; }
    public string? Sen3Name { get; init; }
    public string? Sen4Name { get; init; }
    public string? Sen5Name { get; init; }
    public string? Sen6Name { get; init; }
    public string? Sen7Name { get; init; }
    public string? Sen8Name { get; init; }
    public string? Sen9Name { get; init; }
    public string? Sen10Name { get; init; }
    public string? Sen11Name { get; init; }
    public string? Sen12Name { get; init; }
    public string? Sen13Name { get; init; }
    public string? TypeOfResourcedProvisionName { get; init; }
    public string? ResourcedProvisionOnRoll { get; init; }
    public string? ResourcedProvisionCapacity { get; init; }
    public string? SenUnitOnRoll { get; init; }
    public string? SenUnitCapacity { get; init; }
    public required string GorCode { get; init; }
    public required string GorName { get; init; }
    public required string DistrictAdministrativeCode { get; init; }
    public string? DistrictAdministrativeName { get; init; }
    public required string AdministrativeWardCode { get; init; }
    public string? AdministrativeWardName { get; init; }
    public required string ParliamentaryConstituencyCode { get; init; }
    public string? ParliamentaryConstituencyName { get; init; }
    public required string UrbanRuralCode { get; init; }
    public string? UrbanRuralName { get; init; }
    public required string GssLaCodeName { get; init; }
    public string? Easting { get; init; }
    public string? Northing { get; init; }
    public string? MsoaName { get; init; }
    public string? LsoaName { get; init; }
    public string? InspectorateNameName { get; init; }
    public string? SenStat { get; init; }
    public string? SenNoStat { get; init; }
    public string? BoardingEstablishmentName { get; init; }
    public string? PropsName { get; init; }
    public required string PreviousLaCode { get; init; }
    public string? PreviousLaName { get; init; }
    public string? PreviousEstablishmentNumber { get; init; }
    public string? OfstedRatingName { get; init; }
    public string? RscRegionName { get; init; }
    public string? CountryName { get; init; }
    public string? Uprn { get; init; }
    public string? SiteName { get; init; }
    public string? QabNameCode { get; init; }
    public string? QabNameName { get; init; }
    public string? EstablishmentAccreditedCode { get; init; }
    public string? EstablishmentAccreditedName { get; init; }
    public string? QabReport { get; init; }
    public string? ChNumber { get; init; }
    public required string MsoaCode { get; init; }
    public required string LsoaCode { get; init; }
    public string? Fsm { get; init; }
}
