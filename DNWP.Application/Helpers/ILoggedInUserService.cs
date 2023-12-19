namespace DNWP.Application.Helpers;

public interface ILoggedInUserService
{
    public long? UserId { get; }
    public string UserEmail { get; }
    public string FullName { get; }
    public string MobileNumber { get; }
    public string UserName { get; }
    public List<string> Roles { get; }
    public bool IsAuthenticated { get; }
    public string AccessToken { get; }
    public DateTimeOffset JwtExpiresAt { get; }
}