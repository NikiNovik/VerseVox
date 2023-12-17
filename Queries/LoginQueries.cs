using MySqlConnector;
using System.Security.Cryptography;
using System.Text;

namespace VerseVox.Queries
{
    //            (this.Parent as DockPanel).Children.Remove(this);
    public class LoginQueries
    {
        MySqlConnection connection = new MySqlConnection("Server=" + Scripts.GlobalVariables.ServerAddress + "; User ID=root;Password=root;Database=versedb");
        public void CheckLogin(string login, string password)
        {
            connection.Open();
            string query = "SELECT * FROM users WHERE usermail = @login AND password = @password;";
            MySqlCommand msc = new MySqlCommand(query, connection);
            msc.Parameters.AddWithValue("@login", login);
            msc.Parameters.AddWithValue("@password", GetMD5Hash(password));
            using (MySqlDataReader reader = msc.ExecuteReader())
            {
                if (reader.Read())
                {
                    Scripts.NonStaticVariables.successLogin = true;
                    Scripts.GlobalVariables.userid = reader.GetInt32("id");
                    Scripts.GlobalVariables.username = reader.GetString("username");
                    Scripts.GlobalVariables.usermail = reader.GetString("usermail");
                    Scripts.GlobalVariables.status = reader.GetInt32("status");
                    Scripts.GlobalVariables.password = reader.GetString("password");
                    Scripts.NonStaticVariables.windowId = 50;
                }
                else
                {
                    System.Windows.MessageBox.Show("Неправильные данные");
                }
            }

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
