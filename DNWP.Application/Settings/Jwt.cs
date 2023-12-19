namespace DNWP.Application.Settings;

public class Jwt
{
    public string Audience { get; set; }
    public string Issuer { get; set; }
    public string SecretKey { get; set; }
    public string Alg { get; set; }
    public uint ValidForInMinitues { get; set; }
    public uint RefreshTokenValidForInDays { get; set; }
    public uint LockUserJwtInMinitues { get; set; }
    public bool IsValidateIssuer { get; set; }
    public bool IsValidateAudience { get; set; }
    public bool IsValidateIssuerSigningKey { get; set; }
    public bool IsRequireExpirationTime { get; set; }
    public bool IsValidateLifetime { get; set; }
    public string AuthenticationScheme { get; set; }
    public string TokenType { get; set; }
}
