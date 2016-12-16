using System;
using MySql.Data.MySqlClient;


namespace ReportFunded.db
{
    class db_connect
    {

        private MySqlConnection conn = null;
        public void connect()
        {
            string myConnectionString;

            myConnectionString = "server=" + System.Configuration.ConfigurationManager.AppSettings["db_address"].ToString() + ";" +
                "uid=" + System.Configuration.ConfigurationManager.AppSettings["db_login"].ToString() + ";" +
                "pwd=" + System.Configuration.ConfigurationManager.AppSettings["db_pw"].ToString() + ";" +
                "database=encompass;";

            try
            {
                this.conn = new MySql.Data.MySqlClient.MySqlConnection();
                this.conn.ConnectionString = myConnectionString;
                this.conn.Open();
                Console.WriteLine("MySQL version : {0}", conn.ServerVersion);

            }
            catch (MySql.Data.MySqlClient.MySqlException ex)
            {
                Console.Out.WriteLine(ex.Message);
            }

        }
    
        public void query(String query)
        {

            MySqlDataReader rdr = null;
            string stm = "SELECT * FROM logs";
            MySqlCommand cmd = new MySqlCommand(stm, conn);
            rdr = cmd.ExecuteReader();

            while (rdr.Read())
            {
                Console.WriteLine( rdr.GetString(1));
            }
        }

        public void addLog(String eventName, String message)
        {
            MySqlDataReader reader = null;
            string query = "INSERT INTO logs(timestamp, event, message)"+
                "VALUES(now(),'"+eventName+"','"+message+"')";
            Console.Out.WriteLine("MySQL Query: " + query);
            MySqlCommand cmd = new MySqlCommand(query, conn);
            reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                Console.WriteLine(reader.GetDateTime(0) +"/t" +reader.GetString(1) + "/t" +  reader.GetString(2));

            }
        }

        public void close()
        {
            if (conn != null)
            {
                this.conn.Close();
            }
        }

    }
}
