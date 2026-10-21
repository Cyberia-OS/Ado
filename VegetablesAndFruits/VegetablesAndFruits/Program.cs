using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.Common;
using System.Diagnostics;
using System.Threading.Tasks;

namespace VegetablesAndFruits
{
    class Program
    {
        static string currentProvider;
        static string currentConnectionString;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            // старт на sql server
            var sqlServer = ConfigurationManager.ConnectionStrings["SqlServerConn"];
            currentProvider = sqlServer.ProviderName;
            currentConnectionString = sqlServer.ConnectionString;

            Console.WriteLine("овощи и фрукты (ado.net + фабрика провайдеров)");
            Console.WriteLine($"текущая субд: {currentProvider}\n");

            await EnsureSqlServerDatabaseExistsAsync();
            await EnsureTableAsync();

            while (true)
            {
                Console.WriteLine("\n1. добавить продукт");
                Console.WriteLine("2. показать все");
                Console.WriteLine("3. обновить продукт");
                Console.WriteLine("4. удалить продукт");
                Console.WriteLine("5. переключить субд");
                Console.WriteLine("0. выход");
                Console.Write("выбор: ");

                switch (Console.ReadLine())
                {
                    case "1": await AddProductMenuAsync();
                        break;
                    case "2": await ShowAllAsync(); 
                        break;
                    case "3": await UpdateProductMenuAsync(); 
                        break;
                    case "4": await DeleteProductMenuAsync();
                        break;
                    case "5": await SwitchDatabaseAsync();
                        break;
                    case "0": return;
                    default: Console.WriteLine("неверный выбор"); 
                        break;
                }
            }
        }

        // задание 1: фабрика провайдеров
        static DbConnection CreateConnection()
        {
            var factory = DbProviderFactories.GetFactory(currentProvider);
            var conn = factory.CreateConnection();
            conn.ConnectionString = currentConnectionString;
            return conn;
        }

        // задание 6: смена субд
        static async Task SwitchDatabaseAsync()
        {
            var sqlServer = ConfigurationManager.ConnectionStrings["SqlServerConn"];
            var sqlite = ConfigurationManager.ConnectionStrings["SqliteConn"];

            bool toSqlite = currentProvider == sqlServer.ProviderName;
            currentProvider = toSqlite ? sqlite.ProviderName : sqlServer.ProviderName;
            currentConnectionString = toSqlite ? sqlite.ConnectionString : sqlServer.ConnectionString;

            Console.WriteLine($"переключено на {currentProvider}");

            await EnsureSqlServerDatabaseExistsAsync();
            await EnsureTableAsync();
        }

