using System;
using System.Text;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.BusinessObjects.Loans;
using System.Collections.Generic;

namespace ReportFunded
{
    class Program
    {
        public static object ConfigurationManager { get; private set; }

        static void Main(string[] args)
        {
            Console.Out.WriteLine("Program started...");
            DateTime timestamp = DateTime.Now;

            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();
            String text = "<html><head>";
            text += "<style>table,td{border:1px solid grey;border-collapse:collapse;padding:.5em;}.small{font-size:.7em;}</style>";
            text += "</head><body>";

            text += startApplication();

            text += "<div class='small'>*Data Sources from Encompass. If report is incorrect, please update information in Encompass.* </div>";
            text += "<div class='small'>Report completed in: "+DateTime.Now.Subtract(timestamp).ToString(@"ss\.fff") + " seconds</div> </body></html>";

            Console.Out.WriteLine("Report ready!");

            //send email
            List<String> to = new List<String>();
            to.Add(System.Configuration.ConfigurationManager.AppSettings["to1"].ToString());
            List<String> cc = new List<String>();
            cc.Add(System.Configuration.ConfigurationManager.AppSettings["cc1"].ToString());
            Utility.SendEmail(to, cc,"Funded Files for " + timestamp.ToShortDateString(), text);

            Console.Out.WriteLine("Program finished...");
        }

        private static Session ConnectToServer()
        {
            Console.Out.WriteLine("connecting to server...");
            Session s = new Session();
            s.Start(System.Configuration.ConfigurationManager.AppSettings["Eserver_address"].ToString(), System.Configuration.ConfigurationManager.AppSettings["Eserver_login"].ToString(),
            System.Configuration.ConfigurationManager.AppSettings["Eserver_pw"].ToString());
            Console.Out.WriteLine("connected.");
            return s;
        }



        private static String startApplication()
        {
            Console.Out.WriteLine("Program running...");

            String text = "";

            //connecting to server
            Session session = ConnectToServer();

            DateFieldCriterion cri = new DateFieldCriterion();
            cri.FieldName = "Fields.Log.MS.Date.Funding";
            cri.Value = DateTime.Now;
            cri.Precision = DateFieldMatchPrecision.Day;

            QueryCriterion fullQuery = cri;
            LoanIdentityList ids = session.Loans.Query(fullQuery);

            int count = ids.Count;
            Console.Out.WriteLine("Total Files Funded " + cri.Value.ToShortDateString() + ": " + count);

            text += "Total Files Funded: <b>" + count + "</b><br/><br/>";

            text += "<table border='1'>";

            text += "<tr><th>Investor</th>";
            text += "<th>Inv #</th>";
            text += "<th>Loan #</th>";
            text += "<th>Borrower Name</th>";
           // text += "<th>First Name</th>";
            text += "<th>Loan Amount</th>";
            text += "<th>Processor</th>";
            text += "<th>Loan Officer</th>";
            text += "</tr>";

            foreach (LoanIdentity id in ids)
            {
                //Console.Out.WriteLine(id.Guid);

                text += "<tr><td>";
                Loan loan = session.Loans.Open(id.Guid);

                StringBuilder line = new StringBuilder();
                //  line.Append(loan.Fields["Log.MS.Date.Funding"] + "</td><td>");
                line.Append(loan.Fields["VEND.X263"] + "</td><td>");
                line.Append(loan.Fields["352"] + "</td><td>");
                line.Append(loan.Fields["364"] + "</td><td>");
                line.Append(loan.Fields["37"]+", "+ loan.Fields["4000"] + "</td><td>");
               // line.Append(loan.Fields["4000"] + "</td><td>");
                line.Append(loan.Fields["1109"] + "</td><td>");
                line.Append(loan.Fields["362"] + "</td><td>");
                line.Append(loan.Fields["317"] + "</td>");

                text += line.ToString() + "</tr>";
                Console.Out.Write(".");
                //Console.Out.WriteLine(line.ToString());
                loan.Close();

            }

            text += "</table>";
            //close session
            session.End();

            return text;

        }
    }

}
