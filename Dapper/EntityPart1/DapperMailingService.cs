using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;
using System.Threading.Tasks;
using Dapper;
using MailingListApp.Models;

namespace MailingListApp.Services
{
    public class DapperMailingService
    {
        private readonly string _connectionString;

        public DapperMailingService(string connectionString)
        {
            _connectionString = connectionString;
        }

        private IDbConnection CreateConnection() => new SqlConnection(_connectionString);

        // ЗАДАНИЕ 1: Географическая статистика (Асинхронно)
        public async Task<IEnumerable<SimpleReportItem>> GetSubscribersByCityAsync()
        {
            using (var db = CreateConnection())
                return await db.QueryAsync<SimpleReportItem>("SELECT * FROM View_SubscribersByCity");
        }

        public async Task<IEnumerable<SimpleReportItem>> GetSubscribersByCountryAsync()
        {
            using (var db = CreateConnection())
                return await db.QueryAsync<SimpleReportItem>("SELECT * FROM View_SubscribersByCountry");
        }

        public async Task<IEnumerable<SimpleReportItem>> GetCitiesByCountryAsync()
        {
            using (var db = CreateConnection())
                return await db.QueryAsync<SimpleReportItem>("SELECT * FROM View_CitiesByCountry");
        }

        public async Task<double> GetAvgCitiesPerCountryAsync()
        {
            using (var db = CreateConnection())
                return await db.QueryFirstOrDefaultAsync<double>("SELECT * FROM View_AvgCitiesPerCountry");
        }

        // ЗАДАНИЕ 2: Фильтры и выборки 

        public async Task<IEnumerable<string>> GetCategoriesByCountryAsync(string country)
        {
            using (var db = CreateConnection())
                return await db.QueryAsync<string>(
                    "SELECT * FROM dbo.GetCategoriesByCountrySubscribers(@Country)",
                    new { Country = country });
        }

        public async Task<IEnumerable<PromotionalGood>> GetPromoByIntervalAsync(string categoryName, DateTime start, DateTime end)
        {
            string sql = @"SELECT p.* FROM PromotionalGoods p 
                           JOIN Categories c ON p.category_id = c.id 
                           WHERE c.name = @CategoryName AND p.end_date BETWEEN @Start AND @End";
            using (var db = CreateConnection())
                return await db.QueryAsync<PromotionalGood>(sql, new { CategoryName = categoryName, Start = start, End = end });
        }

        public async Task<IEnumerable<PromotionalGood>> GetPromoForSubscriberAsync(int subscriberId)
        {
            string sql = @"SELECT p.* FROM PromotionalGoods p
                           JOIN Subscriptions sub ON p.category_id = sub.category_id
                           WHERE sub.subscriber_id = @SubId";
            using (var db = CreateConnection())
                return await db.QueryAsync<PromotionalGood>(sql, new { SubId = subscriberId });
        }

        // ЗАДАНИЕ 3: Топы и рейтинги 
        public async Task<IEnumerable<SimpleReportItem>> GetTopCountriesAsync(int topN)
        {
            using (var db = CreateConnection())
                return await db.QueryAsync<SimpleReportItem>(
                    $"SELECT TOP (@Top) country AS GroupName, COUNT(*) AS TotalCount FROM Subscribers GROUP BY country ORDER BY TotalCount DESC",
                    new { Top = topN });
        }

        public async Task<IEnumerable<SimpleReportItem>> GetTopCitiesAsync(int topN)
        {
            using (var db = CreateConnection())
                return await db.QueryAsync<SimpleReportItem>(
                    $"SELECT TOP (@Top) city AS GroupName, COUNT(*) AS TotalCount FROM Subscribers GROUP BY city ORDER BY TotalCount DESC",
                    new { Top = topN });
        }
    }
}
