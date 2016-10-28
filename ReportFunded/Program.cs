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
                report = 3;//manually choose report to run
            }
            else
            {
                debug = false;
            }
            String text = null;
            switch (report)
            {
                
                case 1:
                    FundedReport report1 = new FundedReport();
                    text = report1.run();
                    outputReport("Funded Report", text);
                    break;
                case 2:
                    NotPurchasedReport report2 = new NotPurchasedReport();
                    text = report2.run();
                    outputReport("Not Purchased Report", text);
                    break;
                case 3:
                    NotCTCReport report3 = new NotCTCReport();
                    text = report3.run();
                    outputReport("Not CTC Report", text);
                    break;
                default:
                    Console.Out.WriteLine("[Error]: No report chosen");
                    break;
                    
            }
            Console.Out.WriteLine("Report Finished");

                     
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

        public static void outputReport(String reportName, String message)
        {
            //send email
            if (!debug)
            {
                Utility.SendEmail(to, cc, reportName +" for " + DateTime.Now.ToShortDateString(), message);
            }
            else
            {
                System.IO.File.WriteAllText(@"report.html", message);
            }
        }
        
    }

}
