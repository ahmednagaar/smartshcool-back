using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Nafes.API.Validation;

/// <summary>
/// Validates that a string property contains a valid JSON array
/// </summary>
[AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
public class ValidateJsonArrayAttribute : ValidationAttribute
{
    public int MinCount { get; set; } = 0;
    public int MaxCount { get; set; } = int.MaxValue;

    public ValidateJsonArrayAttribute(int minCount = 0, int maxCount = int.MaxValue)
    {
        MinCount = minCount;
        MaxCount = maxCount;
        ErrorMessage = "الخيارات يجب أن تكون مصفوفة JSON صالحة";
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
    {
        // Null/empty is valid (use [Required] if the field is mandatory)
        if (value == null || string.IsNullOrWhiteSpace(value.ToString()))
        {
            return ValidationResult.Success;
        }

        var json = value.ToString()!.Trim();

        // Must be an array
        if (!json.StartsWith("[") || !json.EndsWith("]"))
        {
            return new ValidationResult("يجب أن تكون الخيارات مصفوفة JSON (تبدأ بـ [ وتنتهي بـ ])");
        }

        try
        {
            var array = JsonSerializer.Deserialize<string[]>(json);

            if (array == null)
            {
                return new ValidationResult("فشل في تحليل مصفوفة الخيارات");
            }

            if (array.Length < MinCount)
            {
                return new ValidationResult($"يجب أن تحتوي الخيارات على {MinCount} عناصر على الأقل");
            }

            if (array.Length > MaxCount)
            {
                return new ValidationResult($"لا يمكن أن تتجاوز الخيارات {MaxCount} عناصر");
            }

            // Check for empty options
            if (array.Any(item => string.IsNullOrWhiteSpace(item)))
            {
                return new ValidationResult("لا يمكن أن تكون الخيارات فارغة");
            }

            // Check for duplicates
            var uniqueItems = array.Select(a => a.Trim().ToLowerInvariant()).Distinct().Count();
            if (uniqueItems != array.Length)
            {
                return new ValidationResult("توجد خيارات مكررة");
            }

            return ValidationResult.Success;
        }
        catch (JsonException ex)
        {
            return new ValidationResult($"تنسيق JSON غير صالح: {ex.Message}");
        }
    }
}
