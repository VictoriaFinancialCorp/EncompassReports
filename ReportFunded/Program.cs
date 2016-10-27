using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;

namespace ReportFunded
{
    class Program
    {

        public static Boolean debug;

        static void Main(string[] args)
        {
            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();

            if (args.Length == 0)
            {
                debug = true;
                //reportFunded();
                reportNotPurchased();
            }
            else
            {
                debug = false;
                switch (Int32.Parse(args[0]))
                {
                    case 1:
                        reportFunded();
                        break;
                    case 2:
                        reportNotPurchased();
                        break;
                }
                
            }          
        }

        public static void reportFunded()
        {
            Console.Out.WriteLine("Funded Report started...");

            //run funding report
            FundedReport report1 = new FundedReport();
            String text = report1.run();

            //send email
            if (!debug) { 
                List<String> to = new List<String>();
                to.Add(System.Configuration.ConfigurationManager.AppSettings["to1"].ToString());
                List<String> cc = new List<String>();
                cc.Add(System.Configuration.ConfigurationManager.AppSettings["cc1"].ToString());
                Utility.SendEmail(to, cc, "Funded Files for " + DateTime.Now.ToShortDateString(), text);
            }
            else
            {

            }
            Console.Out.WriteLine("Funded Report finished...");
        }

        public static void reportNotPurchased()
        {
            Console.Out.WriteLine("Not Purchased Report started...");

            //run report
            NotPurchasedReport report2 = new NotPurchasedReport();
            String text = report2.run();

            //send email
            if (!debug)
            {
                List<String> to = new List<String>();
                to.Add(System.Configuration.ConfigurationManager.AppSettings["to1"].ToString());
                List<String> cc = new List<String>();
                cc.Add(System.Configuration.ConfigurationManager.AppSettings["cc1"].ToString());
                Utility.SendEmail(to, cc, "Not Purchased Files for " + DateTime.Now.ToShortDateString(), text);
            }
            else 
            {
                System.IO.File.WriteAllText(@"NotPurchasedReport.html", text);
            }
            Console.Out.WriteLine("Not Purchased Report finished...");
            
        }



    }

}