        // создание бд sql server (sqlite файл создаёт сам)
        static async Task EnsureSqlServerDatabaseExistsAsync()
        {
            if (!currentProvider.Contains("SqlClient")) return;

            try
            {
                var builder = new System.Data.SqlClient.SqlConnectionStringBuilder(currentConnectionString);
                var dbName = builder.InitialCatalog;
                if (string.IsNullOrEmpty(dbName)) return;

                builder.InitialCatalog = "master";
                using (var conn = new System.Data.SqlClient.SqlConnection(builder.ConnectionString))
                {
                    await conn.OpenAsync();
                    using (var cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = $"IF DB_ID('{dbName}') IS NULL CREATE DATABASE [{dbName}]";
                        await cmd.ExecuteNonQueryAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"не удалось создать бд: {ex.Message}");
            }
        }

        // структура одинаковая для обеих субд
        static async Task EnsureTableAsync()
        {
            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = GetCreateTableSql();
                    await cmd.ExecuteNonQueryAsync();
                }
            }
        }

        static string GetCreateTableSql()
        {
            return currentProvider.Contains("SqlClient")
                ? @"IF OBJECT_ID('Products', 'U') IS NULL
                    CREATE TABLE Products (
                        Id INT IDENTITY(1,1) PRIMARY KEY,
                        Name NVARCHAR(100) NOT NULL,
                        Category NVARCHAR(50) NOT NULL,
                        Color NVARCHAR(50) NULL,
                        Price DECIMAL(10,2) NOT NULL,
                        Quantity INT NOT NULL
                    )"
                : @"CREATE TABLE IF NOT EXISTS Products (
                        Id INTEGER PRIMARY KEY AUTOINCREMENT,
                        Name TEXT NOT NULL,
                        Category TEXT NOT NULL,
                        Color TEXT NULL,
                        Price REAL NOT NULL,
                        Quantity INTEGER NOT NULL
                    )";
        }

        // задание 3: async чтение
        static async Task ShowAllAsync()
        {
            var sw = Stopwatch.StartNew();

            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT Id, Name, Category, Color, Price, Quantity FROM Products ORDER BY Name";

                    using (var reader = await cmd.ExecuteReaderAsync())
                    {
                        Console.WriteLine("id | название         | категория | цвет      | цена    | кол-во");
                        while (await reader.ReadAsync())
                        {
                            var id = reader.GetInt32(0);
                            var name = reader.IsDBNull(1) ? "" : reader.GetString(1);
                            var cat = reader.IsDBNull(2) ? "" : reader.GetString(2);
                            var color = reader.IsDBNull(3) ? "-" : reader.GetString(3);
                            var price = Convert.ToDecimal(reader.GetValue(4));
                            var qty = Convert.ToInt32(reader.GetValue(5));

                            Console.WriteLine($"{id} | {name,-16} | {cat,-9} | {color,-9} | {price,7:F2} | {qty}");
                        }
                    }
                }
            }

            sw.Stop();
            // задание 7: время запроса
            Console.WriteLine($"\nвыполнено за {sw.Elapsed.TotalSeconds:F4} сек");
        }

        // задание 4 (часть 1): async добавление
        static async Task AddProductAsync(string name, string category, string color, decimal price, int quantity)
        {
            var sw = Stopwatch.StartNew();

            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "INSERT INTO Products (Name, Category, Color, Price, Quantity) VALUES (@n, @c, @col, @p, @q)";
                    AddParam(cmd, "@n", name);
                    AddParam(cmd, "@c", category);
                    AddParam(cmd, "@col", color);
                    AddParam(cmd, "@p", price);
                    AddParam(cmd, "@q", quantity);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    sw.Stop();
                    Console.WriteLine($"добавлено {rows} строк(и) за {sw.Elapsed.TotalSeconds:F4} сек");
                }
            }
        }

        static async Task AddProductMenuAsync()
        {
            Console.Write("название: "); var name = Console.ReadLine();
            Console.Write("категория (овощ/фрукт): "); var cat = Console.ReadLine();
            Console.Write("цвет: "); var color = Console.ReadLine();
            Console.Write("цена: "); if (!decimal.TryParse(Console.ReadLine(), out var price)) { Console.WriteLine("неверная цена"); return; }
            Console.Write("количество: "); if (!int.TryParse(Console.ReadLine(), out var qty)) { Console.WriteLine("неверное количество"); return; }

            await AddProductAsync(name, cat, color, price, qty);
        }

        // задание 4 (часть 2): async обновление
        static async Task UpdateProductAsync(int id, string newName, string newColor, decimal? newPrice, int? newQuantity)
        {
            var sw = Stopwatch.StartNew();

            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    var sets = new List<string>();
                    if (newName != null) { sets.Add("Name = @name"); AddParam(cmd, "@name", newName); }
                    if (newColor != null) { sets.Add("Color = @color"); AddParam(cmd, "@color", newColor); }
                    if (newPrice.HasValue) { sets.Add("Price = @price"); AddParam(cmd, "@price", newPrice.Value); }
                    if (newQuantity.HasValue) { sets.Add("Quantity = @qty"); AddParam(cmd, "@qty", newQuantity.Value); }

                    if (sets.Count == 0) { Console.WriteLine("нечего обновлять"); return; }

                    cmd.CommandText = $"UPDATE Products SET {string.Join(", ", sets)} WHERE Id = @id";
                    AddParam(cmd, "@id", id);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    sw.Stop();
                    Console.WriteLine($"обновлено {rows} строк(и) за {sw.Elapsed.TotalSeconds:F4} сек");
                }
            }
        }

        static async Task UpdateProductMenuAsync()
        {
            Console.Write("id продукта: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("неверный id"); return; }

            Console.Write("новое название (enter - не менять): "); var name = Console.ReadLine();
            Console.Write("новый цвет (enter - не менять): "); var color = Console.ReadLine();
            Console.Write("новая цена (enter - не менять): "); var pStr = Console.ReadLine();
            Console.Write("новое количество (enter - не менять): "); var qStr = Console.ReadLine();

            decimal? price = string.IsNullOrEmpty(pStr) ? (decimal?)null : decimal.Parse(pStr);
            int? qty = string.IsNullOrEmpty(qStr) ? (int?)null : int.Parse(qStr);

            await UpdateProductAsync(id,
                string.IsNullOrEmpty(name) ? null : name,
                string.IsNullOrEmpty(color) ? null : color,
                price, qty);
        }

        // задание 5: async удаление
        static async Task DeleteProductAsync(int id)
        {
            var sw = Stopwatch.StartNew();

            using (var conn = CreateConnection())
            {
                await conn.OpenAsync();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "DELETE FROM Products WHERE Id = @id";
                    AddParam(cmd, "@id", id);

                    int rows = await cmd.ExecuteNonQueryAsync();
                    sw.Stop();
                    Console.WriteLine($"удалено {rows} строк(и) за {sw.Elapsed.TotalSeconds:F4} сек");
                }
            }
        }

        static async Task DeleteProductMenuAsync()
        {
            Console.Write("id продукта: ");
            if (!int.TryParse(Console.ReadLine(), out var id)) { Console.WriteLine("неверный id"); return; }

            Console.Write($"удалить продукт id={id}? (y/n): ");
            if (Console.ReadLine()?.Trim().ToLower() != "y") { Console.WriteLine("отмена"); return; }

            await DeleteProductAsync(id);
        }

        static void AddParam(DbCommand cmd, string name, object value)
        {
            var p = cmd.CreateParameter();
            p.ParameterName = name;
            p.Value = value ?? DBNull.Value;
            cmd.Parameters.Add(p);
        }
    }
}