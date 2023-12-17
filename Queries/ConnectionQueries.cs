using MySqlConnector;
using System;

namespace VerseVox.Queries
{
    public class ConnectionQueries
    {
        MySqlConnection connection = new MySqlConnection("Server=" + Scripts.GlobalVariables.ServerAddress + "; User ID=root;Password=root;Database=versedb");
        public void CheckConnectionState()
        {
            Scripts.NonStaticVariables.windowId = 0;
            try
            {
                connection.Open();
                if (connection.Ping())
                {
                    Scripts.NonStaticVariables.windowId = 1;
                }
                else
                {
                    Scripts.NonStaticVariables.windowId = 404;
                }
            }
            catch (Exception ex)
            {
                Scripts.NonStaticVariables.windowId = 404;
                Console.WriteLine(ex.Message);
            }
            finally
            {
                connection.Close();
            }
        }
    }
}
