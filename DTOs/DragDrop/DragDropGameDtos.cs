using System;
using System.Collections.Generic;
using Nafes.API.Modules;

namespace Nafes.API.DTOs.DragDrop;

public class StartGameRequestDto
{
    public int QuestionId { get; set; }
    public GradeLevel Grade { get; set; }
    public SubjectType Subject { get; set; }
}

public class GameSessionDto
{
    public int SessionId { get; set; }
    public int QuestionId { get; set; }
    public string GameTitle { get; set; }
    public string? Instructions { get; set; }
    public int TimeLimit { get; set; }
    public bool ShowImmediateFeedback { get; set; }
    public string UITheme { get; set; }
    
    // Game Data
    public List<DragDropZoneDto> Zones { get; set; }
    public List<DragDropItemDto> Items { get; set; }
    
    // Resume state
    public int CurrentScore { get; set; }
    public int TimeElapsedSeconds { get; set; }
    public List<int> CompletedItemIds { get; set; } // Items already correctly placed
}

public class SubmitAttemptRequestDto
{
    public int SessionId { get; set; }
    public int ItemId { get; set; }
    public int DroppedInZoneId { get; set; }
}

public class SubmitAttemptResponseDto
{
    public bool IsCorrect { get; set; }
    public int PointsEarned { get; set; }
    public int TotalScore { get; set; }
    public string? Message { get; set; } // "Correct!", "Try again", etc.
    public bool IsGameComplete { get; set; }
}

public class GameResultDto
{
    public int SessionId { get; set; }
    public int TotalScore { get; set; }
    public int MaxPossibleScore { get; set; }
    public int CorrectPlacements { get; set; }
    public int WrongPlacements { get; set; }
    public int TimeSpentSeconds { get; set; }
    public int Stars { get; set; } // 1-3 stars
    public string BadgeUrl { get; set; }
}
