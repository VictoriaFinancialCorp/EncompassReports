using EllieMae.Encompass.Client;
using System;
using System.Collections.Generic;

namespace ReportFunded
{
    class Program
    {

        public static Boolean debug;
        public static List<String> to;
        public static List<String> cc;
        public static List<String> bcc;
        public static int report = 0;

        static void Main(string[] args)
        {

            parseArgs(args);
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
                switch (report)
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

        public static void parseArgs(String[] args)
        {
            to = new List<String>();
            cc = new List<String>();
            bcc = new List<String>();//TODO: add bcc feature to reports

            HashSet<String> flags = new HashSet<String>();
            flags.Add("-to");
            flags.Add("-cc");
            flags.Add("-bcc");
            flags.Add("-r");//report number

            String currFlag = null;

            for(int i =0; i < args.Length; i++)
            {
                if (flags.Contains(args[i]))
                {
                    currFlag = args[i];
                }
                else
                {
                    if (currFlag.Equals("-to"))
                    {
                        String[] temp = args[i].Split(',');
                        foreach (String t in temp) {
                            to.Add(t);
                        }
                    }
                    else if(currFlag.Equals("-cc"))
                    {
                        String[] temp = args[i].Split(',');
                        foreach (String t in temp)
                        {
                            cc.Add(t);
                        }
                    }
                    else if (currFlag.Equals("-bcc"))
                    {
                        String[] temp = args[i].Split(',');
                        foreach (String t in temp)
                        {
                            bcc.Add(t);
                        }
                    }
                    else if (currFlag.Equals("-r"))
                    {
                        report = int.Parse(args[i]);
                    }
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
                Utility.SendEmail(to, cc, "Funded Files for " + DateTime.Now.ToShortDateString(), text);
            }
            else
            {
                System.IO.File.WriteAllText(@"FundedReport.html", text);
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
