using System.Text.RegularExpressions;

namespace Nafes.API.Services;

public interface ISanitizationService
{
    string Sanitize(string input);
    bool IsValidMediaUrl(string url);
}

public class SanitizationService : ISanitizationService
{
    private readonly string[] _allowedDomains = { "nafes.com", "nafes-cdn.com", "youtube.com", "vimeo.com" };

    public string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // Basic HTML XSS prevention (allow simple formatting if needed, or strip all)
        // For now, we'll strip known dangerous tags but might want to allow some markdown-like formatting later.
        var sanitized = input;

        // Remove script tags
        sanitized = Regex.Replace(sanitized, "<script.*?>.*?</script>", string.Empty, RegexOptions.IgnoreCase | RegexOptions.Singleline);
        
        // Remove event handlers like onclick, onmouseover
        sanitized = Regex.Replace(sanitized, @"\son\w+=""[^""]*""", string.Empty, RegexOptions.IgnoreCase);

        return sanitized.Trim();
    }

    public bool IsValidMediaUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return true; // Optional

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // Allow any HTTPS URL for now, or restrict to specific domains
        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }
        
        // Example domain restriction (commented out for flexibility, enable if needed)
        // return _allowedDomains.Any(domain => uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase));
        
        return true;
    }
}
