using System;

namespace Nafes.API.DTOs.DragDrop;

public class LeaderboardEntry
{
    public int Rank { get; set; }
    public int StudentId { get; set; }
    public string StudentName { get; set; }
    public int Score { get; set; }
    public double Accuracy { get; set; }
    public DateTime DatePlayed { get; set; }
    public bool IsCurrentUser { get; set; }
    
    // Additional helpful fields
    public string GradeName { get; set; }
    public int TimeSpentSeconds { get; set; }
}
