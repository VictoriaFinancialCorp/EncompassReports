using EllieMae.Encompass.BusinessObjects.Loans;
using EllieMae.Encompass.Client;
using EllieMae.Encompass.Collections;
using EllieMae.Encompass.Query;
using System;
using System.Text;

public class FundedReport
{

    private Session session;

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
        LoanIdentityList ids = session.Loans.Query(fullQuery);

        int count = ids.Count;
        Console.Out.WriteLine("Total Files Funded " + cri.Value.ToShortDateString() + ": " + count);

        text += "Total Files Funded: <b>" + count + "</b><br/><br/>";

        text += "<table border='1'>";

        text += "<tr><th>Investor</th>";
        text += "<th>Inv #</th>";
        text += "<th>Loan #</th>";
        text += "<th>Borrower Name</th>";
        // text += "<th>First Name</th>";
        text += "<th>Loan Amount</th>";
        text += "<th>Processor</th>";
        text += "<th>Loan Officer</th>";
        text += "</tr>";

        foreach (LoanIdentity id in ids)
        {
            //Console.Out.WriteLine(id.Guid);

            text += "<tr><td>";
            Loan loan = session.Loans.Open(id.Guid);

            StringBuilder line = new StringBuilder();
            //  line.Append(loan.Fields["Log.MS.Date.Funding"] + "</td><td>");
            line.Append(loan.Fields["VEND.X263"] + "</td><td>");
            line.Append(loan.Fields["352"] + "</td><td>");
            line.Append(loan.Fields["364"] + "</td><td>");
            line.Append(loan.Fields["37"] + ", " + loan.Fields["4000"] + "</td><td>");
            // line.Append(loan.Fields["4000"] + "</td><td>");
            line.Append(loan.Fields["1109"] + "</td><td>");
            line.Append(loan.Fields["362"] + "</td><td>");
            line.Append(loan.Fields["317"] + "</td>");

            text += line.ToString() + "</tr>";
            Console.Out.Write(".");
            //Console.Out.WriteLine(line.ToString());
            loan.Close();

        }

        text += "</table>";

        return text;

    }
}
