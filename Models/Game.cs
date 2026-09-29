namespace TreasureHunt.Models;

public class Game
{
    public int Id { get; set; }
    public string PlayerName { get; set; } = string.Empty;
    public int Score { get; set; }
    public int CurrentClue { get; set; }
    public bool Completed { get; set; }
}