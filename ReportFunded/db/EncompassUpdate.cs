﻿using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Text;
using log4net;

namespace ReportFunded.db
{
    class EncompassUpdate
    {
        private DateTime timestamp;
        private int reportNum;
        //private Dictionary<string, int> processors;
        private static readonly ILog log = LogManager.GetLogger(typeof(EncompassUpdate));


        public String run(int reportNum)
        {
            this.timestamp = DateTime.Now;
            this.reportNum = reportNum;
            //this.processors = getProcessors();

            startApplication();

            log.Debug("Program finished!");
            return "";
        }

        private Dictionary<string, int> getProcessors()
        {
            Dictionary<string, int> processorList = new Dictionary<string, int>();
            //query list for mySql
            List<MySqlCommand> queries = new List<MySqlCommand>();
            //String timestamp = 
            db_connect connection = new db_connect();
            connection.connect();
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection.getConnection();

            cmd.CommandText = string.Format("SELECT id, name FROM e_processors");

            MySqlDataReader reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                processorList.Add(reader.GetString(1), reader.GetInt32(0));
            }
            reader.Close();

            return processorList;
        }

        private StringFieldCriterion selectLoanFolder()
        {
            StringFieldCriterion folderCri = new StringFieldCriterion();
            folderCri.FieldName = "Loan.LoanFolder";
            folderCri.MatchType = StringFieldMatchType.Exact;
            switch (reportNum)
            {
                case 8:
                    folderCri.Value = "My Pipeline";
                    break;
                case 9:
                    folderCri.Value = "Completed Loans";
                    break;
                case 10:
                    folderCri.Value = "Serviced Loans";
                    break;
                case 11:
                    folderCri.Value = "Adverse Loans";
                    break;
                case 12:
                    folderCri.Value = "Prospective Clients";
                    break;
                default:
                    break;
            }
            log.Info("updating '" + folderCri.Value + "' folder");
            return folderCri;
        }

        private LoanReportCursor encompassQueryBuilder(QueryCriterion folderCri)
        {
            StringList fields = new StringList();
            fields.Add("Fields.VEND.X263");
            fields.Add("Fields.352");
            fields.Add("Fields.364");
            fields.Add("Fields.37");
            fields.Add("Fields.4000");
            fields.Add("Fields.1109");
            fields.Add("Fields.362");
            fields.Add("Fields.317");
            fields.Add("Fields.11");//prop address
            fields.Add("Fields.12");//prop city
            fields.Add("Fields.15");//prop zip
            fields.Add("Fields.362"); //processor
            fields.Add("Fields.317"); //loan officer
            fields.Add("Fields.Log.MS.CurrentMilestone");
            fields.Add("Fields.Log.MS.Date.Started");
            fields.Add("Fields.Log.MS.Date.Submittal");
            fields.Add("Fields.Log.MS.Date.Clear to Close");
            fields.Add("Fields.Log.MS.Date.Docs Drawn");
            fields.Add("Fields.Log.MS.Date.Docs Signing");
            fields.Add("Fields.Log.MS.Date.Funding");
            fields.Add("Fields.Log.MS.Date.Purchased");
            fields.Add("Fields.Log.MS.Date.Completion");
            fields.Add("Fields.Log.MS.Comments.Approval");
            fields.Add("Fields.Log.MS.Comments.Clear to Close");
            fields.Add("Fields.Log.MS.Comments.Completion");
            fields.Add("Fields.Log.MS.Comments.Cond Approval");
            fields.Add("Fields.Log.MS.Comments.Docs Drawn");
            fields.Add("Fields.Log.MS.Comments.Docs Signing");
            fields.Add("Fields.Log.MS.Comments.Funding");
            fields.Add("Fields.Log.MS.Comments.Processing");
            fields.Add("Fields.Log.MS.Comments.Purchased");
            fields.Add("Fields.Log.MS.Comments.Ready for Docs");
            fields.Add("Fields.Log.MS.Comments.Resubmittal");
            fields.Add("Fields.Log.MS.Comments.Shipping");
            fields.Add("Fields.Log.MS.Comments.Started");
            fields.Add("Fields.Log.MS.Comments.Submittal");
            fields.Add("Fields.19"); //loan purpose
            fields.Add("Fields.4"); //loan term
            fields.Add("Fields.1811"); //occupancy
            fields.Add("Fields.3"); //interest rate
            fields.Add("Fields.1393");//current status

            fields.Add("Fields.761"); //locked date
            fields.Add("Fields.2149"); //victoria lock date
            fields.Add("Fields.2220"); //investor lock date
            fields.Add("Fields.2222"); //investor lock exp date
            fields.Add("Fields.2287");//investor lock type

            fields.Add("Fields.2232");//base YSP
            fields.Add("Fields.2273");//total adjustments
            fields.Add("Fields.2277");//net ysp
            fields.Add("Fields.2276");//net srp

            fields.Add("Fields.SERVICE.X8");//servicing status
            fields.Add("Fields.682");//first payment date
            fields.Add("Fields.3514");//1st payment to investor
            fields.Add("Fields.SERVICE.X9");//last statement printed date
                                            // fields.Add("Fields.SERVICE.X10");//Interm Service Printed
            fields.Add("Fields.SERVICE.X39");//number of payments


            LoanReportCursor results = Program.mySession.getSession().Reports.OpenReportCursor(fields, folderCri);
            return results;
            //LoanIdentityList ids = session.Loans.Query(fullQuery);
        }

