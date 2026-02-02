using AutoMapper;
using Nafes.API.DTOs.DragDrop;
using Nafes.API.Modules;
using Nafes.API.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Nafes.API.Services;

public class DragDropGameService : IDragDropGameService
{
    private readonly IDragDropGameSessionRepository _sessionRepository;
    private readonly IDragDropQuestionRepository _questionRepository;
    private readonly IDragDropAttemptRepository _attemptRepository;
    private readonly IMapper _mapper;

    public DragDropGameService(
        IDragDropGameSessionRepository sessionRepository,
        IDragDropQuestionRepository questionRepository,
        IDragDropAttemptRepository attemptRepository,
        IMapper mapper)
    {
        _sessionRepository = sessionRepository;
        _questionRepository = questionRepository;
        _attemptRepository = attemptRepository;
        _mapper = mapper;
    }

    public async Task<GameSessionDto> StartSessionAsync(StartGameRequestDto request, int studentId)
    {
        // 1. Check if active session exists?
        var activeSession = await _sessionRepository.GetActiveSessionAsync(studentId);
        if (activeSession != null)
        {
            // If exists, resume it? Or close and start new?
            // For now, let's just complete the old one or return it.
            // Let's return the active one if it matches the requested QuestionId, else complete it.
            if (activeSession.DragDropQuestionId == request.QuestionId)
            {
               return await MapToGameSessionDto(activeSession);
            }
            else
            {
                await _sessionRepository.CompleteSessionAsync(activeSession.Id);
            }
        }

        // 2. Create new session
        var question = await _questionRepository.GetByIdAsync(request.QuestionId, includeZonesAndItems: true);
        if (question == null) throw new KeyNotFoundException("Question not found");

        var newSession = new DragDropGameSession
        {
            StudentId = studentId,
            DragDropQuestionId = request.QuestionId,
            Grade = request.Grade,
            Subject = request.Subject,
            StartTime = DateTime.UtcNow,
            TotalItems = question.Items.Count,
            TotalScore = 0,
            IsCompleted = false
        };

        var createdSession = await _sessionRepository.CreateSessionAsync(newSession);

        // Need to reload with navigation properties to map DTO fully? 
        // Or just map manually using 'question' variable.
        return await MapToGameSessionDto(createdSession, question);
    }

    public async Task<SubmitAttemptResponseDto> ProcessAttemptAsync(SubmitAttemptRequestDto request)
    {
        var session = await _sessionRepository.GetSessionByIdAsync(request.SessionId, includeAttempts: true);
        if (session == null) throw new KeyNotFoundException("Session not found");
        if (session.IsCompleted) throw new InvalidOperationException("Session is already completed");

        // Validate Item and Zone
        var question = session.DragDropQuestion;
        var item = question.Items.FirstOrDefault(i => i.Id == request.ItemId);
        var zone = question.Zones.FirstOrDefault(z => z.Id == request.DroppedInZoneId);

        if (item == null || zone == null) throw new ArgumentException("Invalid Item or Zone ID");

        // Check if correct
        bool isCorrect = item.CorrectZoneId == zone.Id;
        int points = 0;

        if (isCorrect)
        {
            points = question.PointsPerCorrectItem;
            session.CorrectPlacements++;
            session.TotalScore += points;
            
            // Check if user has already placed this item correctly before?
            // Usually we prevent re-dragging correct items in FE.
            // But if they spy the API, we should check.
             if (session.Attempts.Any(a => a.ItemId == request.ItemId && a.IsCorrect))
             {
                 points = 0; // No double dipping
                 // Revert score add if logic strictly prevents it, but here just don't add more.
                 session.TotalScore -= question.PointsPerCorrectItem; // Revert
             }
        }
        else
        {
            session.WrongPlacements++;
        }

        // Record Attempt
        var attempt = new DragDropAttempt
        {
            SessionId = session.Id,
            ItemId = request.ItemId,
            PlacedInZoneId = request.DroppedInZoneId,
            IsCorrect = isCorrect,
            PointsEarned = points,
            AttemptTime = DateTime.UtcNow
        };

        await _attemptRepository.CreateAttemptAsync(attempt);
        await _sessionRepository.UpdateSessionAsync(session);

        // Check completion
        // If all items are correctly placed
        // We need to know unique correct items placed.
        var correctItemIds = session.Attempts.Where(a => a.IsCorrect).Select(a => a.ItemId).Distinct();
        bool isGameComplete = correctItemIds.Count() >= session.TotalItems;
        
        if (isGameComplete)
        {
            await _sessionRepository.CompleteSessionAsync(session.Id);
        }

        return new SubmitAttemptResponseDto
        {
            IsCorrect = isCorrect,
            PointsEarned = points,
            TotalScore = session.TotalScore,
            Message = isCorrect ? "Excellent!" : "Try again!",
            IsGameComplete = isGameComplete
        };
    }

