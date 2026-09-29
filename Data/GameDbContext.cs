using Microsoft.EntityFrameworkCore;
using TreasureHunt.Models;

namespace TreasureHunt.Data;

public class GameDbContext : DbContext
{
    public GameDbContext(DbContextOptions<GameDbContext> options)
        : base(options)
    {
    }

    public DbSet<Game> Games => Set<Game>();
}