using System;

namespace Nafes.API.Modules;

public class SystemSetting : BaseModel
{
    public string Key { get; set; } = string.Empty; // e.g., "AllowRegistration"
    public string Value { get; set; } = string.Empty; // e.g., "true"
    public string Description { get; set; } = string.Empty;
    public string Group { get; set; } = string.Empty; // e.g., "General", "Security"
    public string Type { get; set; } = "string"; // string, boolean, number
}
