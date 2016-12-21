﻿using EllieMae.Encompass.Client;
using ReportFunded.Reports;
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
            List<String> reports = new List<String> {
                "Funded Report",
                "Not Purchased Report",
                "Not CTC Report < 60 days",
                "Processors Report",
                "Locked for Files CTC, Not Funded Report",
                "Funded w/o Investor Lock Report",
                "Accounting Report for Current Month",
                "db testing",
                "db update"
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
                    //connection.init();
                   
                    connection.addLog("test", "everything working");
                    connection.close();
                    break;
                case 8:
                    db.EncompassUpdate update = new db.EncompassUpdate();
                    update.run();
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

        public static void outputReport(String reportName, String message)
        {
            //send email
            if (!debug)
            {
                Utility.SendEmail(to, cc, bcc, reportName +" for " + DateTime.Now.ToShortDateString(), message);
            }
            else
            {
                System.IO.File.WriteAllText(@"report.html", message);
            }
        }
        
    }

}
