namespace Etrx.Application.Options;

public class AuthOptions
{
    public const string SectionName = "Auth";
    public string? AdminHandle { get; set; }
    public string? AdminActivationCode { get; set; }
    public string[] AllowedOrigins { get; set; } = [];

    public bool IsAdmin(string handle) => !string.IsNullOrWhiteSpace(AdminHandle) &&
        string.Equals(handle, AdminHandle.Trim(), StringComparison.OrdinalIgnoreCase);
}
