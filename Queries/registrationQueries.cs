using MySqlConnector;
using System.Security.Cryptography;
using System.Text;

namespace VerseVox.Queries
{
    public class registrationQueries
    {
        MySqlConnection connection = new MySqlConnection("Server=" + Scripts.GlobalVariables.ServerAddress + "; User ID=root;Password=root;Database=versedb");

        public void sendRegistationRequest(string username, string usermail, string password)
        {
            string query = "SELECT id FROM users WHERE username = @username OR usermail = @usermail";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@username", username);
            msc.Parameters.AddWithValue("@usermail", usermail);
            connection.Open();
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                if (!reader.Read())
                {
                    connection.Close();
                    successfulRegistration(username, usermail, password);
                }
                else
                {
                    connection.Close();
                    System.Windows.MessageBox.Show("Пользователь с таким никнеймом или почтой уже существует");
                }
            }
        }

        public void successfulRegistration(string username, string usermail, string password)
        {
            string query = "INSERT INTO users (username, usermail, password) VALUES (@username, @usermail, @password)";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@username", username);
            msc.Parameters.AddWithValue("@password", GetMD5Hash(password));
            msc.Parameters.AddWithValue("@usermail", usermail);
            connection.Open();
            msc.ExecuteNonQuery();
            connection.Close();
            Scripts.NonStaticVariables.successRegister = true;
        }
        public static string GetMD5Hash(string input)
        {
            using (MD5 md5Hash = MD5.Create())
            {
                byte[] data = md5Hash.ComputeHash(Encoding.UTF8.GetBytes(input));

                StringBuilder sBuilder = new StringBuilder();

                for (int i = 0; i < data.Length; i++)
                {
                    sBuilder.Append(data[i].ToString("x2"));
                }

                return sBuilder.ToString();
            }
        }

    }
}
