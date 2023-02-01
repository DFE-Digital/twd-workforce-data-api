using TeacherIdentity.AuthServer.Models;

namespace WorkforceDataApi.Responses;

public record GetTpsExtractDataItemsResponse
{
    public required IEnumerable<GetTpsExtractDataItemResponseBody> TpsExtractDataItems { get; init; }
}

public record GetTpsExtractDataItemResponseBody
{
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
