namespace CodifyDotnet.Services;
public interface ITokenService
{
    public string CreateToken();
    public string GenerateKey(string userId, Dictionary<string, string> claims);
    public string GenerateToken(string apiKey);
    public void RevokeKey(string token);
    public Dictionary<string, JwtToken> GetTokens(string userId);
}