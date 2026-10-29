using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using Dapper;
using Microsoft.Data.SqlClient;

namespace DapperExample
{
    public class User
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public int Age { get; set; }
    }

    class Program
    {
        static string connectionString = "Server=localhost;Database=TestDB;Integrated Security=True;TrustServerCertificate=True;";

        static void Main(string[] args)
        {
            Console.WriteLine("Старт CRUD операций с Dapper\n");

            // 1. ВСТАВКА 
            var newUser = new User { Name = "Иван Петров", Email = "ivan@example.com", Age = 30 };
            string insertSql = "INSERT INTO Users (Name, Email, Age) VALUES (@Name, @Email, @Age)";

            using (IDbConnection db = new SqlConnection(connectionString))
            {
                int rowsAffected = db.Execute(insertSql, newUser);
                Console.WriteLine($"[CREATE] Вставлено строк: {rowsAffected}");
            }

            // 2. ЧТЕНИЕ 
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var users = db.Query<User>("SELECT * FROM Users WHERE Age > @Age", new { Age = 25 }).ToList();
                Console.WriteLine($"\n[READ] Пользователи старше 25 лет (Найдено: {users.Count}):");
                foreach (var user in users)
                {
                    Console.WriteLine($"ID: {user.Id}, Имя: {user.Name}, Email: {user.Email}, Возраст: {user.Age}");
                }
            }

            // 3. ОБНОВЛЕНИЕ
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var updateSql = "UPDATE Users SET Age = @Age WHERE Name = @Name";
                int rowsUpdated = db.Execute(updateSql, new { Age = 31, Name = "Иван Петров" });
                Console.WriteLine($"\n[UPDATE] Обновлено строк: {rowsUpdated} (Возраст Ивана изменен на 31)");
            }

            // 4. УДАЛЕНИЕ 
            using (IDbConnection db = new SqlConnection(connectionString))
            {
                var deleteSql = "DELETE FROM Users WHERE Name = @Name";
                int rowsDeleted = db.Execute(deleteSql, new { Name = "Иван Петров" });
                Console.WriteLine($"\n[DELETE] Удалено строк: {rowsDeleted} (Иван Петров удален из БД)");
            }

            Console.WriteLine("\nРабота программы завершена");
            Console.ReadLine();
        }
    }
}
