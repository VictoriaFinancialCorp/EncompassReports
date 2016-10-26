﻿using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using EllieMae.Encompass.Reporting;
using ReportFunded;
using System;
using System.Collections.Generic;
using System.Text;

public class FundedReport
{

    private Session session;

    private List<Row> report;
    public FundedReport()
    {
        this.report = new List<Row>();
    }

    public String run()
	{

        DateTime timestamp = DateTime.Now;

        //connect
        this.session = Utility.ConnectToServer();

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
        cri.Value = DateTime.Now;
        cri.Precision = DateFieldMatchPrecision.Day;

        QueryCriterion fullQuery = cri;

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



        //LoanIdentityList ids = session.Loans.Query(fullQuery);

        int count = results.Count;
        Console.Out.WriteLine("Total Files Funded " + cri.Value.ToShortDateString() + ": " + count);

        text += "Total Files Funded: <b>" + count + "</b><br/><br/>";

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
                if (data[field].GetType() == typeof(System.String))
                {
                    line.add(data[field].ToString());
                }
                else
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
        foreach (Row row in report)
        {
            row.toString();
            text += "<tr>";
            foreach (String col in row.getRow())
            {
                text += "<td>" + col + "</td>";
                if (Program.debug)
                {
                    Console.Out.Write(col+"\t");
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
