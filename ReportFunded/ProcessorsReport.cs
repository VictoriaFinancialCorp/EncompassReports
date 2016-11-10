using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using ReportFunded;
using System;
using System.Collections.Generic;

public class ProcessorsReport
{

    private Session session;
    private List<Row> report;

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

        String text = "<html><head><style>";
        text += "table,th,td{text-align:center;border:1px solid grey;border-collapse:collapse;padding:.5em;font-size:.9em;}";
        text += "table{border:2px solid grey}";
        text += ".small{font-size:.7em;}";
        text += "</style></head><body>";

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
        cri2.Value = DateTime.Today.AddDays(-1*days);  //last num days
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
      
        report.Add(header);


        SortCriterionList sortOrder = new SortCriterionList();
        sortOrder.Add(new SortCriterion("Fields.362", SortOrder.Ascending));
        sortOrder.Add(new SortCriterion("Fields.Log.MS.Date.Started",SortOrder.Descending));

        LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery, sortOrder);

        //Console.Out.WriteLine(results.ToString());

        int count = results.Count;
        Console.Out.WriteLine("Total Files" + ": " + count);

        text += "Total Files:<b>" + count + "</b> active last "+ days + " days<br/><br/>";

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
            line.add(Utility.toShortDate(data["Fields.Log.MS.Date.Submittal"]));
            line.add(data["Fields.364"].ToString());
            line.add(data["Fields.4002"].ToString().ToUpper()+", "+ data["Fields.4000"].ToString().ToUpper());
            line.add(data["Fields.11"].ToString().ToUpper());
            line.add(Convert.ToInt32(data["Fields.1109"]).ToString("C"));
            line.add(data["Fields.19"].ToString());
            line.add(Convert.ToInt32(data["Fields.4"]).ToString());
            line.add(Convert.ToDouble(data["Fields.3"]).ToString("F3"));
            line.add(Utility.toShortDate(data["Fields.761"]));
            line.add(data["Fields.362"].ToString());
            line.add(data["Fields.317"].ToString());

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
            text += "<tr>";
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
