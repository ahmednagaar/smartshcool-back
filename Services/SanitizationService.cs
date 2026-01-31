using System.Text.RegularExpressions;
using System.Web;
using Ganss.Xss;

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
    private readonly HtmlSanitizer _sanitizer;

    public SanitizationService()
    {
        _sanitizer = new HtmlSanitizer();
        // Configure to strip all HTML tags for strict plain text sanitization
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedAttributes.Clear();
        _sanitizer.AllowedCssProperties.Clear();
        _sanitizer.AllowedSchemes.Clear();
    }

    /// <summary>
    /// Sanitize text input using robust HTML sanitizer to remove all tags
    /// </summary>
    public string Sanitize(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        
        // Sanitize to remove tags
        var sanitized = _sanitizer.Sanitize(input);
        
        // HtmlSanitizer returns the text content when tags are stripped.
        // It handles encoding safely.
        
        return sanitized.Trim();
    }

    /// <summary>
    /// Sanitize HTML content - checking for dangerous scripts but allowing safe HTML if we wanted to (currently configured to strip all)
    /// </summary>
    public string SanitizeHtml(string input)
    {
        // Reusing the robust sanitizer
        return Sanitize(input);
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

