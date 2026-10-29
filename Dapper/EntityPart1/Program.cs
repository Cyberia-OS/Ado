using System;
using System.Linq;
using System.Threading.Tasks;
using MailingListApp.Services;

namespace MailingListApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connStr = @"Server=(localdb)\MSSQLLocalDB;Database=MailingList;Trusted_Connection=True;Encrypt=False;";

            // Запуск асинхронного кода в синхронной среде .NET Framework
            Task.Run(async () => await RunAppAsync(connStr)).Wait();

            Console.WriteLine("\nНажмите любую клавишу для выхода...");
            Console.ReadKey();
        }

        private static async Task RunAppAsync(string connectionString)
        {
            var service = new DapperMailingService(connectionString);

            try
            {
                Console.WriteLine(" Аналитика рассылки (Dapper Async) \n");

                // Тест Задания 1: Количество покупателей по городам
                var cityStats = await service.GetSubscribersByCityAsync();
                Console.WriteLine("Покупатели в городах:");
                foreach (var item in cityStats)
                {
                    Console.WriteLine($"- {item.GroupName}: {item.TotalCount} чел.");
                }

                // Тест Задания 1: Среднее количество городов
                double avgCities = await service.GetAvgCitiesPerCountryAsync();
                Console.WriteLine($"\nСреднее кол-во городов по странам: {avgCities:F2}");

                var bestCountry = (await service.GetTopCountriesAsync(1)).FirstOrDefault();
                if (bestCountry != null)
                {
                    Console.WriteLine($"\nЛучшая страна по покупателям: {bestCountry.GroupName} ({bestCountry.TotalCount} подп.)");
                }

                // Тест Задания 2: Категории конкретной страны
                string testCountry = "Испания";
                var categories = await service.GetCategoriesByCountryAsync(testCountry);
                Console.WriteLine($"\nРазделы, интересные покупателям из страны ({testCountry}):");
                foreach (var cat in categories)
                {
                    Console.WriteLine($"- {cat}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка при выполнении Dapper-запроса: {ex.Message}");
            }
        }
    }
}
