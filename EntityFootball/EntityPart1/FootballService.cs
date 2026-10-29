using System;
using System.Collections.Generic;
using System.Data;
using Microsoft.Data.SqlClient;

namespace FootballApp.Services
{
    public class FootballService
    {
        private readonly string _connectionString;

        public FootballService(string connectionString)
        {
            _connectionString = connectionString;
        }

        // Задание 2: Получение разницы голов
        public DataTable GetGoalDifferenceReport()
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("SELECT * FROM View_TeamGoalDifference ORDER BY GoalDifference DESC", conn))
            {
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
            }
            return dt;
        }

        // Задание 2: Полная информация о матчах (все, по дате или по команде)
        public DataTable GetMatches(string filterQuery = "SELECT * FROM View_FullMatchDetails", SqlParameter param = null)
        {
            DataTable dt = new DataTable();
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(filterQuery, conn))
            {
                if (param != null) cmd.Parameters.Add(param);
                using (SqlDataAdapter da = new SqlDataAdapter(cmd)) { da.Fill(dt); }
            }
            return dt;
        }

        // Задание 3: Добавление матча с проверкой
        public void AddMatch(string team1, string team2, int goals1, int goals2, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("AddMatchWithCheck", conn))
            {
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Parameters.AddWithValue("@Team1Name", team1);
                cmd.Parameters.AddWithValue("@Team2Name", team2);
                cmd.Parameters.AddWithValue("@Goals1", goals1);
                cmd.Parameters.AddWithValue("@Goals2", goals2);
                cmd.Parameters.AddWithValue("@MatchDate", date.Date);

                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }

        // Задание 3: Удаление матча (Возвращает ID матча, если он найден)
        public int GetMatchIdByTeamsAndDate(string team1, string team2, DateTime date)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand(
                @"SELECT MatchID FROM Matches 
                  WHERE Team1ID = (SELECT TeamID FROM Teams WHERE TeamName = @t1) 
                    AND Team2ID = (SELECT TeamID FROM Teams WHERE TeamName = @t2) 
                    AND MatchDate = @date", conn))
            {
                cmd.Parameters.AddWithValue("@t1", team1);
                cmd.Parameters.AddWithValue("@t2", team2);
                cmd.Parameters.AddWithValue("@date", date.Date);

                conn.Open();
                object res = cmd.ExecuteScalar();
                return res != null ? Convert.ToInt32(res) : -1;
            }
        }

        public void DeleteMatch(int matchId)
        {
            using (SqlConnection conn = new SqlConnection(_connectionString))
            using (SqlCommand cmd = new SqlCommand("DELETE FROM Matches WHERE MatchID = @id", conn))
            {
                cmd.Parameters.AddWithValue("@id", matchId);
                conn.Open();
                cmd.ExecuteNonQuery();
            }
        }
    }
}
