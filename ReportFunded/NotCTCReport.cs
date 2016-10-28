using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using ReportFunded;
using System;
using System.Collections.Generic;

public class NotCTCReport
{

    private Session session;
    private List<Row> report;

    public NotCTCReport()
    {
        this.report = new List<Row>();
    }

    public String run()
    {
        //connect
        session = Utility.ConnectToServer();

        DateTime timestamp = DateTime.Now;

        String text = "<html><head>";
        text += "<style>table,td{text-align:center;border:1px solid grey;border-collapse:collapse;padding:.5em;font-size:.9em;}.small{font-size:.7em;}</style>";
        text += "</head><body>";

        text += startApplication();

        text += "<div class='small'>*Data Sources from Encompass. If report is incorrect, please update information in Encompass.* </div>";
        text += "<div class='small'>Report completed in: " + DateTime.Now.Subtract(timestamp).ToString(@"ss\.fff") + " seconds</div> </body></html>";

        Console.Out.WriteLine("Report ready!");
        session.End();
        return text;
    }
    private String startApplication()
    {
        Console.Out.WriteLine("Program running...");

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
        fields.Add("Fields.Log.MS.CurrentMilestone");
        fields.Add("Fields.Log.MS.Date.Started");
        fields.Add("Fields.Log.MS.Date.Submittal");
        fields.Add("Fields.364");
        //fields.Add("Fields.1868");
        fields.Add("Fields.4002");
        fields.Add("Fields.4000");
        fields.Add("Fields.11");
        fields.Add("Fields.362");
        fields.Add("Fields.317");

        SortCriterionList sortOrder = new SortCriterionList();
        sortOrder.Add(new SortCriterion("Fields.Log.MS.Date.Started",SortOrder.Ascending));

        LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery, sortOrder);

        Console.Out.WriteLine(results.ToString());

        int count = results.Count;
        Console.Out.WriteLine("Total Files Not CTC " + ": " + count);

        text += "Total Files Not CTC last 60 days: <b>" + count + "</b><br/><br/>";

        //headers
        Row row = new Row();
        row.add("Milestone");
        row.add("Date Started");
        row.add("Date Submitted");
        row.add("Loan #");
        row.add("Borrower Name");
        row.add("Address");
        row.add("Processor");
        row.add("Loan Officer");
        report.Add(row);

        foreach (LoanReportData data in results)
        {

            Row line = new Row();
            line.add(data["Fields.Log.MS.CurrentMilestone"].ToString());
            line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Started"]).ToShortDateString());

            if (Convert.ToDateTime(data["Fields.Log.MS.Date.Submittal"]) == DateTime.MinValue)
            {
                line.add(" ");
            }
            else
            {
                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Submittal"]).ToShortDateString());
            }
            
            line.add(data["Fields.364"].ToString());
            //line.add(data["Fields.1868"].ToString());
            line.add(data["Fields.4002"].ToString().ToUpper()+", "+ data["Fields.4000"].ToString().ToUpper());
            line.add(data["Fields.11"].ToString().ToUpper());
            line.add(data["Fields.362"].ToString());
            line.add(data["Fields.317"].ToString());


           /* foreach (String field in fields)
            {

                if (data[field].GetType() == typeof(System.String))
                {
                    line.add(data[field].ToString());
                }
                else if (field.ToString().Equals("Fields.Log.MS.Date.Funding"))
                {
                    double days = DateTime.Now.Subtract(Convert.ToDateTime(data[field])).TotalDays;
                    line.add(Math.Ceiling(days).ToString());
                }
                else if (data[field].GetType() == typeof(System.DateTime))
                {
                    DateTime value = Convert.ToDateTime(data[field]);
                    line.add(value.ToShortDateString());
                }
                else
                {
                    int value = Convert.ToInt32(data[field]);
                    line.add(value.ToString("C"));
                }

            }*/
            report.Add(line);
            Console.Out.Write("."); //status bar
        }
        Console.Out.WriteLine("");
        results.Close();

        text += formatReport(report);

        return text;

    }
    public class Row
    {
        List<String> cols;
        public Row()
        {
            this.cols = new List<String>();
        }
       /* public void add(String element)
        {
            if(element.GetType() == typeof(System.String))
            {
                this.cols.Add(element.ToString());
            }else if (element.GetType() == typeof(System.DateTime))
            {
                this.cols.Add(Convert.ToDateTime(element).ToShortDateString());
            }

        }*/
        public void add(String col)
        {
            this.cols.Add(col);
        }
        public List<String> getRow()
        {
            return cols;
        }
        public String toString()
        {
            return this.cols.ToString();
        }

   
    };

    private String formatReport(List<Row> report)
    {
        String text = "<table border='1'>";
        foreach (Row row in report)
        {
            row.toString();
            text += "<tr>";
            foreach (String col in row.getRow())
            {
                text += "<td>" + col + "</td>";
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
