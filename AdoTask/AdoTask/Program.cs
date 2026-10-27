using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace SpainChampionshipApp
{
    public class Team
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public ICollection<Player> Players { get; set; } = new List<Player>();
    }

    public class Player
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public int TeamId { get; set; }
        public Team Team { get; set; }
    }

    public class Match
    {
        public int Id { get; set; }
        public int HomeTeamId { get; set; }
        public int AwayTeamId { get; set; }
        public int HomeGoals { get; set; }
        public int AwayGoals { get; set; }
        public DateTime MatchDate { get; set; }
    }

    public class ScorerResult
    {
        public int Id { get; set; }
        public string PlayerName { get; set; }
        public string TeamName { get; set; }
        public int GoalsCount { get; set; }
    }

    public class TeamGoalsResult
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public int GoalsScored { get; set; }
        public int GoalsConceded { get; set; }
    }

    public class LeagueTableResult
    {
        public int Id { get; set; }
        public string TeamName { get; set; }
        public int Points { get; set; }
    }

    public class ChampionshipContext : DbContext
    {
        public DbSet<Team> Teams { get; set; }
        public DbSet<Player> Players { get; set; }
        public DbSet<Match> Matches { get; set; }

        //результаты SQL Представлений
        public DbSet<ScorerResult> ScorerResults { get; set; }
        public DbSet<TeamGoalsResult> TeamGoalsResults { get; set; }
        public DbSet<LeagueTableResult> LeagueTableResults { get; set; }

        public ChampionshipContext() { }
        public ChampionshipContext(DbContextOptions<ChampionshipContext> options) : base(options) { }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                optionsBuilder.UseSqlServer(@"Server=(localdb)\MSSQLLocalDB;Database=SpainChampionship;Trusted_Connection=True;TrustServerCertificate=True;");
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //маппинг классов на созданные нами в SSMS представления 
            modelBuilder.Entity<ScorerResult>().HasNoKey().ToView("v_AllScorers");
            modelBuilder.Entity<TeamGoalsResult>().HasNoKey().ToView("v_TeamGoalsStats");
            modelBuilder.Entity<LeagueTableResult>().HasNoKey().ToView("v_LeagueTable");
        }
    }

    public class TestChampionshipContext : ChampionshipContext
    {
        public TestChampionshipContext(DbContextOptions<ChampionshipContext> options) : base(options) { }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder) { }
    }

    //точка входа и логика программы
    class Program
    {
        static async Task Main()
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            //работа с реальной базой данных
            using (ChampionshipContext db = new ChampionshipContext())
            {
                Console.WriteLine("задание 1: Генерация матчей");
                Console.WriteLine("Вызываем sp_GenerateRandomMatches для создания 10 случайных матчей...");
                await db.Database.ExecuteSqlRawAsync("EXEC sp_GenerateRandomMatches @p0", 10);
                Console.WriteLine("Матчи и голы успешно добавлены в базу данных!\n");

                Console.WriteLine("задание 2: Статистика бомбардиров");
                //топ-3 бомбардиров команды с Id = 1 (Реал Мадрид) через хранимую процедуру
                var topTeamScorers = await db.ScorerResults
                    .FromSqlRaw("EXEC sp_GetTopScorersByTeam @TeamId = {0}, @TopCount = {1}", 1, 3)
                    .ToListAsync();
                Console.WriteLine("Tоп-3 бомбардиров Реал Мадрид:");
                foreach (var s in topTeamScorers) Console.WriteLine("  - " + s.PlayerName + ": " + s.GoalsCount + " голов");

                //лучший бомбардир всего чемпионата через View
                var topScorer = await db.ScorerResults.OrderByDescending(s => s.GoalsCount).FirstOrDefaultAsync();
                Console.WriteLine("Лучший бомбардир Ла Лиги: " + (topScorer != null ? topScorer.PlayerName : "Нет") + " — " + (topScorer != null ? topScorer.GoalsCount : 0) + " голов\n");

                Console.WriteLine("задание 3: Забитые и пропущенные голы");
                //топ-3 самых забивающих команд через View
                var topScoring = await db.TeamGoalsResults.OrderByDescending(t => t.GoalsScored).Take(3).ToListAsync();
                Console.WriteLine("Топ-3 забивающих команд:");
                foreach (var t in topScoring) Console.WriteLine("  - " + t.TeamName + ": " + t.GoalsScored + " голов");

                //команда с лучшей защитой (меньше всего пропустила)
                var bestDefense = await db.TeamGoalsResults.OrderBy(t => t.GoalsConceded).FirstOrDefaultAsync();
                Console.WriteLine("Меньше всех пропустила команда: " + (bestDefense != null ? bestDefense.TeamName : "Нет") + " (" + (bestDefense != null ? bestDefense.GoalsConceded : 0) + " голов)\n");

                Console.WriteLine("задание 4: Таблица очков");
                //топ-3 команд по очкам
                var leaders = await db.LeagueTableResults.OrderByDescending(l => l.Points).Take(3).ToListAsync();
                Console.WriteLine("Лидеры чемпионата (Топ-3):");
                foreach (var l in leaders) Console.WriteLine("  - " + l.TeamName + ": " + l.Points + " очков");
            }

            //автоматический тест 
            Console.WriteLine("\nзадание 5: Запуск теста EF Core");
            await RunInMemoryTest();

            Console.WriteLine("\nНажмите Enter для выхода");
            Console.ReadLine();
        }

        //логика тестирования без создания отдельного проекта
        static async Task RunInMemoryTest()
        {
            var options = new DbContextOptionsBuilder<ChampionshipContext>()
                .UseInMemoryDatabase(databaseName: "ChampionshipTestDb_" + Guid.NewGuid().ToString())
                .Options;

            using (TestChampionshipContext testDb = new TestChampionshipContext(options))
            {
                //подготовка данных (Arrange)
                var team1 = new Team { Id = 1, Name = "Test Real" };
                var team2 = new Team { Id = 2, Name = "Test Barca" };
                testDb.Teams.AddRange(team1, team2);

                //добавляем победу первой команды (2:0)
                testDb.Matches.Add(new Match { HomeTeamId = 1, AwayTeamId = 2, HomeGoals = 2, AwayGoals = 0, MatchDate = DateTime.Now });
                await testDb.SaveChangesAsync();

                //выполнение расчета (Act)
                var points = await testDb.Matches
                    .Where(m => m.HomeTeamId == 1)
                    .Select(m => m.HomeGoals > m.AwayGoals ? 3 : (m.HomeGoals == m.AwayGoals ? 1 : 0))
                    .FirstOrDefaultAsync();

                //проверка (Assert)
                if (points == 3)
                {
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[ТЕСТ ПРОЙДЕН] Логика EF Core начислила 3 очка за победу.");
                }
                else
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("[ТЕСТ ПРОВАЛЕН] Вместо 3 очков начислено: " + points);
                }
                Console.ForegroundColor = ConsoleColor.Gray;
            }
        }
    }
}
