using System.Text.RegularExpressions;
using System.Web;

namespace Nafes.API.Services;

public interface ISanitizationService
{
    string Sanitize(string input);
    string SanitizeHtml(string input);
    bool IsValidMediaUrl(string url);
    bool IsValidJsonArray(string json);
}

public class SanitizationService : ISanitizationService
{
    private readonly string[] _allowedDomains = { "nafes.com", "nafes-cdn.com", "youtube.com", "vimeo.com", "cloudinary.com" };

    /// <summary>
    /// Sanitize text input by HTML encoding and removing dangerous patterns
    /// </summary>
    public string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        // HTML encode to prevent XSS
        var sanitized = HttpUtility.HtmlEncode(input);

        return sanitized.Trim();
    }

    /// <summary>
    /// Sanitize HTML content - strips all tags for plain text contexts
    /// </summary>
    public string SanitizeHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;

        var sanitized = input;

        // Remove all script tags and content (case insensitive, handles newlines)
        sanitized = Regex.Replace(sanitized, @"<script[^>]*>[\s\S]*?</script>", string.Empty, RegexOptions.IgnoreCase);
        
        // Remove all style tags and content
        sanitized = Regex.Replace(sanitized, @"<style[^>]*>[\s\S]*?</style>", string.Empty, RegexOptions.IgnoreCase);
        
        // Remove all HTML tags
        sanitized = Regex.Replace(sanitized, @"<[^>]+>", string.Empty);
        
        // Remove event handlers (onclick, onmouseover, etc.)
        sanitized = Regex.Replace(sanitized, @"\s*on\w+\s*=\s*[""'][^""']*[""']", string.Empty, RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"\s*on\w+\s*=\s*[^\s>]+", string.Empty, RegexOptions.IgnoreCase);

        // Remove javascript: and data: URLs
        sanitized = Regex.Replace(sanitized, @"javascript\s*:", string.Empty, RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"data\s*:", string.Empty, RegexOptions.IgnoreCase);
        sanitized = Regex.Replace(sanitized, @"vbscript\s*:", string.Empty, RegexOptions.IgnoreCase);

        // Decode HTML entities and re-encode to catch double encoding attacks
        sanitized = HttpUtility.HtmlDecode(sanitized);
        sanitized = HttpUtility.HtmlEncode(sanitized);

        return sanitized.Trim();
    }

    public bool IsValidMediaUrl(string url)
    {
        if (string.IsNullOrEmpty(url)) return true; // Optional field

        if (!Uri.TryCreate(url, UriKind.Absolute, out var uri))
        {
            return false;
        }

        // Must be HTTPS
        if (uri.Scheme != Uri.UriSchemeHttps)
        {
            return false;
        }

        // Check against allowed domains (optional but recommended)
        // Uncomment for stricter security:
        // return _allowedDomains.Any(domain => uri.Host.EndsWith(domain, StringComparison.OrdinalIgnoreCase));

        return true;
    }

    /// <summary>
    /// Validate that a string is a valid JSON array
    /// </summary>
    public bool IsValidJsonArray(string json)
    {
        if (string.IsNullOrEmpty(json)) return true; // Optional

        json = json.Trim();
        if (!json.StartsWith("[") || !json.EndsWith("]")) return false;

        try
        {
            System.Text.Json.JsonSerializer.Deserialize<string[]>(json);
            return true;
        }
        catch
        {
            return false;
        }
    }
}

