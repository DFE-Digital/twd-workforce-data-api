using TeacherIdentity.AuthServer.Models;

namespace WorkforceDataApi.Models;

public class TpsExtractDataItem
{
    public const int TeachingStatusFixedLength = 1;
    public const int TrnFixedLength = 7;
    public const int FirstNameMaxLength = 200;
    public const int LastNameMaxLength = 200;
    public const int PostcodeMaxLength = 10;
    public const int EmailAddressMaxLength = 200;
    public const int NinoFixedLength = 9;
    public const int LocalAuthorityNumberFixedLength = 3;
    public const int EstablishmentNumberFixedLength = 4;

    public const string MemberIdIndexName = "ix_tps_extract_data_item_member_id";
    public const string TrnIndexName = "ix_tps_extract_data_item_trn";

    public required string TpsExtractDataItemId { get; init; }
    public required string MemberId { get; init; }
    public required string TeachingStatus { get; init; }    
    public required string Trn { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Nino { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string EmailAddress { get; init; }
    public required string MemberPostcode { get; init; }
    public required FullOrPartTimeIndicatorType FullOrPartTimeIndicator { get; init; }
    public required string LocalAuthorityNumber { get; init; }
    public required string EstablishmentNumber { get; init; }
    public required string EstablishmentPostcode { get; init; }
    public required DateOnly EmploymentPeriodStartDate { get; init; }
    public required DateOnly EmploymentPeriodEndDate { get; init; }
    public required DateTime Created { get; init; }
    public required DateTime Updated { get; init; }
}
