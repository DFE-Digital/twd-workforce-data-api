namespace WorkforceDataApi.DevUtils.Models.Identity;

public class User
{    
    public Guid UserId { get; set; }
    public required string EmailAddress { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public DateOnly? DateOfBirth { get; set; }    
    public UserType UserType { get; set; }
    public string? Trn { get; set; }
}
