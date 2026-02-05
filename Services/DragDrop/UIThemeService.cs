using System.Collections.Generic;
using System.Linq;
using Nafes.API.DTOs.DragDrop;

namespace Nafes.API.Services;

public class UIThemeService : IUIThemeService
{
    private readonly List<UITheme> _themes = new()
    {
        new UITheme 
        { 
            Id = "modern", 
            Name = "Modern Clean", 
            PrimaryColor = "#3B82F6", 
            SecondaryColor = "#10B981", 
            BackgroundPattern = "bg-dots", 
            Font = "Inter",
            Description = "Clean and professional design suitable for all grades.",
            PreviewImageUrl = ""
        },
        new UITheme 
        { 
            Id = "nature", 
            Name = "Nature Explorer", 
            PrimaryColor = "#4ADE80", 
            SecondaryColor = "#A3E635", 
            BackgroundPattern = "bg-leaves", 
            Font = "Comic Neue",
            Description = "Fun and organic theme with nature elements.",
            PreviewImageUrl = ""
        },
        new UITheme 
        { 
            Id = "ocean", 
            Name = "Deep Ocean", 
            PrimaryColor = "#0EA5E9", 
            SecondaryColor = "#38BDF8", 
            BackgroundPattern = "bg-waves", 
            Font = "Quicksand",
            Description = "Calming blue tones with aquatic vibes.",
            PreviewImageUrl = ""
        },
        new UITheme 
        { 
            Id = "sunset", 
            Name = "Warm Sunset", 
            PrimaryColor = "#F97316", 
            SecondaryColor = "#FDBA74", 
            BackgroundPattern = "bg-sun", 
            Font = "Dosis",
            Description = "Energetic warm colors for engagement.",
            PreviewImageUrl = ""
        }
    };

    public IEnumerable<UITheme> GetAvailableThemes()
    {
        return _themes;
    }

    public UITheme? GetThemeById(string themeId)
    {
        return _themes.FirstOrDefault(t => t.Id.ToLower() == themeId.ToLower());
    }
}
