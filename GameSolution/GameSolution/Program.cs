using System;
using System.Linq;
using GameData;
using GameModels;

namespace GameConsoleApp
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("ЛАБОРАТОРНАЯ РАБОТА: CODE FIRST\n");

            using (var db = new GameContext())
            {
                // 1. Создать БД 
                if (db.Database.CreateIfNotExists())
                    Console.WriteLine("База данных 'GameDb' создана.");
                else
                    Console.WriteLine("База данных 'GameDb' уже существует.");

                // 2. Добавить игры
                if (!db.Games.Any())
                {
                    db.Games.Add(new Game
                    {
                        Title = "The Witcher 3",
                        Studio = "CD Projekt Red",
                        Genre = "RPG",
                        ReleaseDate = new DateTime(2015, 5, 19)
                    });
                    db.Games.Add(new Game
                    {
                        Title = "GTA V",
                        Studio = "Rockstar North",
                        Genre = "Action",
                        ReleaseDate = new DateTime(2013, 9, 17)
                    });
                    db.Games.Add(new Game
                    {
                        Title = "Minecraft",
                        Studio = "Mojang",
                        Genre = "Sandbox",
                        ReleaseDate = new DateTime(2011, 11, 18)
                    });
                    db.SaveChanges();
                    Console.WriteLine("Добавлено 3 игры");
                }
                else
                {
                    Console.WriteLine("В БД уже есть записи, добавление пропущено.");
                }

                // 3. Показать все игры
                Console.WriteLine("\nСписок игр в БД");
                foreach (var g in db.Games.OrderBy(x => x.ReleaseDate).ToList())
                {
                    Console.WriteLine($"{g.Id}. {g.Title,-20} | {g.Studio,-15} | {g.Genre,-10} | {g.ReleaseDate:dd.MM.yyyy}");
                }
            }

            Console.WriteLine("\nНажмите Enter для выхода");
            Console.ReadLine();
        }
    }
}