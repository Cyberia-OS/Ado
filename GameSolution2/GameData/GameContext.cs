using System.Data.Entity;
using GameModels;

namespace GameData
{
    public class GameContext : DbContext
    {
        public GameContext() : base("name=GameContext")
        {
            Database.SetInitializer(new CreateDatabaseIfNotExists<GameContext>());
        }

        public DbSet<Game> Games { get; set; }
    }
}