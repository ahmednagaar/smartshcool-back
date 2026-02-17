using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Nafes.API.DTOs.FlipCard;
using Nafes.API.Modules;
using Nafes.API.Services.FlipCard;

namespace Nafes.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    [AllowAnonymous]  // Allow guest play - students can play with or without account
    public class FlipCardGameController : ControllerBase
    {
        private readonly IFlipCardGameService _service;

        public FlipCardGameController(IFlipCardGameService service)
        {
            _service = service;
        }

        [HttpPost("start")]
        public async Task<IActionResult> StartSession([FromBody] StartFlipCardGameDto dto)
        {
            try
            {
                var session = await _service.StartSessionAsync(dto);

                var response = new
                {
                    session.Id,
                    session.TotalPairs,
                    session.GameMode,
                    Question = new
                    {
                        session.FlipCardQuestion.Id,
                        session.FlipCardQuestion.GameTitle,
                        session.FlipCardQuestion.Instructions,
                        session.FlipCardQuestion.TimerMode,
                        session.FlipCardQuestion.TimeLimitSeconds,
                        session.FlipCardQuestion.ShowHints,
                        session.FlipCardQuestion.MaxHints,
                        session.FlipCardQuestion.UITheme,
                        session.FlipCardQuestion.CardBackDesign,
                        session.FlipCardQuestion.CustomCardBackUrl,
                        session.FlipCardQuestion.EnableAudio,
                        session.FlipCardQuestion.EnableExplanations,
                        session.FlipCardQuestion.NumberOfPairs,
                        Cards = ShuffleAndMaskCards(session.FlipCardQuestion.Pairs)
                    }
                };

                return Ok(response);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        private List<GameCardDto> ShuffleAndMaskCards(List<FlipCardPair> pairs)
        {
            var cards = new List<GameCardDto>();
            var random = new Random();

            foreach (var pair in pairs)
            {
                // Card 1
                cards.Add(new GameCardDto
                {
                    Id = $"card-{pair.Id}-{1}",
                    PairId = pair.Id,
                    CardNumber = 1,
                    Type = pair.Card1Type,
                    Text = pair.Card1Text,
                    ImageUrl = pair.Card1ImageUrl,
                    AudioUrl = pair.Card1AudioUrl
                });

                // Card 2
                cards.Add(new GameCardDto
                {
                    Id = $"card-{pair.Id}-{2}",
                    PairId = pair.Id,
                    CardNumber = 2,
                    Type = pair.Card2Type,
                    Text = pair.Card2Text,
                    ImageUrl = pair.Card2ImageUrl,
                    AudioUrl = pair.Card2AudioUrl
                });
            }

            return cards.OrderBy(c => random.Next()).ToList();
        }

        [HttpPost("match")]
        public async Task<IActionResult> RecordMatch([FromBody] RecordMatchDto dto)
        {
            try
            {
                var result = await _service.RecordMatchAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("wrong-match")]
        public async Task<IActionResult> RecordWrongMatch([FromBody] RecordWrongMatchDto dto)
        {
             try
            {
                var result = await _service.RecordWrongMatchAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return NotFound(new { message = ex.Message });
            }
        }

        [HttpPost("hint")]
        public async Task<IActionResult> GetHint([FromBody] GetHintDto dto)
        {
            try
            {
                var result = await _service.GetHintAsync(dto);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("complete")]
        public async Task<IActionResult> CompleteSession([FromBody] CompleteSessionRequest request)
        {
            try
            {
                var result = await _service.CompleteSessionAsync(request.SessionId);
                return Ok(result);
            }
            catch (Exception)
            {
                return NotFound();
            }
        }
    }

    public class CompleteSessionRequest
    {
        public int SessionId { get; set; }
    }
}