        private Dictionary<String, String> dbQueryBuilder(LoanReportData data) {
            Dictionary<String, String> map = new Dictionary<String, String>();

            map.Add("investor", data["Fields.VEND.X263"].ToString().ToUpper());
            map.Add("investorNum", data["Fields.352"].ToString().ToUpper());
            //map.Add("timestamp", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
            map.Add("b1_lname", data["Fields.37"].ToString().ToUpper());
            map.Add("b1_fname", data["Fields.4000"].ToString().ToUpper());
            map.Add("loanAmt", Convert.ToInt32(data["Fields.1109"]).ToString("C"));
            map.Add("loanNum", data["Fields.364"].ToString());
            map.Add("currentMilestone", data["Fields.Log.MS.CurrentMilestone"].ToString());
            map.Add("currentStatus", data["Fields.1393"].ToString());


            String startedDate = (data["Fields.Log.MS.Date.Started"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Started"]).ToString("yyyy-MM-dd");
            map.Add("startedDate", startedDate);

            String submittalDate = (data["Fields.Log.MS.Date.Submittal"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Submittal"]).ToString("yyyy-MM-dd");
            map.Add("submittalDate", submittalDate);

            String CTCDate = (data["Fields.Log.MS.Date.Clear to Close"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Clear to Close"]).ToString("yyyy-MM-dd");
            map.Add("CTCDate", CTCDate);

            String docsSignedDate = (data["Fields.Log.MS.Date.Docs Signing"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Docs Signing"]).ToString("yyyy-MM-dd");
            map.Add("docsSignedDate", docsSignedDate);

            String docsDrawnDate = (data["Fields.Log.MS.Date.Docs Drawn"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Docs Drawn"]).ToString("yyyy-MM-dd");
            map.Add("docsDrawnDate", docsDrawnDate);

            String fundedDate = (data["Fields.Log.MS.Date.Funding"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Funding"]).ToString("yyyy-MM-dd");
            map.Add("fundedDate", fundedDate);

            String purchasedDate = (data["Fields.Log.MS.Date.Purchased"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Purchased"]).ToString("yyyy-MM-dd");
            map.Add("purchasedDate", purchasedDate);

            String completionDate = (data["Fields.Log.MS.Date.Completion"] == null) ? null : Convert.ToDateTime(data["Fields.Log.MS.Date.Completion"]).ToString("yyyy-MM-dd");
            map.Add("completionDate", completionDate);

            map.Add("startedComments", data["Fields.Log.MS.Comments.Started"].ToString());
            map.Add("processingComments", data["Fields.Log.MS.Comments.Processing"].ToString());
            map.Add("submittalComments", data["Fields.Log.MS.Comments.Submittal"].ToString());
            map.Add("conditionalComments", data["Fields.Log.MS.Comments.Cond Approval"].ToString());
            map.Add("resubmittedComments", data["Fields.Log.MS.Comments.Resubmittal"].ToString());
            map.Add("approvalComments", data["Fields.Log.MS.Comments.Approval"].ToString());
            map.Add("CTCComments", data["Fields.Log.MS.Comments.Clear to Close"].ToString());
            map.Add("readyForDocComments", data["Fields.Log.MS.Comments.Ready for Docs"].ToString());
            map.Add("docsDrawnComments", data["Fields.Log.MS.Comments.Docs Drawn"].ToString());
            map.Add("docSignedComments", data["Fields.Log.MS.Comments.Docs Signing"].ToString());
            map.Add("fundedComments", data["Fields.Log.MS.Comments.Funding"].ToString());
            map.Add("shippedComments", data["Fields.Log.MS.Comments.Shipping"].ToString());
            map.Add("purchasedComments", data["Fields.Log.MS.Comments.Purchased"].ToString());
            map.Add("completionComments", data["Fields.Log.MS.Comments.Completion"].ToString());


            /*if(data.Guid == "{5913cacc-9304-4b06-82d8-922ec4392796}")
            {
                Console.WriteLine(data["Fields.Log.MS.Comments.Processing"]);
            }*/


            map.Add("int_rate", data["Fields.3"].ToString());
            map.Add("loan_purpose", data["Fields.19"].ToString());
            map.Add("loan_term", data["Fields.4"].ToString());
            map.Add("occupancy", data["Fields.1811"].ToString());

            map.Add("address", data["Fields.11"].ToString());
            map.Add("city", data["Fields.12"].ToString());
            map.Add("zip", data["Fields.15"].ToString());

            String lockedDate = (data["Fields.761"] == null) ? null : Convert.ToDateTime(data["Fields.761"]).ToString("yyyy-MM-dd");
            map.Add("lockedDate", lockedDate);

            String victoriaLockDate = (data["Fields.2149"] == null) ? null : Convert.ToDateTime(data["Fields.2149"]).ToString("yyyy-MM-dd");
            map.Add("victoriaLockDate", victoriaLockDate);

            String investorLockDate = (data["Fields.2220"] == null) ? null : Convert.ToDateTime(data["Fields.2220"]).ToString("yyyy-MM-dd");
            map.Add("investorLockDate", investorLockDate);

            String investorLockExpDate = (data["Fields.2222"] == null) ? null : Convert.ToDateTime(data["Fields.2222"]).ToString("yyyy-MM-dd");
            map.Add("investorLockExpDate", investorLockExpDate);
            map.Add("investorLockType", data["Fields.2287"].ToString());

            map.Add("baseYSP", data["Fields.2232"].ToString());
            map.Add("totalAdj", data["Fields.2273"].ToString());
            map.Add("netYSP", data["Fields.2277"].ToString());
            map.Add("netSRP", data["Fields.2276"].ToString());


            map.Add("processor", data["Fields.362"].ToString());
            map.Add("loanOfficer", data["Fields.317"].ToString());

            map.Add("servicingStatus", data["Fields.SERVICE.X8"].ToString());//servicing status
            map.Add("paymentsCollected", data["Fields.SERVICE.X39"].ToString());//number of payments

            String firstPaymentDate = (data["Fields.682"] == null) ? null : Convert.ToDateTime(data["Fields.682"]).ToString("yyyy-MM-dd");
            map.Add("firstPaymentDate", firstPaymentDate);//first payment date

            String firstPaymentDateInvestor = (data["Fields.3514"] == null) ? null : Convert.ToDateTime(data["Fields.3514"]).ToString("yyyy-MM-dd");
            map.Add("firstPaymentDateInvestor", firstPaymentDateInvestor);//1st payment to investor

            String mortgageStatementLastPrinted = (data["Fields.SERVICE.X9"] == null) ? null : Convert.ToDateTime(data["Fields.SERVICE.X9"]).ToString("yyyy-MM-dd");
            map.Add("mortgageStatementLastPrinted", mortgageStatementLastPrinted);//last statement printed date
            return map;
        }

        private void startApplication()
        {
            log.Debug("Program running...");
            
            //setup folder query
            StringFieldCriterion folderCri = selectLoanFolder();

            //execute encompass query
            LoanReportCursor results = encompassQueryBuilder(folderCri);
            int total = results.Count;
            int count = 0;
            log.Info("Total Files: " + total);


            //query list for mySql
            //List<MySqlCommand> queries = new List<MySqlCommand>();

            log.Debug("connecting to MySQL...");
            db_connect connection = new db_connect();
            connection.connect();
            log.Debug("connected to MySQL");
            MySqlCommand cmd = new MySqlCommand();
            cmd.Connection = connection.getConnection();


            foreach (LoanReportData data in results)
            {
                Dictionary<String, String> map = dbQueryBuilder(data);

                /*cmd.CommandText = string.Format(
                    "INSERT INTO loans(guid, investor, investorNum, createdAt, b1_lname, b1_fname, loanAmt, loanNum, fundedDate, processor, loan_officer, purchasedDate) " + 
                    "values(@v1, @v2, @v3, @createdAt, @v4, @v5, @v6, @v7, @v8, @v9, @v10, " + 
                    "@v11, @currMilestone) " +
                    "ON DUPLICATE KEY UPDATE guid=@v1, investor=@v2, investorNum=@v3, updatedAt=@updatedAt, b1_lname=@v4, b1_fname=@v5, "+
                    "loanAmt=@v6, loanNum=@v7, fundedDate=@v8, processor=@v9, loan_officer=@v10, " + 
                    "purchasedDate=@v11"
                    );*/

                var now  = timestamp.ToString("yyyy - MM - dd HH: mm:ss");
          

                StringBuilder insert = new StringBuilder();
                insert.Append("INSERT INTO loans(guid, createdAt, loanFolder ");
                //cols
                foreach(String key in map.Keys)
                {
                    insert.Append(", " + key);
                }
                //values
                insert.Append(") ");
                StringBuilder values = new StringBuilder();
                values.Append("VALUES('" + data.Guid + "', '" + now + "', '" + folderCri.Value + "'"  );
                foreach(String key in map.Keys)
                {
                    if (map[key] != null)
                    {
                        values.Append(", '" + MySqlHelper.EscapeString(" " + map[key]) + "'");
                    }
                    else
                    {
                        values.Append(", NULL");
                    }
                }
                values.Append(") ");
                StringBuilder update2 = new StringBuilder();
                update2.Append("ON DUPLICATE KEY UPDATE updatedAt='" + now + "', loanFolder='" + folderCri.Value+ "'");
                foreach(String key in map.Keys)
                {
                    if (map[key] != null)
                    {
                        update2.Append(", " + key + "='" + MySqlHelper.EscapeString(" " + map[key]) + "'");
                    }
                    else
                    {
                        update2.Append(", " + key + "=NULL");
                    }
                }

                //mysql command
                cmd.CommandText = insert.ToString() + values.ToString() + update2.ToString();

               /* cmd.Parameters.Clear();
                cmd.Parameters.AddWithValue("@v1", data.Guid);
                cmd.Parameters.AddWithValue("@v2", map["investor"]);
                cmd.Parameters.AddWithValue("@v3", map["investorNum"]);
                cmd.Parameters.AddWithValue("@createdAt", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@updatedAt", timestamp.ToString("yyyy-MM-dd HH:mm:ss"));
                cmd.Parameters.AddWithValue("@v4", map["b1_lname"]);
                cmd.Parameters.AddWithValue("@v5", map["b1_fname"]);
                cmd.Parameters.AddWithValue("@v6", map["loanAmt"]);
                cmd.Parameters.AddWithValue("@v7", map["loanNum"]);
                cmd.Parameters.AddWithValue("@v8", map["fundedDate"]);
                cmd.Parameters.AddWithValue("@v9", map["processor"]);
                cmd.Parameters.AddWithValue("@v10", map["loanOfficer"]);                    
                cmd.Parameters.AddWithValue("@v11", map["purchasedDate"]);
                cmd.Prepare();*/

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

            TimeSpan time = DateTime.Now.Subtract(timestamp);

            /*connection.addLog("db_update", "updated: " + folderCri.Value + total + " rows in " + 
                String.Format("{0} min {1}.{2} sec"
                    , time.Minutes
                    , time.Seconds
                    , time.Milliseconds) );*/

            connection.close();

            log.Info("Finished updating db in " + 
                String.Format("{0} min {1}.{2} sec"
                    , time.Minutes
                    , time.Seconds
                    , time.Milliseconds));
        }
    }
}
