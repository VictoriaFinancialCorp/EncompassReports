using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportFunded.db
{
    class EncompassUpdate
    {
        private Session session;
        private DateTime timestamp;

        public String run()
        {
            this.timestamp = DateTime.Now;

            //connect
            this.session = Utility.ConnectToServer();

            startApplication();

            Console.Out.WriteLine("db update finished!");
            session.End();
            return "";
        }
        private void startApplication()
        {
            Console.Out.WriteLine("Program running...");

            StringFieldCriterion folderCri = new StringFieldCriterion();
            folderCri.FieldName = "Loan.LoanFolder";
            folderCri.Value = "My Pipeline";
            folderCri.MatchType = StringFieldMatchType.Exact;

            QueryCriterion fullQuery = folderCri;

            StringList fields = new StringList();
            fields.Add("Fields.VEND.X263");
            fields.Add("Fields.352");
            fields.Add("Fields.364");
            fields.Add("Fields.37");
            fields.Add("Fields.4000");
            fields.Add("Fields.1109");
            fields.Add("Fields.362");
            fields.Add("Fields.317");

            LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery);
            //LoanIdentityList ids = session.Loans.Query(fullQuery);

            int total = results.Count;
            int count = 0;
            Console.Out.WriteLine("Total Files: " + total);


            //query list for mySql
            List<MySqlCommand> queries = new List<MySqlCommand>();
            //String timestamp = 
            db_connect connection = new db_connect();
            connection.connect();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection.getConnection();


            foreach (LoanReportData data in results)
            {
                Dictionary<String, String> map = new Dictionary<String, String>();

                map.Add("investor", data["Fields.VEND.X263"].ToString().ToUpper());
                map.Add("investorNum", data["Fields.352"].ToString().ToUpper());
                map.Add("timestamp", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                map.Add("b1_lname", data["Fields.37"].ToString().ToUpper());
                map.Add("b1_fname", data["Fields.4000"].ToString().ToUpper());
                map.Add("loanAmt", Convert.ToInt32(data["Fields.1109"]).ToString("C"));
                map.Add("loanNum", data["Fields.364"].ToString());
              
                data["Fields.362"].ToString();
                data["Fields.317"].ToString();
                

                cmd.CommandText = string.Format(
                    "INSERT INTO loan(guid, investor, investorNum, createdAt, b1_lname, b1_fname, loanAmt, loanNum) " + 
                    "values(@v1, @v2, @v3, @createdAt, @v4, @v5, @v6, @v7) " +
                    "ON DUPLICATE KEY UPDATE guid=@v1, investor=@v2, investorNum=@v3, updatedAt=@updatedAt, b1_lname=@v4, b1_fname=@v5, loanAmt=@v6, loanNum=@v7"
                    );

                cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@v1", data.Guid);
                cmd.Parameters.AddWithValue("@v2", map["investor"]);
                cmd.Parameters.AddWithValue("@v3", map["investorNum"]);
                cmd.Parameters.AddWithValue("@createdAt", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@updatedAt", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@v4", map["b1_lname"]);
                cmd.Parameters.AddWithValue("@v5", map["b1_fname"]);
                cmd.Parameters.AddWithValue("@v6", map["loanAmt"]);
                cmd.Parameters.AddWithValue("@v7", map["loanNum"]);
                cmd.Prepare();

                cmd.ExecuteNonQuery();

                //status output
                count++;
                if(count % 10 == 0)
                {
                    Console.Write("\r{0}   ", "writing: " + count + "/" + total);
                }

            }
            Console.Out.WriteLine("");
            results.Close();


            connection.addLog("db update", "updated: " + total + " rows in " + DateTime.Now.Subtract(timestamp).ToString(@"s\.fff") + " seconds");
            connection.close();

            Console.Out.WriteLine("Finished updating db");



        }
    }
}
