using System;
using System.Collections.Generic;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;


namespace ReportFunded
{
    class Locks4CTCNotFundedReport
    {
        private Session session;
        private List<Row> report;

        public Locks4CTCNotFundedReport()
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

            Console.Out.WriteLine("Report ready!");
            session.End();
            return html;
        }
        private String startApplication()
        {
            Console.Out.WriteLine("Program running...");

            String text = "";

            //Investor Lock Date
            DateFieldCriterion invLockEmpty = new DateFieldCriterion();
            invLockEmpty.FieldName = "Fields.2220";
            invLockEmpty.Value = DateFieldCriterion.EmptyDate;
            invLockEmpty.MatchType = OrdinalFieldMatchType.Equals;

            //non empty CTC date
            DateFieldCriterion ctcNonEmpty = new DateFieldCriterion();
            ctcNonEmpty.FieldName = "Fields.Log.MS.Date.Clear to Close";
            ctcNonEmpty.Value = DateFieldCriterion.NonEmptyDate;
            ctcNonEmpty.MatchType = OrdinalFieldMatchType.Equals;
         
            //empty funding date
            DateFieldCriterion fundDateEmpty = new DateFieldCriterion();
            fundDateEmpty.FieldName = "Fields.Log.MS.Date.Funding";
            fundDateEmpty.Value = DateFieldCriterion.EmptyDate;
            fundDateEmpty.MatchType = OrdinalFieldMatchType.Equals;

            //or
            DateFieldCriterion fundDateNotEmpty = new DateFieldCriterion();
            fundDateNotEmpty.FieldName = "Fields.Log.MS.Date.Funding";
            fundDateNotEmpty.Value = DateFieldCriterion.NonEmptyDate;
            fundDateNotEmpty.MatchType = OrdinalFieldMatchType.Equals;

            StringFieldCriterion folderCri = new StringFieldCriterion();
            folderCri.FieldName = "Loan.LoanFolder";
            folderCri.Value = "My Pipeline";
            folderCri.MatchType = StringFieldMatchType.Exact;

            DateFieldCriterion purchDateEmpty = new DateFieldCriterion();
            purchDateEmpty.FieldName = "Fields.Log.MS.Date.Purchased";
            purchDateEmpty.Value = DateFieldCriterion.EmptyDate;
            purchDateEmpty.MatchType = OrdinalFieldMatchType.Equals;

            QueryCriterion fullQuery = folderCri.And((ctcNonEmpty.And(fundDateEmpty)));

            StringList fields = new StringList();
            Row row = new Row();
            row.setHeader(true);

            row.add("Investor");
            fields.Add("Fields.2825");

            row.add("Inv #");
            fields.Add("Fields.2826");

            row.add("Loan #");
            fields.Add("Fields.364");

            row.add("Borrower Name");
            fields.Add("Fields.4002");
            fields.Add("Fields.4000");

            row.add("Address");
            fields.Add("Fields.11");

            row.add("Rate");
            fields.Add("Fields.3");

            row.add("Loan Amount");
            fields.Add("Fields.1109");

            row.add("Milestone");
            fields.Add("Fields.Log.MS.CurrentMilestone");

            row.add("Date Started");
            fields.Add("Fields.Log.MS.Date.Started");
            
            row.add("Base YSP");
            fields.Add("Fields.2232");

            row.add("Total Adj");
            fields.Add("Fields.2273");

            row.add("Net YSP");
            fields.Add("Fields.2274");

            row.add("Net SRP");
            fields.Add("Fields.2276");

            row.add("Total Rebate");
            

            row.add("Locked");
            fields.Add("Fields.2400");

            row.add("Victoria Lock Date");
            fields.Add("Fields.761");
            fields.Add("Fields.2149");

            row.add("Inv Lock Date");
            fields.Add("Fields.2220");

            row.add("Inv Lock Exp");
            fields.Add("Fields.2222");

            row.add("P/S/I");
            fields.Add("Fields.1811");

            row.add("Purpose");
            fields.Add("Fields.19");            

            row.add("Processor");
            fields.Add("Fields.362");

            row.add("Loan Officer");
            fields.Add("Fields.317");

            report.Add(row);


            SortCriterionList sortOrder = new SortCriterionList();
            sortOrder.Add(new SortCriterion("Fields.Log.MS.CurrentMilestone", SortOrder.Descending));
            sortOrder.Add(new SortCriterion("Fields.2149", SortOrder.Ascending));

            LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery, sortOrder);

            Console.Out.WriteLine(results.ToString());

            int count = results.Count;
            Console.Out.WriteLine("Total Files" + ": " + count);

            text += "Total Files CTC, Not Funded: <b>" + count + "</b><br/><br/>";


            //iterate through query and format
            foreach (LoanReportData data in results)
            {

                Row line = new Row();
                line.add(data["Fields.2825"].ToString());
                line.add(data["Fields.2826"].ToString());
                line.add(data["Fields.364"].ToString());

                line.add((data["Fields.4002"].ToString() + " " + data["Fields.4000"].ToString()).ToUpper());
                line.add(data["Fields.11"].ToString().ToUpper());
                line.add(Convert.ToDouble(data["Fields.3"]).ToString("F3"));
                line.add(Convert.ToInt32(data["Fields.1109"]).ToString("C"));


                line.add(data["Fields.Log.MS.CurrentMilestone"].ToString());
                
                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Started"]).ToShortDateString());

                line.add(Utility.toPercent(data["Fields.2232"]));
                line.add(Utility.toPercent(data["Fields.2273"]));
                line.add(Utility.toPercent(data["Fields.2274"]));
                line.add(Utility.toPercent(data["Fields.2276"]));
              
                line.add((Convert.ToDouble(data["Fields.2274"]) + Convert.ToDouble(data["Fields.2276"])).ToString("F3"));


                line.add(data["Fields.2400"].ToString());
                line.add(Utility.toShortDate(data["Fields.2149"]));
                line.add(Utility.toShortDate(data["Fields.2220"]));
                line.add(Utility.toShortDate(data["Fields.2222"]));

             
                //occupancy
                line.add(data["Fields.1811"].ToString().Substring(0, 1));

                String purpose = data["Fields.19"].ToString();
                if(purpose.Equals("Cash-Out Refinance"))
                {
                    line.add("C/O Refi");
                }else if(purpose.Equals("NoCash-Out Refinance"))
                {
                    line.add("No C/O Refi");
                }else if (purpose.Equals("Purchase"))
                {
                    line.add("Purch");
                }else
                {
                    line.add(purpose);
                }
               
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
                }
                text += "</tr>";
            }
            text += "</table>";
            return text;
        }

    }
}
