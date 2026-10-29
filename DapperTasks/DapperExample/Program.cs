using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperModule7
{
    public class Buyer
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public DateTime BirthDate { get; set; }
        public string Gender { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
    }

    public class Section
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }

    public class PromoProduct
    {
        public int Id { get; set; }
        public string ProductName { get; set; } = string.Empty;
        public int SectionId { get; set; }
        public string Country { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    class Program
    {
        private static readonly string connectionString =
            "Server=(localdb)\\MSSQLLocalDB;Database=PromoMailingDB;Integrated Security=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // ЗАДАНИЕ 2: Проверка подключения при старте приложения
            Console.WriteLine("Попытка подключения к базе данных «Список рассылки»...");
            if (!TestConnection())
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("\n[ОШИБКА] Не удалось подключиться к базе данных. Проверьте строку подключения или работу SQL Server.");
                Console.ResetColor();
                Console.ReadLine();
                return;
            }

            while (true)
            {
                Console.Clear();
                Console.ForegroundColor = ConsoleColor.Cyan;
                Console.WriteLine("СИСТЕМА УПРАВЛЕНИЯ СПИСКАМИ РАССЫЛОК");
                Console.ResetColor();
                Console.WriteLine("1. Отобразить всех покупателей");
                Console.WriteLine("2. Отобразить Email всех покупателей");
                Console.WriteLine("3. Отобразить список разделов интересов");
                Console.WriteLine("4. Отобразить список акционных товаров");
                Console.WriteLine("5. Отобразить список всех городов");
                Console.WriteLine("6. Отобразить список всех стран");
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("7. [Фильтр] Покупатели из конкретного города");
                Console.WriteLine("8. [Фильтр] Покупатели из конкретной страны");
                Console.WriteLine("9. [Фильтр] Акции для конкретной страны");
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("0. Выход из программы (Отключение от БД)");
                Console.ResetColor();
                Console.Write("\nВыберите пункт меню: ");

                string? choice = Console.ReadLine();
                Console.WriteLine();

                switch (choice)
                {
                    case "1": ShowAllBuyers(); break;
                    case "2": ShowAllEmails(); break;
                    case "3": ShowAllSections(); break;
                    case "4": ShowAllPromoProducts(); break;
                    case "5": ShowAllCities(); break;
                    case "6": ShowAllCountries(); break;
                    case "7": ShowBuyersByCity(); break;
                    case "8": ShowBuyersByCountry(); break;
                    case "9": ShowPromosByCountry(); break;
                    case "0":
                        Console.WriteLine("Отключение от базы данных. До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный пункт меню. Нажмите Enter для повтора...");
                        Console.ReadLine();
                        break;
                }
            }
        }

        // Реализация ЗАДАНИЯ 2: Подключение и проверка
        private static bool TestConnection()
        {
            try
            {
                using (IDbConnection db = new SqlConnection(connectionString))
                {
                    db.Open(); 
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("[УСПЕШНО] Подключение к БД установлено.");
                    Console.ResetColor();
                    Console.WriteLine("Нажмите Enter для перехода к главному меню...");
                    Console.ReadLine();
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Детали ошибки: {ex.Message}");
                return false;
            }
        }

        // ЗАДАНИЕ 3: Вывод всех покупателей
        private static void ShowAllBuyers()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var buyers = db.Query<Buyer>("SELECT * FROM Buyers").ToList();
                Console.WriteLine($"--- Список всех покупателей ({buyers.Count}): ---");
                foreach (var b in buyers)
                {
                    Console.WriteLine($"ID: {b.Id} | {b.FullName} | Рожд: {b.BirthDate:dd.MM.yyyy} | Пол: {b.Gender} | Email: {b.Email} | Локация: {b.Country}, {b.City}");
                }
            }
            WaitKey();
        }

        // ЗАДАНИЕ 3: Вывод только Email покупателей
        private static void ShowAllEmails()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                // Dapper автоматически маппит простые типы (string) напрямую
                var emails = db.Query<string>("SELECT Email FROM Buyers").ToList();
                Console.WriteLine("--- Email адреса покупателей: ---");
                foreach (var email in emails)
                {
                    Console.WriteLine($"- {email}");
                }
            }
            WaitKey();
        }

        // ЗАДАНИЕ 3: Вывод списка разделов
        private static void ShowAllSections()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var sections = db.Query<Section>("SELECT * FROM Sections").ToList();
                Console.WriteLine("--- Список категорий/разделов: ---");
                foreach (var s in sections)
                {
                    Console.WriteLine($"ID: {s.Id} | Наименование: {s.Name}");
                }
            }
            WaitKey();
        }

        // ЗАДАНИЕ 3: Вывод всех акционных товаров
        private static void ShowAllPromoProducts()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var products = db.Query<PromoProduct>("SELECT * FROM PromoProducts").ToList();
                Console.WriteLine("--- Все акционные товары: ---");
                foreach (var p in products)
                {
                    Console.WriteLine($"[{p.Country}] {p.ProductName} (Период: {p.StartDate:dd.MM.yyyy} - {p.EndDate:dd.MM.yyyy})");
                }
            }
            WaitKey();
        }

        // ЗАДАНИЕ 3: Вывод уникальных городов
        private static void ShowAllCities()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var cities = db.Query<string>("SELECT DISTINCT City FROM Buyers").ToList();
                Console.WriteLine("--- Список городов присутствия покупателей: ---");
                foreach (var city in cities) Console.WriteLine($"- {city}");
            }
            WaitKey();
        }

        // ЗАДАНИЕ 3: Вывод уникальных стран
        private static void ShowAllCountries()
        {
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var countries = db.Query<string>("SELECT DISTINCT Country FROM Buyers").ToList();
                Console.WriteLine("--- Список стран присутствия покупателей: ---");
                foreach (var country in countries) Console.WriteLine($"- {country}");
            }
            WaitKey();
        }

        // ЗАДАНИЕ 4: Фильтрация покупателей по городу (Безопасный параметризованный SQL)
        private static void ShowBuyersByCity()
        {
            Console.Write("Введите название города для поиска: ");
            string? city = Console.ReadLine();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var buyers = db.Query<Buyer>("SELECT * FROM Buyers WHERE City = @CityName", new { CityName = city }).ToList();
                Console.WriteLine($"\n--- Покупатели из города {city} ({buyers.Count}): ---");
                foreach (var b in buyers) Console.WriteLine($"- {b.FullName} ({b.Email})");
            }
            WaitKey();
        }

        // ЗАДАНИЕ 4: Фильтрация покупателей по стране
        private static void ShowBuyersByCountry()
        {
            Console.Write("Введите название страны для поиска: ");
            string? country = Console.ReadLine();

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var buyers = db.Query<Buyer>("SELECT * FROM Buyers WHERE Country = @CountryName", new { CountryName = country }).ToList(); Console.WriteLine($"\n--- Покупатели из страны {country} ({buyers.Count}): ---");
                foreach (var b in buyers) Console.WriteLine($"- {b.FullName} | Город: {b.City} ({b.Email})");
            }
            WaitKey();
        }
        // ЗАДАНИЕ 4: Фильтрация акций по конкретной стране
        private static void ShowPromosByCountry()
        {
            Console.Write("Введите название страны для просмотра акций: ");
            string? country = Console.ReadLine();
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var products = db.Query("SELECT * FROM PromoProducts WHERE Country = @CountryName", new { CountryName = country }).ToList();
                Console.WriteLine($"\n--- Акции, действующие в стране: {country} ({products.Count}) ---");
                foreach (var p in products)
                {
                    Console.WriteLine($"- {p.ProductName} | Активна с {p.StartDate:dd.MM.yyyy} по {p.EndDate:dd.MM.yyyy}");
                }
            }
            WaitKey();
        }
        private static void WaitKey()
        {
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("\nНажмите любую клавишу для возврата в меню...");
            Console.ResetColor();
            Console.ReadKey();
        }
    }
}
