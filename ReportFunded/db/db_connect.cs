using System;
using MySql.Data.MySqlClient;


namespace ReportFunded.db
{
    class db_connect
    {

        private MySqlConnection conn = null;
        public MySqlConnection Connect()
        {
            string myConnectionString;

            myConnectionString = "server=127.0.0.1;uid=root;" +
                "pwd=;database=encompass;";

            try
            {
                conn = new MySql.Data.MySqlClient.MySqlConnection();
                conn.ConnectionString = myConnectionString;
                conn.Open();
                Console.WriteLine("MySQL version : {0}", conn.ServerVersion);
                
            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }
            return conn;
        }

        public void close()
        {
            if (conn != null)
            {
                conn.Close();
            }
        }

    }
}
