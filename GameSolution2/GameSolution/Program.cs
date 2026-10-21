using System;
using System.Collections.Generic;
using System.Linq;
using GameData;
using GameModels;

namespace GameConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("лабораторная работа: игры (ef6)\n");

            EnsureDatabase();
            ShowAllGames();
            Task1_SearchDemo();
            Task2_AnalyticsDemo();
            Task3_CrudDemo();

            Console.WriteLine("\nвсе игры после crud:");
            ShowAllGames();

            Console.WriteLine("\nнажмите enter для выхода...");
            Console.ReadLine();
        }

        static void EnsureDatabase()
        {
            using (var db = new GameContext())
            {
                if (db.Database.CreateIfNotExists())
                    Console.WriteLine("база данных создана");
                else
                    Console.WriteLine("база данных уже существует");

                bool needSeed = !db.Games.Any() || db.Games.All(g => g.CopiesSold == 0);

                if (needSeed)
                {
                    if (db.Games.Any())
                    {
                        db.Games.RemoveRange(db.Games.ToList());
                        db.SaveChanges();
                        Console.WriteLine("старые данные очищены");
                    }
                    SeedData(db);
                    Console.WriteLine($"добавлено {db.Games.Count()} игр");
                }
                else
                {
                    Console.WriteLine($"в бд уже {db.Games.Count()} игр");
                }
            }
        }

        static void SeedData(GameContext db)
        {
            db.Games.AddRange(new[]
            {
                new Game { Title = "The Witcher 3", Studio = "CD Projekt Red", Genre = "RPG", ReleaseDate = new DateTime(2015, 5, 19), Mode = GameMode.SinglePlayer, CopiesSold = 50_000_000 },
                new Game { Title = "GTA V", Studio = "Rockstar North", Genre = "Action", ReleaseDate = new DateTime(2013, 9, 17), Mode = GameMode.Both, CopiesSold = 185_000_000 },
                new Game { Title = "Minecraft", Studio = "Mojang", Genre = "Sandbox", ReleaseDate = new DateTime(2011, 11, 18), Mode = GameMode.Both, CopiesSold = 300_000_000 },
                new Game { Title = "CS:GO", Studio = "Valve", Genre = "Shooter", ReleaseDate = new DateTime(2012, 8, 21), Mode = GameMode.Multiplayer, CopiesSold = 40_000_000 },
                new Game { Title = "Cyberpunk 2077", Studio = "CD Projekt Red", Genre = "RPG", ReleaseDate = new DateTime(2020, 12, 10), Mode = GameMode.SinglePlayer, CopiesSold = 25_000_000 },
                new Game { Title = "Among Us", Studio = "InnerSloth", Genre = "Party", ReleaseDate = new DateTime(2018, 6, 15), Mode = GameMode.Both, CopiesSold = 5_000_000 },
            });
            db.SaveChanges();
        }

        static void ShowAllGames()
        {
            using (var db = new GameContext())
            {
                var games = db.Games.OrderBy(g => g.ReleaseDate).ToList();
                PrintGames(games, "все игры");
            }
        }

        static void Task1_SearchDemo()
        {
            Console.WriteLine("\nзадание 1: поиск");

            Console.WriteLine("\nпо названию 'Witcher':");
            SearchByTitle("Witcher");

            Console.WriteLine("\nпо студии 'CD Projekt Red':");
            SearchByStudio("CD Projekt Red");

            Console.WriteLine("\nпо студии и названию:");
            SearchByStudioAndTitle("CD Projekt Red", "Witcher 3");

            Console.WriteLine("\nпо стилю 'RPG':");
            SearchByGenre("RPG");

            Console.WriteLine("\nпо году 2013:");
            SearchByReleaseYear(2013);
        }

        static void SearchByTitle(string title)
        {
            using (var db = new GameContext())
            {
                var results = db.Games
                    .Where(g => g.Title.ToLower().Contains(title.ToLower()))
                    .ToList();
                PrintGames(results, $"по названию '{title}'");
            }
        }

        static void SearchByStudio(string studio)
        {
            using (var db = new GameContext())
            {
                var results = db.Games
                    .Where(g => g.Studio.ToLower().Contains(studio.ToLower()))
                    .ToList();
                PrintGames(results, $"по студии '{studio}'");
            }
        }

        static void SearchByStudioAndTitle(string studio, string title)
        {
            using (var db = new GameContext())
            {
                var results = db.Games
                    .Where(g => g.Studio.ToLower().Contains(studio.ToLower())
                        && g.Title.ToLower().Contains(title.ToLower()))
                    .ToList();
                PrintGames(results, "по студии и названию");
            }
        }

        static void SearchByGenre(string genre)
        {
            using (var db = new GameContext())
            {
                var results = db.Games
                    .Where(g => g.Genre.ToLower() == genre.ToLower())
                    .ToList();
                PrintGames(results, $"по стилю '{genre}'");
            }
        }

        static void SearchByReleaseYear(int year)
        {
            using (var db = new GameContext())
            {
                var results = db.Games
                    .Where(g => g.ReleaseDate.Year == year)
                    .ToList();
                PrintGames(results, $"по году {year}");
            }
        }

        static void Task2_AnalyticsDemo()
        {
            Console.WriteLine("\nзадание 2: аналитика");
            using (var db = new GameContext())
            {
                Console.WriteLine("\nоднопользовательские:");
                var sp = db.Games
                    .Where(g => g.Mode == GameMode.SinglePlayer || g.Mode == GameMode.Both)
                    .OrderBy(g => g.Title)
                    .ToList();
                PrintGames(sp, "однопользовательские");

                Console.WriteLine("\nмногопользовательские:");
                var mp = db.Games
                    .Where(g => g.Mode == GameMode.Multiplayer || g.Mode == GameMode.Both)
                    .OrderBy(g => g.Title)
                    .ToList();
                PrintGames(mp, "многопользовательские");

                Console.WriteLine("\nмаксимум продаж:");
                var maxSold = db.Games.OrderByDescending(g => g.CopiesSold).FirstOrDefault();
                if (maxSold != null)
                    Console.WriteLine($"  {maxSold.Title} ({maxSold.Studio}) - {maxSold.CopiesSold:N0}");

                Console.WriteLine("\nминимум продаж:");
                var minSold = db.Games.OrderBy(g => g.CopiesSold).FirstOrDefault();
                if (minSold != null)
                    Console.WriteLine($"  {minSold.Title} ({minSold.Studio}) - {minSold.CopiesSold:N0}");

                Console.WriteLine("\nтоп-3 по продажам:");
                var top3Best = db.Games.OrderByDescending(g => g.CopiesSold).Take(3).ToList();
                PrintGames(top3Best, "топ-3");

                Console.WriteLine("\nтоп-3 непродаваемых:");
                var top3Worst = db.Games.OrderBy(g => g.CopiesSold).Take(3).ToList();
                PrintGames(top3Worst, "топ-3 худших");
            }
        }

        static void Task3_CrudDemo()
        {
            Console.WriteLine("\nзадание 3: crud");

            Console.WriteLine("\nдобавление:");
            AddGame("Hades", "Supergiant Games", "Roguelike",
                new DateTime(2020, 9, 17), GameMode.SinglePlayer, 3_000_000);

            Console.WriteLine("\nпроверка дубликата:");
            AddGame("Hades", "Supergiant Games", "Roguelike",
                new DateTime(2020, 9, 17), GameMode.SinglePlayer, 3_000_000);

            Console.WriteLine("\nизменение:");
            UpdateGame("Hades", "Supergiant Games",
                newGenre: "Action-Roguelike", newCopiesSold: 5_500_000);

            Console.WriteLine("\nудаление:");
            DeleteGame("Hades", "Supergiant Games");
        }

        static bool AddGame(string title, string studio, string genre,
            DateTime releaseDate, GameMode mode, int copiesSold)
        {
            using (var db = new GameContext())
            {
                var existing = db.Games.FirstOrDefault(g => g.Title.ToLower() == title.ToLower());

                if (existing != null)
                {
                    Console.WriteLine($"  игра '{title}' уже существует (id={existing.Id}). отмена");
                    return false;
                }

                var game = new Game
                {
                    Title = title,
                    Studio = studio,
                    Genre = genre,
                    ReleaseDate = releaseDate,
                    Mode = mode,
                    CopiesSold = copiesSold
                };
                db.Games.Add(game);
                db.SaveChanges();
                Console.WriteLine($"  добавлена игра '{title}' (id={game.Id})");
                return true;
            }
        }

        static bool UpdateGame(string title, string studio,
            string newTitle = null, string newStudio = null, string newGenre = null,
            DateTime? newReleaseDate = null, GameMode? newMode = null, int? newCopiesSold = null)
        {
            using (var db = new GameContext())
            {
                var game = db.Games.FirstOrDefault(g => g.Title.ToLower() == title.ToLower()
                    && g.Studio.ToLower() == studio.ToLower());

                if (game == null)
                {
                    Console.WriteLine($"  игра '{title}' от '{studio}' не найдена");
                    return false;
                }

                if (newTitle != null) game.Title = newTitle;
                if (newStudio != null) game.Studio = newStudio;
                if (newGenre != null) game.Genre = newGenre;
                if (newReleaseDate.HasValue) game.ReleaseDate = newReleaseDate.Value;
                if (newMode.HasValue) game.Mode = newMode.Value;
                if (newCopiesSold.HasValue) game.CopiesSold = newCopiesSold.Value;

                db.SaveChanges();
                Console.WriteLine($"  игра '{game.Title}' обновлена (id={game.Id})");
                return true;
            }
        }

        static bool DeleteGame(string title, string studio)
        {
            using (var db = new GameContext())
            {
                var game = db.Games.FirstOrDefault(g => g.Title.ToLower() == title.ToLower()
                    && g.Studio.ToLower() == studio.ToLower());

                if (game == null)
                {
                    Console.WriteLine($"  игра '{title}' от '{studio}' не найдена");
                    return false;
                }

                Console.WriteLine($"  найдена: {game.Title} ({game.Studio}, {game.Genre}, {game.ReleaseDate:dd.MM.yyyy})");
                Console.Write($"  удалить '{game.Title}'? (y/n): ");
                var answer = Console.ReadLine()?.Trim().ToLower();
                bool confirmed = answer == "y" || answer == "yes" || answer == "д" || answer == "да";

                if (!confirmed)
                {
                    Console.WriteLine("  отмена");
                    return false;
                }

                db.Games.Remove(game);
                db.SaveChanges();
                Console.WriteLine($"  игра '{game.Title}' удалена");
                return true;
            }
        }

        static void PrintGames(List<Game> games, string context)
        {
            if (games.Count == 0)
            {
                Console.WriteLine($"  нет игр ({context})");
                return;
            }
            foreach (var g in games)
                Console.WriteLine($"  {g.Id}. {g.Title,-18} | {g.Studio,-15} | {g.Genre,-15} | {g.ReleaseDate:dd.MM.yyyy} | {g.Mode,-12} | {g.CopiesSold,12:N0}");
        }
    }
}