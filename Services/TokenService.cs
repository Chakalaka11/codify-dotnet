using CodifyDotnet.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace CodifyDotnet.Services;
public class TokenService : ITokenService
{
    private readonly string _privateKey;
    private readonly string _issuer;

    private readonly Dictionary<string, JwtToken> _tokens;
    private readonly Dictionary<string, List<Claim>> _registeredClaims;
    private readonly Dictionary<string, List<string>> _assignedKeys;

    public TokenService(IOptions<JwtConfiguration> options)
    {
        _privateKey = options.Value.PrivateKey;
        _issuer = options.Value.Issuer;
        _tokens = new Dictionary<string, JwtToken>();
        _registeredClaims = new Dictionary<string, List<Claim>>();
        _assignedKeys = new Dictionary<string, List<string>>();
    }
    public string CreateToken()
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_privateKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var Sectoken = new JwtSecurityToken(
            issuer: _issuer,
            audience: null,
            claims: [new Claim("ChangeService", "Allowed")],
          expires: DateTime.Now.AddMinutes(120),
          signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(Sectoken);
        return token;
    }

    public string GenerateKey(string userId, Dictionary<string, string> claims)
    {
        // Generate new key
        var apiKeyGuid = Guid.NewGuid();
        var castedClaims = new List<Claim>();
        foreach (var item in claims)
        {
            castedClaims.Add(new Claim(item.Key, item.Value));
        }
        _registeredClaims.Add(apiKeyGuid.ToString(), castedClaims);

        // Create connection between user and new key
        if (!_assignedKeys.ContainsKey(userId))
        {
            _assignedKeys.Add(userId, new List<string>());
        }
        _assignedKeys[userId].Add(apiKeyGuid.ToString());

        return apiKeyGuid.ToString();
    }

    public string GenerateToken(string apiKey)
    {
        if (!_registeredClaims.ContainsKey(apiKey))
        {
            Console.WriteLine("This key wasn't generated");
            return null;
        }

        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_privateKey));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);

        var securityToken = new JwtSecurityToken(
            issuer: _issuer,
            audience: null,
            claims: _registeredClaims[apiKey],
            expires: DateTime.Now.AddMinutes(120),
            signingCredentials: credentials);

        var token = new JwtSecurityTokenHandler().WriteToken(securityToken);

        if (_tokens.ContainsKey(apiKey))
        {
            _tokens[apiKey].Token = token;
            _tokens[apiKey].LastUpdated = DateTime.Now;
        }
        else
        {
            _tokens.Add(apiKey, new JwtToken() { Token = token, LastUpdated = DateTime.Now });
        }
        return token;
    }

    public void RevokeKey(string token)
    {
        var existingToken = _tokens.FirstOrDefault(x => x.Value.Token == token);
        _tokens.Remove(existingToken.Key);
    }

    public Dictionary<string, JwtToken> GetTokens(string userId)
    {
        if (!_assignedKeys.ContainsKey(userId))
        {
            return null;
        }

        var apiKeys = _assignedKeys.Where(x => x.Key == userId).Select(x => x.Value).First();
        var existingTokens = _tokens.Where(x => apiKeys.Contains(x.Key)).ToDictionary();
        return existingTokens;
    }
}

public class JwtToken
{
    public string Token { get; set; }
    public DateTime LastUpdated { get; set; }
}