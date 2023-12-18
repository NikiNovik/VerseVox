using MySqlConnector;
using System.Collections.Generic;

namespace VerseVox.Queries
{
    public class ChatQueries
    {
        MySqlConnection connection = new MySqlConnection("Server=" + Scripts.GlobalVariables.ServerAddress + "; User ID=root;Password=root;Database=versedb");

        public List<string> GetMessageHistory()
        {
            List<string> messages = new List<string>();
            string query = "SELECT u.username, m.message FROM usersmessages m JOIN users u ON m.senderid = u.id WHERE (m.senderid = @userid AND m.receiverid = @receiverid) OR (m.senderid = @receiverid AND m.receiverid = @userid) ORDER BY m.time ASC;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@userid", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiverid", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    messages.Add(reader.GetString("username"));
                    messages.Add(reader.GetString("message"));
                }
            }
            connection.Close();
            return messages;
        }
        //SELECT username, STATUS, activity FROM users WHERE id = 20
        public List<string> GetChatStatus()
        {
            List<string> messages = new List<string>();
            string query = "SELECT username, status, activity FROM users WHERE id = @chatid;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@chatid", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    messages.Add(reader.GetString("username"));
                    messages.Add(reader.GetInt32("status").ToString());
                    messages.Add(reader.GetString("activity"));
                }
            }
            connection.Close();
            return messages;
        }

        public List<string> CheckFriendStatus()
        {
            List<string> messages = new List<string>();
            string query = "SELECT frid FROM usersfriends m JOIN users u ON m.sender = u.id WHERE (m.sender = @sender AND m.receiver = @receiver) OR (m.sender = @receiver AND m.receiver = @sender)";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@sender", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiver", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    messages.Add(reader.GetInt32("frid").ToString());
                }
            }
            connection.Close();
            return messages;
        }

        public void SendMessageToUser(string Message)
        {
            string query = "INSERT INTO usersmessages (senderid, receiverid, message) VALUES (@sender, @receiver, @message);";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@sender", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiver", Scripts.NonStaticVariables.ChatID);
            msc.Parameters.AddWithValue("@message", Message);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
        }
        public int GetCallStatus()
        {
            string query = "SELECT acceptedcall FROM userscalls JOIN users ON userscalls.receiverid = users.id " +
                "WHERE senderid = @senderid AND receiverid = @receiverid OR senderid = @receiverid AND receiverid = @senderid";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@senderid", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiverid", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetInt32("acceptedcall") == 1)
                    {
                        return 2;
                    }
                    else
                    {
                        return 1;
                    }
                }
                if (!reader.HasRows)
                {
                    return 0;
                }
            }
            return 0;
        }

        public void GetCallRequest()
        {
            string query = "UPDATE userscalls SET acceptedcall = 1 WHERE senderid = @receiverid AND receiverid = @senderid";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@senderid", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiverid", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
        }

        public bool checkWhoSender()
        {
            string query = "SELECT acceptedcall, senderid, receiverid FROM userscalls WHERE senderid = @id OR receiverid = @id";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@id", Scripts.GlobalVariables.userid);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    if (reader.GetInt32("senderid") == Scripts.GlobalVariables.userid)
                    {
                        return true;
                    }
                    if (reader.GetInt32("receiverid") == Scripts.GlobalVariables.userid)
                    {
                        return false;
                    }

                }
                if (!reader.HasRows)
                {
                    return false;
                }
            }
            connection.Close();
            return false;

        }

        public int getChannelID()
        {
            string query = "SELECT callid FROM userscalls WHERE senderid = @senderid AND receiverid = @receiverid OR senderid = @receiverid AND receiverid = @senderid";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@senderid", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiverid", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                while (reader.Read())
                {
                    return reader.GetInt32("callid");
                }
            }
            return 1;
        }

        public void SendCallRequest()
        {
            string query = "SELECT acceptedcall FROM userscalls JOIN users ON userscalls.receiverid = users.id WHERE receiverid = @receiverid ";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@receiverid", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            {
                using (MySqlDataReader reader = msc.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        if (reader.GetInt32("acceptedcall") == 1)
                        {
                            System.Windows.MessageBox.Show("Этот пользователь уже разговаривает");
                        }

                    }
                    if (!reader.HasRows)
                    {
                        connection.Close();
                        SendMessageToUser("*Пытается вам позвонить*");
                        query = "INSERT INTO userscalls (senderid, receiverid) VALUES (@senderid, @receiverid);";
                        msc = new MySqlCommand(query, connection);
                        msc.Parameters.AddWithValue("@senderid", Scripts.GlobalVariables.userid);
                        msc.Parameters.AddWithValue("@receiverid", Scripts.NonStaticVariables.ChatID);
                        connection.Open();
                        msc.ExecuteNonQuery();
                        connection.Close();
                    }
                }
            }
            connection.Close();
        }

        public void EndCallRequest()
        {
            string query = "DELETE FROM userscalls WHERE senderid = @senderid AND senderid = @senderid AND receiverid = @receiverid OR senderid = @receiverid AND receiverid = @senderid";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@senderid", Scripts.GlobalVariables.userid);
            msc.Parameters.AddWithValue("@receiverid", Scripts.NonStaticVariables.ChatID);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
            SendMessageToUser("*Завершает звонок*");
        }

    }
}
