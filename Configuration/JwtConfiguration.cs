namespace CodifyDotnet.Configuration;

public class JwtConfiguration
{
    public static string SectionName = "Jwt";

    public string PrivateKey { get; set; }
    public string Issuer { get; set; }
}