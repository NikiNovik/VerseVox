using MySqlConnector;
using System;
using System.Collections.Generic;

namespace VerseVox.Queries
{
    public class ProfileQueries
    {
        MySqlConnection connection = new MySqlConnection("Server=" + Scripts.GlobalVariables.ServerAddress + "; User ID=root;Password=root;Database=versedb");
        public void updateStatus()
        {
            string query = "UPDATE users SET status = @status, last_update = NOW(), activity = @activity WHERE id = @id;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@id", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@status", Scripts.GlobalVariables.status);
            msc.Parameters.AddWithValue("@activity", Scripts.GlobalVariables.activity);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
        }

        public List<string> GetMessageNotify()
        {
            List<string> usermes = new List<string>();
            string query = "SELECT MAX(m.time) as last_message_time , u.id, u.username , m.message FROM usersmessages m JOIN users u ON m.senderid = u.id WHERE m.receiverid = @userid GROUP BY u.username, m.message ORDER BY last_message_time DESC;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@userid", Scripts.GlobalVariables.userid);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    usermes.Add(reader.GetInt32("id").ToString());
                    usermes.Add(reader.GetString("username"));
                    usermes.Add(reader.GetString("message"));
                }
            }
            connection.Close();
            return usermes;

        }
    }
}
