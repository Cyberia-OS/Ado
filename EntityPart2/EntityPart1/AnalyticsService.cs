using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient; 

namespace MailingListApp.Services
{
    public class AnalyticsService
    {
        private readonly string _connectionString;

        public AnalyticsService(string connectionString)
        {
            _connectionString = connectionString;
        }


        public List<CategoryReportItem> GetCategoriesReport(string functionName, int topN, bool flag)
        {
            var result = new List<CategoryReportItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand($"SELECT category_name, subscribers_count FROM {functionName}(@topN, @flag)", conn))
            {
                cmd.Parameters.AddWithValue("@topN", topN);
                cmd.Parameters.AddWithValue("@flag", flag ? 1 : 0);

                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new CategoryReportItem
                        {
                            Name = reader.GetString(0),
                            Count = Convert.ToInt64(reader.GetValue(1))
                        });
                    }
                }
            }
            return result;
        }


        public void ArchiveExpiredPromotions()
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("ArchiveExpiredPromotions", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }


        public List<AnalyticsViewItem> GetViewReport(string viewName)
        {
            var result = new List<AnalyticsViewItem>();

            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand($"SELECT group_name, avg_age FROM {viewName}", conn))
            {
                conn.Open();
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        result.Add(new AnalyticsViewItem
                        {
                            GroupName = reader.GetString(0),
                            Value = Convert.ToDouble(reader.GetValue(1))
                        });
                    }
                }
            }
            return result;
        }
    }

    public class CategoryReportItem
    {
        public string Name { get; set; }
        public long Count { get; set; }
    }

    public class AnalyticsViewItem
    {
        public string GroupName { get; set; }
        public double Value { get; set; }
    }
}
