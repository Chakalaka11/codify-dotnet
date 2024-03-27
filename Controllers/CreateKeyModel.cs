public class CreateKeyModel
{
    internal static List<string> SupportedClaims = new List<string>() { "AddOrRemoveService", "EditService", "LookupService" };

    public List<String> PassedClaims { get; set; }
}