using System;
using System.Collections.Generic;

namespace ReportFunded
{
    class Program
    {
        public static object ConfigurationManager { get; private set; }

        static void Main(string[] args)
        {
            Console.Out.WriteLine("Program started...");

            //run funding report
            FundedReport report1 = new FundedReport();
            String text1 = report1.run();

            //send email
            List<String> to = new List<String>();
            to.Add(System.Configuration.ConfigurationManager.AppSettings["to1"].ToString());
            List<String> cc = new List<String>();
            cc.Add(System.Configuration.ConfigurationManager.AppSettings["cc1"].ToString());
            Utility.SendEmail(to, cc,"Funded Files for " + DateTime.Now.ToShortDateString(), text1);

            Console.Out.WriteLine("Program finished...");
        }


       
    }

}
