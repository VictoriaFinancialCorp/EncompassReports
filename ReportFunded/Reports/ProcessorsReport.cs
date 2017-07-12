using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using ReportFunded;
using System;
using System.Collections.Generic;
using System.IO;
using log4net;

namespace ReportFunded.Reports
{
    class ProcessorsReport
    {

        private Session session;
        private List<Row> report;
        private static readonly ILog log = LogManager.GetLogger(typeof(ProcessorsReport));

        private int days = 180; //report in past num of days

        public ProcessorsReport()
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
            cri2.Value = DateTime.Today.AddDays(-1 * days);  //last num days
            cri2.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;

            StringFieldCriterion folderCri = new StringFieldCriterion();
            folderCri.FieldName = "Loan.LoanFolder";
            folderCri.Value = "My Pipeline";
            folderCri.MatchType = StringFieldMatchType.Exact;

            QueryCriterion fullQuery = folderCri.And(cri.And(cri2));

            StringList fields = new StringList();
            Row header = new Row();
            header.setHeader(true);
            header.add("Milestone");
            fields.Add("Fields.Log.MS.CurrentMilestone");

            header.add("Date Started");
            fields.Add("Fields.Log.MS.Date.Started");

            header.add("Date Submitted");
            fields.Add("Fields.Log.MS.Date.Submittal");

            header.add("Loan #");
            fields.Add("Fields.364");

            header.add("Borrower Name");
            fields.Add("Fields.4002");
            fields.Add("Fields.4000");

            header.add("Address");
            fields.Add("Fields.11");

            header.add("Loan Amount");
            fields.Add("Fields.1109");

            header.add("Purpose");
            fields.Add("Fields.19");

            header.add("Term");
            fields.Add("Fields.4");

            header.add("Rate");
            fields.Add("Fields.3");

            header.add("Locked Date");
            fields.Add("Fields.761");

            header.add("Processor");
            fields.Add("Fields.362");

            header.add("Loan Officer");
            fields.Add("Fields.317");

            header.add("Milestone Notes");
            fields.Add("Fields.Log.MS.Stage");
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
            fields.Add("Fields.Log.MS.Comments.Sent to Title");
            fields.Add("Fields.Log.MS.Comments.Shipping");
            fields.Add("Fields.Log.MS.Comments.Started");
            fields.Add("Fields.Log.MS.Comments.Submittal");


            report.Add(header);


            SortCriterionList sortOrder = new SortCriterionList();
            sortOrder.Add(new SortCriterion("Fields.362", SortOrder.Ascending));
            sortOrder.Add(new SortCriterion("Fields.Log.MS.Date.Started", SortOrder.Descending));

            LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery, sortOrder);

            //Console.Out.WriteLine(results.ToString());

            int count = results.Count;
            log.Info("Total Files" + ": " + count);

            text += "Total Files:<b>" + count + "</b> active last " + days + " days<br/><br/>";

            String currProcessor = "";//"null" string
            count = 0;

            //iterate through query and format
            foreach (LoanReportData data in results)
            {
                if (!currProcessor.Equals(data["Fields.362"].ToString()))
                {
                    Row subheader = new Row();
                    subheader.setHeader(true);
                    subheader.add("Processor: ");
                    subheader.add(currProcessor);
                    subheader.add("Count: " + count);
                    report.Add(subheader);
                    text += formatReport(report);
                    text += "<br/><br/>";

                    // reset
                    count = 0;
                    report.Clear();
                    currProcessor = data["Fields.362"].ToString();

                    report.Add(header);



                }
                count++;
                Row line = new Row();
                line.add(data["Fields.Log.MS.CurrentMilestone"].ToString());
                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Started"]).ToShortDateString());

                if (Math.Ceiling(DateTime.Now.Subtract(Convert.ToDateTime(data["Fields.Log.MS.Date.Started"])).TotalDays) > 60)
                {
                    line.setWarn(true);
                }
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
                //milestone comments
                string nextMilestone = data["Fields.Log.MS.Stage"].ToString();

                //string notes = "";
                StringList log = new StringList();
                log.Add(data["Fields.Log.MS.Comments.Started"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Processing"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Submittal"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Cond Approval"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Resubmittal"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Clear to Close"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Ready for Docs"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Docs Drawn"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Docs Signing"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Funding"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Purchased"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Shipping"].ToString());
                log.Add(data["Fields.Log.MS.Comments.Completion"].ToString());
                String notes = "";
                foreach (string l in log)
                {
                    if (l.Trim().Length != 0)
                    {
                        using (StringReader reader = new StringReader(l.ToString()))
                        {
                            string li = "";
                            while ((li = reader.ReadLine()) != null)
                            {
                                notes += li.Trim() + "<br/>";
                            }

                        }

                    }

                }
                line.add(notes);

                if (Program.debug)
                {
                    line.add(data.Guid);
                }


                report.Add(line);
                Console.Out.Write("."); //status bar
            }
            Console.Out.WriteLine("");
            results.Close();

            //text += formatReport(report);

            return text;

        }

        private String formatReport(List<Row> report)
        {
            HashSet<String> processors = new HashSet<String>();


            String text = "<table border='1'>";
            foreach (Row row in report)
            {
                //row.toString();
                if (row.isWarning())
                {
                    text += "<tr class='yellow'>";
                }
                else
                {
                    text += "<tr>";
                }

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
                }
                text += "</tr>";
            }
            text += "</table>";
            return text;
        }
    }
}