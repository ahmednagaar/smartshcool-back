using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.DragDrop;

public class DragDropQuestionDto
{
    public int Id { get; set; }
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
    public string GameTitle { get; set; }
    public string? Instructions { get; set; }
    public int NumberOfZones { get; set; }
    public List<DragDropZoneDto> Zones { get; set; } = new();
    public List<DragDropItemDto> Items { get; set; } = new();
    public DifficultyLevel? DifficultyLevel { get; set; } // Derived or explicit
    public int? TimeLimit { get; set; }
    public int PointsPerCorrectItem { get; set; }
    public bool ShowImmediateFeedback { get; set; }
    public string UITheme { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public int DisplayOrder { get; set; }
    public DateTime CreatedDate { get; set; }
}

public class DragDropZoneDto
{
    public int Id { get; set; }
    public int DragDropQuestionId { get; set; }
    public string Label { get; set; }
    public string ColorCode { get; set; }
    public int ZoneOrder { get; set; }
    public string? IconUrl { get; set; }
}

public class DragDropItemDto
{
    public int Id { get; set; }
    public int DragDropQuestionId { get; set; }
    public string Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public int CorrectZoneId { get; set; }
    public int ItemOrder { get; set; }
    public string? Explanation { get; set; }
}

public class CreateDragDropQuestionDto
{
    [Required]
    public GradeLevel Grade { get; set; }
    [Required]
    public SubjectType Subject { get; set; }
    [Required]
    public string GameTitle { get; set; }
    public string? Instructions { get; set; }
    public int NumberOfZones { get; set; }
    public List<CreateDragDropZoneDto> Zones { get; set; } = new();
    public List<CreateDragDropItemDto> Items { get; set; } = new();
    public DifficultyLevel? DifficultyLevel { get; set; }
    public int? TimeLimit { get; set; }
    public int PointsPerCorrectItem { get; set; } = 10;
    public bool ShowImmediateFeedback { get; set; } = true;
    public string UITheme { get; set; } = "modern";
}

public class CreateDragDropZoneDto
{
    [Required]
    public string Label { get; set; }
    public string ColorCode { get; set; } = "#4CAF50";
    public int ZoneOrder { get; set; }
    public string? IconUrl { get; set; }
}

public class CreateDragDropItemDto
{
    [Required]
    public string Text { get; set; }
    public string? ImageUrl { get; set; }
    public string? AudioUrl { get; set; }
    public int CorrectZoneIndex { get; set; } // Index in the Zones list sent in CreateDto, as ID is not yet known
    public int ItemOrder { get; set; }
    public string? Explanation { get; set; }
}

public class UpdateDragDropQuestionDto : CreateDragDropQuestionDto
{
    public int Id { get; set; }
    public new List<UpdateDragDropZoneDto> Zones { get; set; } = new();
    public new List<UpdateDragDropItemDto> Items { get; set; } = new();
    public bool IsActive { get; set; }
}

public class UpdateDragDropZoneDto : CreateDragDropZoneDto
{
    public int? Id { get; set; } // If null, it's new
}

public class UpdateDragDropItemDto : CreateDragDropItemDto
{
    public int? Id { get; set; } // If null, new
    public new int? CorrectZoneId { get; set; } // If set, use ID. If null, use Index? Or just use ID and validation.
    // For update API, we usually expect IDs. If replacing all, maybe different.
    // Let's assume Update replaces the collection or handles ID matching.
}