    public async Task<GameResultDto> CompleteGameAsync(int sessionId)
    {
        var session = await _sessionRepository.GetSessionByIdAsync(sessionId);
        if (session == null) throw new KeyNotFoundException("Session not found");

        if (!session.IsCompleted)
        {
            await _sessionRepository.CompleteSessionAsync(sessionId);
        }

        // Calculate Stars
        int maxScore = session.TotalItems * session.DragDropQuestion.PointsPerCorrectItem;
        double percentage = maxScore > 0 ? (double)session.TotalScore / maxScore : 0;
        int stars = percentage >= 0.9 ? 3 : (percentage >= 0.6 ? 2 : 1);

        return new GameResultDto
        {
            SessionId = session.Id,
            TotalScore = session.TotalScore,
            MaxPossibleScore = maxScore,
            CorrectPlacements = session.CorrectPlacements,
            WrongPlacements = session.WrongPlacements,
            TimeSpentSeconds = session.TimeSpentSeconds,
            Stars = stars,
            BadgeUrl = $"/assets/badges/stars-{stars}.png"
        };
    }

    public async Task<GameSessionDto?> GetActiveSessionAsync(int studentId)
    {
        var session = await _sessionRepository.GetActiveSessionAsync(studentId);
        return session == null ? null : await MapToGameSessionDto(session);
    }

    private async Task<GameSessionDto> MapToGameSessionDto(DragDropGameSession session, DragDropQuestion? question = null)
    {
        // if question is null, use session.DragDropQuestion (checking if null)
        var q = question ?? session.DragDropQuestion;
        
        if (q == null)
        {
             // Should verify if loaded, if not load it
             q = await _questionRepository.GetByIdAsync(session.DragDropQuestionId, includeZonesAndItems: true);
        }

        // Map Items and Zones
        // We need to return items shuffled? or as is?
        // Usually shuffle is good, but FE can shuffle. Let's send as is.
        
        var zonesDto = _mapper.Map<List<DragDropZoneDto>>(q.Zones);
        var itemsDto = _mapper.Map<List<DragDropItemDto>>(q.Items);

        // Filter out already completed items? Or let FE handle it? 
        // FE needs to know which are completed to show them in zones.
        var completedItemIds = session.Attempts != null 
            ? session.Attempts.Where(a => a.IsCorrect).Select(a => a.ItemId).Distinct().ToList()
            : new List<int>();

        return new GameSessionDto
        {
            SessionId = session.Id,
            QuestionId = q.Id,
            GameTitle = q.GameTitle,
            Instructions = q.Instructions,
            TimeLimit = q.TimeLimit ?? 0,
            ShowImmediateFeedback = q.ShowImmediateFeedback,
            UITheme = q.UITheme,
            Zones = zonesDto,
            Items = itemsDto,
            CurrentScore = session.TotalScore,
            TimeElapsedSeconds = (int)(DateTime.UtcNow - session.StartTime).TotalSeconds,
            CompletedItemIds = completedItemIds
        };
    }
}
