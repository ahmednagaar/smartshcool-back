using Nafes.API.DTOs.Question;
using Nafes.API.Modules;

namespace Nafes.API.Validation;

public static class PassageQuestionValidation
{
    public static List<string> ValidateQuestionCreate(QuestionCreateDto dto)
    {
        var errors = new List<string>();
        
        if (dto.Type == QuestionType.Passage)
        {
            if (string.IsNullOrWhiteSpace(dto.PassageText))
            {
                errors.Add("نص القطعة مطلوب لأسئلة القطع");
            }
            else if (dto.PassageText.Length < 100)
            {
                errors.Add("نص القطعة يجب أن يحتوي على 100 حرف على الأقل");
            }
            
            if (dto.SubQuestions == null || dto.SubQuestions.Count < 2)
            {
                errors.Add("يجب إضافة سؤالين فرعيين على الأقل للقطعة");
            }
            else if (dto.SubQuestions.Count > 20)
            {
                errors.Add("لا يمكن إضافة أكثر من 20 سؤال فرعي للقطعة");
            }
            else
            {
                for (int i = 0; i < dto.SubQuestions.Count; i++)
                {
                    var sq = dto.SubQuestions[i];
                    
                    if (string.IsNullOrWhiteSpace(sq.Text))
                    {
                        errors.Add($"نص السؤال الفرعي {i + 1} مطلوب");
                    }
                    
                    try
                    {
                        var options = System.Text.Json.JsonSerializer.Deserialize<string[]>(sq.Options);
                        if (options == null || options.Length < 2)
                        {
                            errors.Add($"السؤال الفرعي {i + 1} يجب أن يحتوي على خيارين على الأقل");
                        }
                        else if (options.Length > 6)
                        {
                            errors.Add($"السؤال الفرعي {i + 1} لا يمكن أن يحتوي على أكثر من 6 خيارات");
                        }
                        else if (!options.Contains(sq.CorrectAnswer))
                        {
                            errors.Add($"الإجابة الصحيحة للسؤال الفرعي {i + 1} يجب أن تكون من ضمن الخيارات");
                        }
                    }
                    catch
                    {
                        errors.Add($"خيارات السؤال الفرعي {i + 1} يجب أن تكون بصيغة JSON صحيحة");
                    }
                }
            }
        }
        else
        {
            // Normal questions should not have passage fields
            if (!string.IsNullOrWhiteSpace(dto.PassageText))
            {
                errors.Add("نص القطعة متاح فقط لأسئلة القطع");
            }
            if (dto.SubQuestions != null && dto.SubQuestions.Count > 0)
            {
                errors.Add("الأسئلة الفرعية متاحة فقط لأسئلة القطع");
            }
        }
        
        return errors;
    }
}
