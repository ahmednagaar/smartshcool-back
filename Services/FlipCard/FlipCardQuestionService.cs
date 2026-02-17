using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nafes.API.DTOs.FlipCard;
using Nafes.API.Modules;
using Nafes.API.Repositories.FlipCard;

namespace Nafes.API.Services.FlipCard
{
    public class FlipCardQuestionService : IFlipCardQuestionService
    {
        private readonly IFlipCardQuestionRepository _repository;
        // private readonly ISanitizationService _sanitizer; // Assuming it exists

        public FlipCardQuestionService(IFlipCardQuestionRepository repository)
        {
            _repository = repository;
        }

        public async Task<FlipCardQuestion> CreateQuestionAsync(CreateFlipCardQuestionDto dto)
        {
            if (dto.NumberOfPairs < 4 || dto.NumberOfPairs > 12)
                throw new Exception("Number of pairs must be between 4 and 12");

            if (dto.Pairs.Count != dto.NumberOfPairs)
                throw new Exception($"Must provide exactly {dto.NumberOfPairs} pairs");

            var question = new FlipCardQuestion
            {
                Grade = (GradeLevel)dto.GradeId,
                Subject = (SubjectType)dto.SubjectId,
                GameTitle = dto.GameTitle,
                Instructions = dto.Instructions,
                GameMode = dto.GameMode,
                NumberOfPairs = dto.NumberOfPairs,
                DifficultyLevel = dto.DifficultyLevel,
                TimerMode = dto.TimerMode,
                TimeLimitSeconds = dto.TimeLimitSeconds,
                PointsPerMatch = dto.PointsPerMatch,
                MovePenalty = dto.MovePenalty,
                ShowHints = dto.ShowHints,
                MaxHints = dto.MaxHints,
                UITheme = dto.UITheme,
                CardBackDesign = dto.CardBackDesign,
                CustomCardBackUrl = dto.CustomCardBackUrl,
                EnableAudio = dto.EnableAudio,
                EnableExplanations = dto.EnableExplanations,
                Category = dto.Category,
                DisplayOrder = dto.DisplayOrder,
                CreatedDate = DateTime.UtcNow,
                IsActive = true,
                Pairs = dto.Pairs.Select((p, index) => new FlipCardPair
                {
                    Card1Type = p.Card1Type,
                    Card1Text = p.Card1Text,
                    Card1ImageUrl = p.Card1ImageUrl,
                    Card1AudioUrl = p.Card1AudioUrl,
                    Card2Type = p.Card2Type,
                    Card2Text = p.Card2Text,
                    Card2ImageUrl = p.Card2ImageUrl,
                    Card2AudioUrl = p.Card2AudioUrl,
                    Explanation = p.Explanation,
                    PairOrder = index + 1,
                    DifficultyWeight = p.DifficultyWeight ?? 5
                }).ToList()
            }; // Sanitize inputs if SanitizationService is injected

            return await _repository.CreateAsync(question);
        }

        public async Task<FlipCardQuestion> UpdateQuestionAsync(int id, UpdateFlipCardQuestionDto dto)
        {
            var question = await _repository.GetByIdAsync(id, includePairs: true);
            if (question == null) throw new Exception("Question not found"); // Use specific NotFoundException

            // Update fields
            question.GameTitle = dto.GameTitle;
            question.Instructions = dto.Instructions;
            question.GameMode = dto.GameMode;
            question.NumberOfPairs = dto.NumberOfPairs;
            question.DifficultyLevel = dto.DifficultyLevel;
            question.TimerMode = dto.TimerMode;
            question.TimeLimitSeconds = dto.TimeLimitSeconds;
            question.PointsPerMatch = dto.PointsPerMatch;
            question.MovePenalty = dto.MovePenalty;
            question.ShowHints = dto.ShowHints;
            question.MaxHints = dto.MaxHints;
            question.UITheme = dto.UITheme;
            question.CardBackDesign = dto.CardBackDesign;
            question.CustomCardBackUrl = dto.CustomCardBackUrl;
            question.EnableAudio = dto.EnableAudio;
            question.EnableExplanations = dto.EnableExplanations;
            question.Category = dto.Category;
            question.DisplayOrder = dto.DisplayOrder;
            question.LastModifiedDate = DateTime.UtcNow;

            // Update Pairs - naive approach: remove all and Add new
            // Better approach: track IDs but here we can just replace as Pairs are owned by Question
            question.Pairs.Clear(); 
            // In EF Core, clearing the list and adding new might cause issues if not handling deletions explicitly, 
            // but with Cascade delete it should work on SaveChanges if tracked?
            // Safer to use repo logic or Mapper, but manual mapping here:
            
            question.Pairs = dto.Pairs.Select((p, index) => new FlipCardPair
            {
                FlipCardQuestionId = id,
                Card1Type = p.Card1Type,
                Card1Text = p.Card1Text,
                Card1ImageUrl = p.Card1ImageUrl,
                Card1AudioUrl = p.Card1AudioUrl,
                Card2Type = p.Card2Type,
                Card2Text = p.Card2Text,
                Card2ImageUrl = p.Card2ImageUrl,
                Card2AudioUrl = p.Card2AudioUrl,
                Explanation = p.Explanation,
                PairOrder = index + 1,
                DifficultyWeight = p.DifficultyWeight ?? 5
            }).ToList();

            return await _repository.UpdateAsync(question);
        }

        public async Task<IEnumerable<FlipCardQuestion>> GetByGradeAndSubjectAsync(int gradeId, int subjectId)
        {
            return await _repository.GetByGradeAndSubjectAsync(gradeId, subjectId);
        }

        public async Task<FlipCardQuestion?> GetByIdAsync(int id, bool includePairs = false)
        {
            return await _repository.GetByIdAsync(id, includePairs);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            return await _repository.DeleteAsync(id);
        }

        public async Task<object> GetAllPaginatedAsync(int page, int pageSize, string? searchTerm = null)
        {
            var items = await _repository.GetAllPaginatedAsync(page, pageSize, searchTerm);
            // Need total count for real pagination metadata
            return new { items }; 
        }

        public async Task<int> GetQuestionCountAsync(int gradeId, int subjectId)
        {
            return await _repository.GetQuestionCountAsync(gradeId, subjectId);
        }

        public async Task<IEnumerable<string>> GetCategoriesAsync(int? gradeId = null, int? subjectId = null)
        {
            return await _repository.GetCategoriesAsync(gradeId, subjectId);
        }

        public async Task<FlipCardQuestion?> GetRandomQuestionAsync(int gradeId, int subjectId, int? difficultyLevel = null)
        {
            return await _repository.GetRandomQuestionAsync(gradeId, subjectId, difficultyLevel);
        }
    }
}
