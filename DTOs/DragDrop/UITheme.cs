namespace Nafes.API.DTOs.DragDrop;

public class UITheme
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string PrimaryColor { get; set; }
    public string SecondaryColor { get; set; }
    public string BackgroundPattern { get; set; } // URL or CSS class
    public string Font { get; set; }
    public string Description { get; set; }
    public string PreviewImageUrl { get; set; }
}
