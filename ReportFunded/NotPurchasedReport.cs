using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using ReportFunded;
using System;
using System.Collections.Generic;
using System.Globalization;

public class NotPurchasedReport
{

    private Session session;
    private List<Row> report;

    public NotPurchasedReport()
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

        html += HtmlReport.getFooter();

        Console.Out.WriteLine("Report ready!");
        session.End();
        return html;
    }
    private String startApplication()
    {
        Console.Out.WriteLine("Program running...");

        String text = "";


        DateFieldCriterion cri = new DateFieldCriterion();
        cri.FieldName = "Fields.Log.MS.Date.Funding";
        cri.Value = DateFieldCriterion.NonEmptyDate;
        cri.MatchType = OrdinalFieldMatchType.Equals;

        DateFieldCriterion cri2 = new DateFieldCriterion();
        cri2.FieldName = "Fields.Log.MS.Date.Purchased";
        cri2.Value = DateFieldCriterion.EmptyDate;
        cri2.MatchType = OrdinalFieldMatchType.Equals;

        StringFieldCriterion folderCri = new StringFieldCriterion();
        folderCri.FieldName = "Loan.LoanFolder";
        folderCri.Value = "My Pipeline";
        folderCri.MatchType = StringFieldMatchType.Exact;

        QueryCriterion fullQuery = folderCri.And(cri.And(cri2));

        StringList fields = new StringList();
        fields.Add("Fields.VEND.X263");
        fields.Add("Fields.352");
        fields.Add("Fields.364");
        fields.Add("Fields.37");
        fields.Add("Fields.4000");
        fields.Add("Fields.1109");
        fields.Add("Fields.362");
        fields.Add("Fields.317");
        fields.Add("Fields.Log.MS.Date.Funding");
        SortCriterionList sortOrder = new SortCriterionList();
        sortOrder.Add(new SortCriterion("Fields.Log.MS.Date.Funding"));

        LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery, sortOrder);

        Console.Out.WriteLine(results.ToString());

        int count = results.Count;
        Console.Out.WriteLine("Total Files Not Purchased " + DateTime.Now.ToShortDateString() + ": " + count);

        text += "Total Files Not Purchased: <b>" + count + "</b><br/><br/>";

        //headers
        Row row = new Row();
        row.setHeader(true);
        row.add("Investor");
        row.add("Inv #");
        row.add("Loan #");
        row.add("Borrower Name");
        //row.add("First Name");
        row.add("Loan Amount");
        row.add("Processor");
        row.add("Loan Officer");
        row.add("Funding Date");
        report.Add(row);

        
        foreach (LoanReportData data in results)
        {
            Row line = new Row();
            line.add(data["Fields.VEND.X263"].ToString());
            line.add(data["Fields.352"].ToString());
            line.add(data["Fields.364"].ToString());
            line.add(data["Fields.37"].ToString().ToUpper() + ", " + data["Fields.4000"].ToString().ToUpper());
            line.add(Convert.ToInt32(data["Fields.1109"]).ToString("C"));
            line.add(data["Fields.362"].ToString());
            line.add(data["Fields.317"].ToString());
            line.add(Math.Ceiling(DateTime.Now.Subtract(Convert.ToDateTime(data["Fields.Log.MS.Date.Funding"])).TotalDays).ToString());

            report.Add(line);
            Console.Out.Write("."); //status bar
        }
        Console.Out.WriteLine("");
        results.Close();

        text += formatReport(report) ;

        return text;

    }


    private String formatReport(List<Row> report)
    {
        String text = "<table border='1'>";
        foreach(Row row in report)
        {
            row.toString();
            text += "<tr>";
            foreach(String col in row.getRow())
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
