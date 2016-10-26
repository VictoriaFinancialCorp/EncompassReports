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

        String text = "<html><head>";
        text += "<style>table,td{border:1px solid grey;border-collapse:collapse;padding:.5em;}.small{font-size:.7em;}</style>";
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
        cri.FieldName = "Fields.Log.MS.Date.Funding";
        cri.Value = DateFieldCriterion.NonEmptyDate;
        cri.MatchType = OrdinalFieldMatchType.Equals;

        DateFieldCriterion cri2 = new DateFieldCriterion();
        cri2.FieldName = "Fields.Log.MS.Date.Purchased";
        cri2.Value = DateFieldCriterion.EmptyDate;
        cri2.MatchType = OrdinalFieldMatchType.Equals;

        NumericFieldCriterion laCri = new NumericFieldCriterion();
        laCri.FieldName = "Loan.LoanAmount";
        laCri.Value = 100000;
        laCri.MatchType = OrdinalFieldMatchType.GreaterThanOrEquals;

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

        LoanReportCursor results = session.Reports.OpenReportCursor(fields, fullQuery);

        Console.Out.WriteLine(results.ToString());

        int count = results.Count;
        Console.Out.WriteLine("Total Files Not Purchased " + DateTime.Now.ToShortDateString() + ": " + count);

        text += "Total Files Not Purchased: <b>" + count + "</b><br/><br/>";

        //headers
        Row row = new Row();
        row.add("Investor");
        row.add("Inv #");
        row.add("Loan #");
        row.add("Last Name");
        row.add("First Name");
        row.add("Loan Amount");
        row.add("Processor");
        row.add("Loan Officer");
        report.Add(row);
        
        foreach (LoanReportData data in results)
        {
            Row line = new Row();
            foreach (String field in fields)
            {
                if(data[field].GetType() == typeof(System.String))
                {
                    line.add(data[field].ToString());
                }else
                {
                    int value = Convert.ToInt32(data[field]); 
                   // Console.Out.WriteLine(value.ToString("C"));
                    line.add(value.ToString("C"));
                }
                
            }
            report.Add(line);
            Console.Out.Write("."); //status bar
        }
        Console.Out.WriteLine("");
        results.Close();

        text += formatReport(report) ;

        return text;

    }
    public class Row {
        List<String> cols;
        public Row(){
            this.cols = new List<String>();
        }
        public void add(String element)
        {
            this.cols.Add(element);
        }
        public List<String> getRow()
        {
            return cols;
        }
        public String toString()
        {
            return this.cols.ToString();
        }

        internal void add(object p)
        {
            throw new NotImplementedException();
        }
    };

    private String formatReport(List<Row> report)
    {
        String text = "<table border='1'>";
        foreach(Row row in report)
        {
            row.toString();
            text += "<tr>";
            foreach(String col in row.getRow())
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
