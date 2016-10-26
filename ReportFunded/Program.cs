using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;

namespace ReportFunded
{
    class Program
    {
        public static object ConfigurationManager { get; private set; }

        static void Main(string[] args)
        {

            //connect
            Session session = Utility.ConnectToServer();

            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();

            //report1(session);
            report2(session);

            session.End();

        }

        public static void report1(Session session)
        {
            Console.Out.WriteLine("Funded Report started...");

            //run funding report
            FundedReport report1 = new FundedReport(session);
            String text = report1.run();

            //send email
            List<String> to = new List<String>();
            to.Add(System.Configuration.ConfigurationManager.AppSettings["to1"].ToString());
            List<String> cc = new List<String>();
            cc.Add(System.Configuration.ConfigurationManager.AppSettings["cc1"].ToString());
            Utility.SendEmail(to, cc, "Funded Files for " + DateTime.Now.ToShortDateString(), text);

            Console.Out.WriteLine("Funded Report finished...");
        }

        public static void report2(Session session)
        {
            Console.Out.WriteLine("Not Purchased Report started...");

            //run report
            NotPurchasedReport report2 = new NotPurchasedReport(session);
            String text = report2.run();

            //send email
            List<String> to = new List<String>();
            to.Add(System.Configuration.ConfigurationManager.AppSettings["to1"].ToString());
            List<String> cc = new List<String>();
            cc.Add(System.Configuration.ConfigurationManager.AppSettings["cc1"].ToString());
            Utility.SendEmail(to, cc, "Not Purchased Files for " + DateTime.Now.ToShortDateString(), text);

            Console.Out.WriteLine("Not Purchased Report finished...");
            
        }



    }

}
