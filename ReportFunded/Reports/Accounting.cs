using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using ReportFunded;


namespace ReportFunded.Reports
{
    
    class Accounting
    {
        private Session session;
        private List<Row> report;

        public Accounting()
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
            /* DateFieldCriterion invLockEmpty = new DateFieldCriterion();
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
             fundDateNotEmpty.MatchType = OrdinalFieldMatchType.Equals;*/

            StringFieldCriterion folderCri = new StringFieldCriterion();
            folderCri.FieldName = "Loan.LoanFolder";
            folderCri.Value = "My Pipeline";
            folderCri.MatchType = StringFieldMatchType.Exact;

            StringFieldCriterion folderCri2 = new StringFieldCriterion();
            folderCri2.FieldName = "Loan.LoanFolder";
            folderCri2.Value = "Completed Loans";
            folderCri2.MatchType = StringFieldMatchType.Exact;

            DateFieldCriterion purchDateNotEmpty = new DateFieldCriterion();
            purchDateNotEmpty.FieldName = "Fields.Log.MS.Date.Purchased";
            purchDateNotEmpty.Value = DateFieldCriterion.NonEmptyDate;
            purchDateNotEmpty.MatchType = OrdinalFieldMatchType.Equals;

            DateFieldCriterion purchDateThisMonth = new DateFieldCriterion();
            purchDateThisMonth.FieldName = "Fields.Log.MS.Date.Purchased";
            purchDateThisMonth.Value = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            purchDateThisMonth.MatchType = OrdinalFieldMatchType.GreaterThan;

            QueryCriterion fullQuery = purchDateThisMonth.And(purchDateNotEmpty).And(folderCri.Or(folderCri2));

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

            row.add("Funded");
            fields.Add("Fields.Log.MS.Date.Funding");

            row.add("Purchased");
            fields.Add("Fields.Log.MS.Date.Purchased");

           // row.add("Base YSP");
            fields.Add("Fields.2232");

           // row.add("Total Adj");
            fields.Add("Fields.2273");

          //  row.add("Net YSP");
            fields.Add("Fields.2274");

           // row.add("Net SRP");
            fields.Add("Fields.2276");

            row.add("Total Rebate");


            

            row.add("Processor");
            fields.Add("Fields.362");

            row.add("Loan Officer");
            fields.Add("Fields.317");

            report.Add(row);


            SortCriterionList sortOrder = new SortCriterionList();
            sortOrder.Add(new SortCriterion("Fields.Log.MS.Date.Purchased", SortOrder.Ascending));

            LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery, sortOrder);

            Console.Out.WriteLine(results.ToString());

            int count = results.Count;
            Console.Out.WriteLine("Total Files" + ": " + count);


            //end program if results empty
            if (count == 0)
            {
                results.Close();
                Environment.Exit(0);
            }

            text += "Total Files Purchased this Month-to-Date: <b>" + count + "</b><br/><br/>";


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

                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Funding"]).ToShortDateString());
                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Purchased"]).ToShortDateString());

             //   line.add(Utility.toPercent(data["Fields.2232"]));
              //  line.add(Utility.toPercent(data["Fields.2273"]));
              //  line.add(Utility.toPercent(data["Fields.2274"]));
              //  line.add(Utility.toPercent(data["Fields.2276"]));

                line.add((Convert.ToDouble(data["Fields.2274"]) + Convert.ToDouble(data["Fields.2276"])).ToString("F3"));
                
               

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
            String text = "";
            text += "<table border='1'>";
            foreach (Row row in report)
            {
                row.toString();
                if (row.isWarning())
                {
                    text += "<tr class='yellow'>";
                }
                else
                {
                    text += "<tr>";
                }
                foreach (String col in row.getRow())
                {
                    if (row.isHeader())
                    {
                        text += "<th>" + col + "</th>";
                    }
                    else
                    {
                        text += "<td>" + col + "</td>";
                    }
                }
                text += "</tr>";
            }
            text += "</table>";
            return text;
        }

    }
}
