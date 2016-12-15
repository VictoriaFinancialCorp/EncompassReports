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

            row.add("Rebate");

            row.add("Investor Interest");
            fields.Add("Fields.2834");

            row.add("Investor Fees");
            fields.Add("Fields.2373");
            fields.Add("Fields.2375");
            fields.Add("Fields.2377");
            fields.Add("Fields.2379");
            fields.Add("Fields.2381");
            fields.Add("Fields.2383");

            row.add("Appraisal");
            fields.Add("Fields.641");

            row.add("Credit Fee");
            fields.Add("Fields.640");

            row.add("Interest");
            fields.Add("Fields.334");

            row.add("Escrow Fees");
            fields.Add("Fields.NEWHUD2.X11");
            fields.Add("Fields.NEWHUD2.X14");
            fields.Add("Fields.NEWHUD.X808");
            fields.Add("Fields.NEWHUD.X810");
            fields.Add("Fields.NEWHUD.X812");
            fields.Add("Fields.NEWHUD.X814");
            fields.Add("Fields.NEWHUD.X816");
            fields.Add("Fields.NEWHUD.X818");
            fields.Add("Fields.NEWHUD.X639");

            row.add("Title Fees");
            fields.Add("Fields.NEWHUD.X572");
            fields.Add("Fields.NEWHUD.X639");
            fields.Add("Fields.NEWHUD.X215");
            fields.Add("Fields.NEWHUD.X216");
            fields.Add("Fields.1763");
            fields.Add("Fields.1768");
            fields.Add("Fields.1773");
            fields.Add("Fields.1778");
            fields.Add("Fields.NEWHUD.X1604");
            fields.Add("Fields.NEWHUD.X1612");

            row.add("Recording Fee");
            fields.Add("Fields.NEWHUD.X607");

            row.add("Processor");
            fields.Add("Fields.362");

            row.add("Loan Officer");
            fields.Add("Fields.317");

            //loan purpose
            fields.Add("Fields.19");

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
            //local variables
            double rebateTotal = 0;
            double escrowTotal = 0;
            double titleTotal = 0;
            double interestTotal = 0;
            double warehouseFeeTotal = 0;
            double warehouseIntTotal = 0;
            double appraisalTotal = 0;
            double creditTotal = 0;
            double recordingTotal = 0;
            double investorFeeTotal = 0;
            double investorIntTotal = 0;

            text += "Total Files Purchased this Month-to-Date: <b>" + count + "</b><br/><br/>";
            text += "<div class='small'><ul>Fees and Income not included" +
                "<li>warehouse fees and interest</li>" +
                "<li>payoff fees refunded to borrower</li>" +
                "</ul> </div>";

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
                int loanAmt = Convert.ToInt32(data["Fields.1109"]);
                line.add(loanAmt.ToString("C"));

                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Funding"]).ToShortDateString());
                line.add(Convert.ToDateTime(data["Fields.Log.MS.Date.Purchased"]).ToShortDateString());

                //   line.add(Utility.toPercent(data["Fields.2232"]));
                //  line.add(Utility.toPercent(data["Fields.2273"]));
                //  line.add(Utility.toPercent(data["Fields.2274"]));
                //  line.add(Utility.toPercent(data["Fields.2276"]));
                Double rebate = (Convert.ToDouble(data["Fields.2274"]) + Convert.ToDouble(data["Fields.2276"]));
                if (rebate <= 0.0001)
                {
                    line.add(rebate.ToString("F3"), 1);
                    line.add(" ");
                }
                else {
                    line.add(rebate.ToString("F3"));
                    Double rebateAmt = (rebate - 100) * loanAmt * .01;
                    if(rebateAmt <= 0)
                    {
                        line.add(rebateAmt.ToString("C"), 2);
                    }else
                    {
                        line.add(rebateAmt.ToString("C"));
                    }
                    rebateTotal += rebateAmt;
                    
                }
                double investorInt = Convert.ToDouble(data["Fields.2834"]);
                line.add((investorInt * -1).ToString("C"));
                investorIntTotal += investorInt;


                double investorFees = Convert.ToDouble(data["Fields.2373"]) + Convert.ToDouble(data["Fields.2375"])
                    + Convert.ToDouble(data["Fields.2377"]) + Convert.ToDouble(data["Fields.2379"])
                    + Convert.ToDouble(data["Fields.2381"]) + Convert.ToDouble(data["Fields.2383"]);
                line.add((investorFees * -1).ToString("C"));
                investorFeeTotal += investorFees;

                double appraisalFee = (Convert.ToDouble(data["Fields.641"]) );
                line.add((-1*appraisalFee).ToString("C")); //appraisal fee
                appraisalTotal += appraisalFee;

                double creditFee = (Convert.ToDouble(data["Fields.640"]));
                line.add((creditFee * -1).ToString("C")); //Credit fee
                creditTotal += creditFee;

                double interest = Convert.ToDouble(data["Fields.334"]);
                line.add((interest).ToString("C")); //interest
                interestTotal += interest;

                
                String purpose = data["Fields.19"].ToString();
                if (purpose.Contains("Refi"))
                {
                    double escrowFees = Convert.ToDouble(data["Fields.NEWHUD2.X11"]) +
                        Convert.ToDouble(data["Fields.NEWHUD.X808"]) + Convert.ToDouble(data["Fields.NEWHUD2.X14"]) +
                        Convert.ToDouble(data["Fields.NEWHUD.X810"]) + Convert.ToDouble(data["Fields.NEWHUD.X812"]) +
                        Convert.ToDouble(data["Fields.NEWHUD.X814"]) + Convert.ToDouble(data["Fields.NEWHUD.X816"]) +
                        Convert.ToDouble(data["Fields.NEWHUD.X818"])  ;
                    line.add(escrowFees.ToString("C"));
                    escrowTotal += escrowFees;

                    double titleFees =
                        Convert.ToDouble(data["Fields.NEWHUD.X215"]) + Convert.ToDouble(data["Fields.NEWHUD.X216"])  +
                    Convert.ToDouble(data["Fields.1763"]) + Convert.ToDouble(data["Fields.1768"]) +
                    Convert.ToDouble(data["Fields.NEWHUD.X639"]) +
                    Convert.ToDouble(data["Fields.1773"]) + Convert.ToDouble(data["Fields.1778"]) +
                    Convert.ToDouble(data["Fields.NEWHUD.X1604"]) + Convert.ToDouble(data["Fields.NEWHUD.X1612"]);
                    titleTotal += titleFees;

                    //Convert.ToDouble(data["Fields.NEWHUD.X572"]) owner's title
                    line.add((-1*titleFees).ToString("C"));

                    double recordingFee = Convert.ToDouble(data["Fields.NEWHUD.X607"]);
                    line.add((recordingFee*-1).ToString("C"));
                    recordingTotal += recordingFee;
                }
                else
                {
                    line.add("Purchase");
                    line.add("Purchase");
                    line.add("Purchase");
                }



                line.add(data["Fields.362"].ToString());
                line.add(data["Fields.317"].ToString());



                report.Add(line);
                Console.Out.Write("."); //status bar
            }

            //process last row
            Row totals = new Row();
            totals.setHeader(true);
            totals.add("");
            totals.add("");
            totals.add("");
            totals.add("");
            totals.add("");
            totals.add("");
            totals.add("");
            totals.add("Count");
            totals.add(count.ToString());
            totals.add("Total");
            totals.add(rebateTotal.ToString("C"));
            totals.add((-1 * investorIntTotal).ToString("C"));
            totals.add((-1 * investorFeeTotal).ToString("C"));
            totals.add((-1 * appraisalTotal).ToString("C"));
            totals.add((-1 * creditTotal).ToString("C"));
            totals.add((-1 * interestTotal).ToString("C"));
            totals.add(escrowTotal.ToString("C"));
            totals.add((-1 * titleTotal).ToString("C"));
            totals.add((-1 * recordingTotal).ToString("C"));
            totals.add("Grand Total");
            double sum = rebateTotal - investorIntTotal - investorFeeTotal
                - appraisalTotal - creditTotal + interestTotal + escrowTotal
                - titleTotal - recordingTotal;
            totals.add(sum.ToString("C"));

            report.Add(totals);

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
                foreach (Col col in row.getRow())
                {
                    if (row.isHeader())
                    {
                        text += "<th>" + col.toString() + "</th>";
                    }
                    else
                    {
                        if (col.isWarning())
                        {
                            text += "<td class='yellow'>" + col.toString() + "</td>";
                        }else if (col.isAlert())
                        {
                            text += "<td class='alert'>" + col.toString() + "</td>";
                        }
                        else
                        {
                            text += "<td>" + col.toString() + "</td>";
                        }
                    }
                }
                text += "</tr>";
            }
            text += "</table>";
            return text;
        }

    }
}
