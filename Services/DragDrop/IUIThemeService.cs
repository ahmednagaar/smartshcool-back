using System.Collections.Generic;
using Nafes.API.DTOs.DragDrop;

namespace Nafes.API.Services;

public interface IUIThemeService
{
    IEnumerable<UITheme> GetAvailableThemes();
    UITheme? GetThemeById(string themeId);
}
