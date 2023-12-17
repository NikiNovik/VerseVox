using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace VerseVox.Queries
{
    public class ListQueries
    {
        MySqlConnection connection = new MySqlConnection("Server=" + Scripts.GlobalVariables.ServerAddress + "; User ID=root;Password=root;Database=versedb");

        public List<string> GetInvites()
        {
            List<string> usernames = new List<string>();
            string query = "SELECT id, username FROM users WHERE id IN ( SELECT sender FROM usersfriends WHERE receiver = @id AND sender NOT IN ( SELECT receiver FROM usersfriends WHERE sender = @id) ) AND id != @id;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@id", Scripts.GlobalVariables.userid);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    usernames.Add(reader.GetString("username"));
                    usernames.Add(reader.GetInt32("id").ToString());
                }
            }
            connection.Close();
            return usernames;
        }

        public List<string> GetFriends()
        {
            List<string> usernames = new List<string>();
            string query = "SELECT id, username, status, activity FROM users WHERE id IN ( SELECT receiver FROM usersfriends WHERE sender = @id AND receiver IN ( SELECT sender FROM usersfriends WHERE receiver = @id ) ) AND id != @id;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@id", Scripts.GlobalVariables.userid);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    usernames.Add(reader.GetString("username"));
                    usernames.Add(reader.GetInt32("id").ToString());
                    usernames.Add(reader.GetInt32("status").ToString());
                    usernames.Add(reader.GetString("activity"));
                }
            }
            connection.Close();
            return usernames;
        }

        public List<string> SearchFriend(string nickname)
        {
            List<string> usernames = new List<string>();
            string query = "SELECT id, username FROM users WHERE username = @nickname AND id NOT IN ( SELECT receiver FROM usersfriends WHERE sender = @id ) AND id != @id;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@id", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@nickname", nickname);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    usernames.Add(reader.GetString("username"));
                    usernames.Add(reader.GetInt32("id").ToString());
                }
            }
            connection.Close();
            return usernames;
        }
        public void applyRequest()
        {
            string query = "INSERT INTO usersfriends (receiver, sender) VALUES (@receiver, @sender);";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@sender", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiver", Scripts.NonStaticVariables.selectedUserId);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
        }
        public void rejectRequest()
        {
            string query = "DELETE FROM usersfriends WHERE receiver = @receiver AND sender = @sender;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@receiver", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@sender", Scripts.NonStaticVariables.selectedUserId);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
        }

        public void deleteRequest()
        {
            string query = "DELETE FROM usersfriends WHERE receiver = @receiver AND sender = @sender OR receiver = @sender AND sender = @receiver;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@receiver", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@sender", Scripts.NonStaticVariables.selectedUserId);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
        }

    }
}
