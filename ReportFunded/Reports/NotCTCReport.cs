using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using ReportFunded;
using System;
using System.Collections.Generic;
using log4net;

namespace ReportFunded.Reports
{
    class NotCTCReport
    {

        private Session session;
        private List<Row> report;
        private static readonly ILog log = LogManager.GetLogger(typeof(NotCTCReport));

        public NotCTCReport()
        {
            this.report = new List<Row>();
        }

        public String run()
        {
            //connect
            session = Utility.ConnectToServer();

            DateTime timestamp = DateTime.Now;

            String html = HtmlReport.getHeader();
            html += "<body>";

            html += startApplication();

            html += HtmlReport.getFooter(timestamp);

            log.Info("Report ready!");
            session.End();
            return html;
        }
        private String startApplication()
        {
            log.Info("Program running...");

            String text = "";


            DateFieldCriterion cri = new DateFieldCriterion();
            cri.FieldName = "Fields.Log.MS.Date.Clear to Close";
            cri.Value = DateFieldCriterion.EmptyDate;
            cri.MatchType = OrdinalFieldMatchType.Equals;

            DateFieldCriterion cri2 = new DateFieldCriterion();
            cri2.FieldName = "Fields.Log.MS.Date.Started";
            cri2.Value = DateTime.Today.AddDays(-60);  //last 60 days
            cri2.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;

            StringFieldCriterion folderCri = new StringFieldCriterion();
            folderCri.FieldName = "Loan.LoanFolder";
            folderCri.Value = "My Pipeline";
            folderCri.MatchType = StringFieldMatchType.Exact;

            QueryCriterion fullQuery = folderCri.And(cri.And(cri2));

            StringList fields = new StringList();
            Row row = new Row();
            row.setHeader(true);

            row.add("Milestone");
            fields.Add("Fields.Log.MS.CurrentMilestone");

            row.add("Date Started");
            fields.Add("Fields.Log.MS.Date.Started");

            row.add("Date Submitted");
            fields.Add("Fields.Log.MS.Date.Submittal");

            row.add("Loan #");
            fields.Add("Fields.364");

            row.add("Borrower Name");
            fields.Add("Fields.4002");
            fields.Add("Fields.4000");

            row.add("Address");
            fields.Add("Fields.11");

            row.add("Loan Amount");
            fields.Add("Fields.1109");

            row.add("Purpose");
            fields.Add("Fields.19");

            row.add("Term");
            fields.Add("Fields.4");

            row.add("Rate");
            fields.Add("Fields.3");

            row.add("Locked Date");
            fields.Add("Fields.761");

            row.add("Processor");
            fields.Add("Fields.362");

            row.add("Loan Officer");
            fields.Add("Fields.317");

            report.Add(row);


            SortCriterionList sortOrder = new SortCriterionList();
            sortOrder.Add(new SortCriterion("Fields.Log.MS.Date.Started", SortOrder.Ascending));

            LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery, sortOrder);

            Console.Out.WriteLine(results.ToString());

            int count = results.Count;
            log.Info("Total Files Not CTC " + ": " + count);

            text += "Total Files Not CTC last 60 days: <b>" + count + "</b><br/><br/>";


            //iterate through query and format
            foreach (LoanReportData data in results)
            {

                Row line = new Row();
                line.add(data["Fields.Log.MS.CurrentMilestone"].ToString());
                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Started"]).ToShortDateString());
                line.add(Utility.toShortDate(data["Fields.Log.MS.Date.Submittal"]));
                line.add(data["Fields.364"].ToString());
                line.add(data["Fields.4002"].ToString().ToUpper() + ", " + data["Fields.4000"].ToString().ToUpper());
                line.add(data["Fields.11"].ToString().ToUpper());
                line.add(Convert.ToInt32(data["Fields.1109"]).ToString("C"));
                line.add(data["Fields.19"].ToString());
                line.add(Convert.ToInt32(data["Fields.4"]).ToString());
                line.add(Convert.ToDouble(data["Fields.3"]).ToString("F3"));
                line.add(Utility.toShortDate(data["Fields.761"]));
                line.add(data["Fields.362"].ToString());
                line.add(data["Fields.317"].ToString());



                report.Add(line);
                Console.Out.Write("."); //status bar
            }
            Console.Out.WriteLine("");
            results.Close();

            text += formatReport(report);

            return text;

        }

        private String formatReport(List<Row> report)
        {
            String text = "<table border='1'>";
            foreach (Row row in report)
            {
                row.toString();
                text += "<tr>";
                foreach (Col col in row.getRow())
                {
                    if (row.isHeader())
                    {
                        text += "<th>" + col.toString() + "</th>";
                    }
                    else
                    {
                        text += "<td>" + col.toString() + "</td>";
                    }
                    if (Program.debug)
                    {
                        Console.Out.Write(col + "\t");
                    }
                }
                if (Program.debug)
                {
                    Console.Out.WriteLine("");
                }
                text += "</tr>";
            }
            text += "</table>";
            return text;
        }
    }
}