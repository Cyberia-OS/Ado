using System;
using System.Collections.Generic;
using MailingListApp.Services;

namespace MailingListApp
{
    internal class Program
    {
        static void Main(string[] args)
        {
            string connectionString = "Server=.;Database=MailingList;Trusted_Connection=True;Encrypt=False;";

            // Обратите внимание на знак @ перед строкой, он обязателен, если в имени есть символ \


            try
            {
                Console.WriteLine("=== Приложение 'Список рассылки' запущенно ===\n");
                AnalyticsService analytics = new AnalyticsService(connectionString);

                Console.WriteLine("Шаг 1: Запуск процедуры архивации устаревших акций...");
                analytics.ArchiveExpiredPromotions();
                Console.WriteLine("Архивация успешно выполнена в базе данных.\n");

                Console.WriteLine("Шаг 2: Получение ТОП-3 популярных разделов...");
                List<CategoryReportItem> topCategories = analytics.GetCategoriesReport("GetCategoriesByPopularity", 3, true);

                if (topCategories.Count == 0)
                {
                    Console.WriteLine("Данные не найдены (проверьте наличие записей в таблицах СУБД).");
                }
                else
                {
                    foreach (var item in topCategories)
                    {
                        Console.WriteLine($"- Раздел: {item.Name} | Подписчиков: {item.Count}");
                    }
                }

                Console.WriteLine("\nШаг 3: Получение среднего возраста покупателей по городам...");
                List<AnalyticsViewItem> cityAgeReport = analytics.GetViewReport("View_AvgAgeByCity");
                foreach (var item in cityAgeReport)
                {
                    Console.WriteLine($"- Город: {item.GroupName} | Средний возраст: {item.Value:F1} лет");
                }

            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"\n[Ошибка базы данных]: {ex.Message}");
                Console.ResetColor();
            }

            Console.WriteLine("\nРабота завершена. Нажмите любую клавишу для закрытия программы...");
            Console.ReadKey();
        }
    }
}
