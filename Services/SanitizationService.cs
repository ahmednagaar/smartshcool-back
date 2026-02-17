using System.Text.RegularExpressions;
using System.Web;
using Ganss.Xss;

namespace Nafes.API.Services;

public interface ISanitizationService
{
    string Sanitize(string input);
    string SanitizeHtml(string input);
    string SanitizePassageHtml(string input);
    bool IsValidMediaUrl(string url);
    bool IsValidJsonArray(string json);
}

public class SanitizationService : ISanitizationService
{
    private readonly HtmlSanitizer _sanitizer;
    private readonly HtmlSanitizer _passageSanitizer;

    public SanitizationService()
    {
        _sanitizer = new HtmlSanitizer();
        // Configure to strip all HTML tags for strict plain text sanitization
        _sanitizer.AllowedTags.Clear();
        _sanitizer.AllowedAttributes.Clear();
        _sanitizer.AllowedCssProperties.Clear();
        _sanitizer.AllowedSchemes.Clear();
        
        // Permissive sanitizer for passage rich text
        _passageSanitizer = new HtmlSanitizer();
        _passageSanitizer.AllowedTags.Clear();
        foreach (var tag in new[] { "b", "i", "u", "strong", "em", "p", "br", "ul", "ol", "li", "h1", "h2", "h3", "h4", "a", "img", "span", "div", "table", "tr", "td", "th", "thead", "tbody" })
            _passageSanitizer.AllowedTags.Add(tag);
        _passageSanitizer.AllowedAttributes.Clear();
        foreach (var attr in new[] { "href", "src", "alt", "class", "style", "target", "dir" })
            _passageSanitizer.AllowedAttributes.Add(attr);
        _passageSanitizer.AllowedSchemes.Add("https");
        _passageSanitizer.AllowedSchemes.Add("http");
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
    /// Sanitize HTML content - strip all tags
    /// </summary>
    public string SanitizeHtml(string input)
    {
        return Sanitize(input);
    }

    /// <summary>
    /// Sanitize passage HTML - allow safe formatting tags, strip scripts
    /// </summary>
    public string SanitizePassageHtml(string input)
    {
        if (string.IsNullOrEmpty(input)) return string.Empty;
        return _passageSanitizer.Sanitize(input).Trim();
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

