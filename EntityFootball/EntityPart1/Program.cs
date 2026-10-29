using System;
using System.Data;
using FootballApp.Services;

namespace FootballApp
{
    internal class Program
    {
        static void Main(string[] args)
        {      
            string connStr = @"Server=(localdb)\MSSQLLocalDB;Database=SpainFootballDB;Trusted_Connection=True;Encrypt=False;";


            FootballService service = new FootballService(connStr);

            try
            {
                Console.WriteLine("Управление Чемпионатом Испании");

                //Задания 2: Показ разницы голов
                Console.WriteLine("\n[Отчет]: Разница забитых/пропущенных голов:");
                DataTable goalsTable = service.GetGoalDifferenceReport();
                foreach (DataRow row in goalsTable.Rows)
                {
                    Console.WriteLine($"- Команда: {row["TeamName"],-15} | Забито: {row["TotalScored"]} | Пропущено: {row["TotalConceded"]} | Разница: {row["GoalDifference"]}");
                }

                // Задания 3: Удаление матча с подтверждением пользователя
                Console.WriteLine("\nУдаление матча ");
                Console.Write("Введите название Команды 1 (Хозяева): ");
                string t1 = Console.ReadLine();
                Console.Write("Введите название Команды 2 (Гости): ");
                string t2 = Console.ReadLine();
                DateTime matchDate = DateTime.Now.Date; // Или запросите ввод через DateTime.Parse

                int matchId = service.GetMatchIdByTeamsAndDate(t1, t2, matchDate);

                if (matchId == -1)
                {
                    Console.WriteLine("Матч с такими параметрами не найден в СУБД.");
                }
                else
                {
                    // Задание 3: Обязательный интерактивный вопрос пользователю
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.Write($"Вы уверены, что хотите удалить матч {t1} - {t2} от {matchDate:dd.MM.yyyy}? (y/n): ");
                    Console.ResetColor();

                    string answer = Console.ReadLine()?.ToLower();
                    if (answer == "y" || answer == "yes" || answer == "да")
                    {
                        service.DeleteMatch(matchId);
                        Console.WriteLine("Матч успешно удален.");
                    }
                    else
                    {
                        Console.WriteLine("Удаление отменено пользователем.");
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка выполнения операции: {ex.Message}");
            }

            Console.WriteLine("\nНажмите любую клавишу для завершения...");
            Console.ReadKey();
        }
    }
}
