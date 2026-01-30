using System.ComponentModel.DataAnnotations;
using System.Text.Json;

namespace Nafes.API.Validation;

/// <summary>
/// Validates that a string is a valid JSON array and meets specific criteria
/// </summary>
[AttributeUsage(AttributeTargets.Property)]
public class ValidateJsonArrayAttribute : ValidationAttribute
{
    private readonly int _minCount;
    private readonly int _maxCount;

    public ValidateJsonArrayAttribute(int minCount = 1, int maxCount = 10)
    {
        _minCount = minCount;
        _maxCount = maxCount;
    }

    protected override ValidationResult? IsValid(object? value, ValidationContext context)
    {
        if (value == null) return ValidationResult.Success; // Allow nulls, use [Required] if needed
        
        var stringValue = value.ToString();
        if (string.IsNullOrWhiteSpace(stringValue)) return ValidationResult.Success;

        try
        {
            var options = JsonSerializer.Deserialize<List<string>>(stringValue);
            
            if (options == null)
            {
                return new ValidationResult("تنسيق JSON غير صالح");
            }

            if (options.Count < _minCount)
            {
                return new ValidationResult($"يجب أن تحتوي القائمة على {_minCount} عناصر على الأقل");
            }

            if (options.Count > _maxCount)
            {
                return new ValidationResult($"لا يمكن أن تتجاوز القائمة {_maxCount} عناصر");
            }

            if (options.Any(string.IsNullOrWhiteSpace))
            {
                return new ValidationResult("لا يمكن أن تحتوي القائمة على قيم فارغة");
            }

            return ValidationResult.Success;
        }
        catch (JsonException)
        {
            return new ValidationResult("تنسيق JSON غير صالح");
        }
        catch (Exception ex)
        {
            return new ValidationResult($"خطأ في التحقق من صحة البيانات: {ex.Message}");
        }
    }
}
