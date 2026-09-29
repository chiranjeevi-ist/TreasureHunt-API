using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using TreasureHunt.Data;
using TreasureHunt.Models;

namespace TreasureHunt.Controllers;

[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/game")]
public class GameController : ControllerBase
{
    private readonly GameDbContext _context;

    public GameController(GameDbContext context)
    {
        _context = context;
    }

    // Start a new game
    [HttpPost("start")]
    public async Task<IActionResult> StartGame(string playerName)
    {
        if (string.IsNullOrWhiteSpace(playerName))
        {
            return BadRequest("Player name is required.");
        }

        var game = new Game
        {
            PlayerName = playerName.Trim(),
            Score = 0,
            CurrentClue = 1,
            Completed = false
        };

        _context.Games.Add(game);

        await _context.SaveChangesAsync();

        return Ok(game);
    }

    // Get game details
    [HttpGet("{id}")]
    public async Task<IActionResult> GetGame(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound("Game not found.");
        }

        return Ok(game);
    }

    // Solve current clue
    [HttpPost("{id}/solve")]
    public async Task<IActionResult> SolveClue(int id, string answer)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound("Game not found.");
        }

        // Prevent submitting answers after completing the game
        if (game.Completed)
        {
            return BadRequest("Game already completed.");
        }

        // Validate answer
        if (string.IsNullOrWhiteSpace(answer))
        {
            return BadRequest("Please enter an answer.");
        }

        // Remove spaces and ignore capitalization
        var userAnswer = answer.Trim().ToLowerInvariant();

        // Get the correct answer based on the current clue
        var correctAnswer = game.CurrentClue switch
        {
            1 => "keyboard",
            2 => "clock",
            3 => "future",
            _ => ""
        };

        // Check answer
        if (userAnswer != correctAnswer)
        {
            return BadRequest("Wrong answer. Try again!");
        }

        // Correct answer
        game.Score += 100;
        game.CurrentClue++;

        // Complete game after clue 3
        if (game.CurrentClue > 3)
        {
            game.Completed = true;
        }

        await _context.SaveChangesAsync();

        return Ok(new
        {
            message = game.Completed
                ? "🎉 Congratulations! You found the treasure!"
                : "Correct! You found the clue.",

            game.Score,
            game.CurrentClue,
            game.Completed
        });
    }

    // Get player's score
    [HttpGet("{id}/score")]
    public async Task<IActionResult> GetScore(int id)
    {
        var game = await _context.Games.FindAsync(id);

        if (game == null)
        {
            return NotFound("Game not found.");
        }

        return Ok(new
        {
            game.PlayerName,
            game.Score,
            game.Completed
        });
    }
}