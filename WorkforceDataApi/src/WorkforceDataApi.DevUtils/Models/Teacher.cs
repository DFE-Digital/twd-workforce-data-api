using TeacherIdentity.AuthServer.Models;

namespace WorkforceDataApi.DevUtils.Models;

public class Teacher
{
    public required string MemberId { get; init; }
    public required string Trn { get; init; }
    public required string FirstName { get; init; }
    public required string LastName { get; init; }
    public required string Nino { get; init; }
    public required DateOnly DateOfBirth { get; init; }
    public required string EmailAddress { get; init; }
    public required string MemberPostcode { get; init; }
    public required FullOrPartTimeIndicatorType FullOrPartTimeIndicator { get; init; }
    public required string TeachingStatus { get; init; }
}
