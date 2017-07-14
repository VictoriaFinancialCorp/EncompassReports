using ReportFunded.Reports;
using System;
using System.Collections.Generic;
using log4net;


namespace ReportFunded
{
    class Program
    {

        public static Boolean debug;
        public static List<String> to;
        public static List<String> cc;
        public static List<String> bcc;
        public static int report = 0;
        public static SessionManager mySession;
        private static readonly ILog log = LogManager.GetLogger(typeof(Program));


        static void Main(string[] args)
        {
            log.Info("Starting Report Application...");
            DateTime timestamp = DateTime.Now;


            List<String> reports = new List<String> {
                "Funded Report",
                "Not Purchased Report",
                "Not CTC Report < 60 days",
                "Processors Report",
                "Locked for Files CTC, Not Funded Report",
                "Funded w/o Investor Lock Report",
                "Accounting Report for Current Month",
                "db testing",
                "db update: 'My Pipeline'",
                "db update: 'Completed Loans'",
                "db update: 'Servicing'",
                "db update: 'Adverse'",
                "db update: ALL"
            };

            parseArgs(args);
            new EllieMae.Encompass.Runtime.RuntimeServices().Initialize();

            if (args.Length == 0)
            {
                debug = true;
                Console.Out.WriteLine("----------------------");
                Console.Out.WriteLine("Select report to run:");
                for (int i =0;i<reports.Count;i++)
                {
                    Console.Out.WriteLine(i+" "+ reports[i]);
                }
                Console.Out.WriteLine("----------------------");
                int input = Int32.Parse(Console.In.ReadLine());
                report = input;
                Console.Out.WriteLine("Selection: "+reports[input]);

            }
            else
            {
                debug = false;
            }
            String text = null;

            mySession = new SessionManager();

            log.Info("Encompass Session begin");
            switch (report)
            {

                case 0:
                    FundedReport report1 = new FundedReport();
                    text = report1.run();
                    outputReport(reports[report], text);
                    break;
                case 1:
                    NotPurchasedReport report2 = new NotPurchasedReport();
                    text = report2.run();
                    outputReport(reports[report], text);
                    break;
                case 2:
                    NotCTCReport report3 = new NotCTCReport();
                    text = report3.run();
                    outputReport(reports[report], text);
                    break;
                case 3:
                    ProcessorsReport report4 = new ProcessorsReport();
                    text = report4.run();
                    outputReport(reports[report], text);
                    break;
                case 4:
                    Locks4CTCNotFundedReport report5 = new Locks4CTCNotFundedReport();
                    text = report5.run();
                    outputReport(reports[report], text);
                    break;
                case 5:
                    FundedNotLockedReport report6 = new FundedNotLockedReport();
                    text = report6.run();
                    outputReport(reports[report], text);
                    break;
                case 6:
                    Accounting report7 = new Accounting();
                    text = report7.run();
                    outputReport(reports[report], text);
                    break;
                case 7:
                    db.db_connect connection = new db.db_connect();
                    connection.connect();
                    log.Debug("everything working");
                    connection.close();
                    break;
                case 8:
                    db.EncompassUpdate update = new db.EncompassUpdate();
                    update.run(report);
                    break;
                case 9:
                    db.EncompassUpdate update1 = new db.EncompassUpdate();
                    update1.run(report);
                    break;
                case 10:
                    db.EncompassUpdate update2 = new db.EncompassUpdate();
                    update2.run(report);
                    break;
                case 11:
                    db.EncompassUpdate update3 = new db.EncompassUpdate();
                    update3.run(report);
                    break;
                case 12:
                    db.EncompassUpdate updateAll = new db.EncompassUpdate();
                    updateAll.run(8);
                    updateAll.run(9);
                    updateAll.run(10);
                    updateAll.run(11);
                    updateAll.run(12);
                    break;
                default:
                    log.Error("No report chosen");
                    break;

            }
            log.Info("Encompass session closing..");
            mySession.closeSession();

            TimeSpan time = DateTime.Now.Subtract(timestamp);
            log.Info("Report Application Finished in " +
                String.Format("{0} min {1}.{2} sec"
                    , time.Minutes
                    , time.Seconds
                    , time.Milliseconds));
            //log.Info("Report Application Finished!");

                     
        }

        private static void parseArgs(String[] args)
        {
            to = new List<String>();
            cc = new List<String>();
            bcc = new List<String>();

            HashSet<String> flags = new HashSet<String> {
                "-to", "-cc", "-bcc", "-r"
            };
         


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

        private static void outputReport(String reportName, String message)
        {
            //send email
            if (!Program.debug)
            {
                Utility.SendEmail(Program.to, Program.cc, Program.bcc, reportName + " for " + DateTime.Now.ToShortDateString(), message);
            }
            else
            {
                System.IO.File.WriteAllText(@"report.html", message);
            }
        }

    }

}
