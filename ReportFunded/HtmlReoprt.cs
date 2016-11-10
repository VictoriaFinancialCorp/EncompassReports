using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportFunded
{
    public class HtmlReport{

        public static String getHeader()
        {
            String header = "<html><head><style>";
            List<String> css = new List<String> {
               "table,th,td{text-align:center;border:1px solid grey;border-collapse:collapse;padding:.5em;font-size:.9em;}",
               "table{border:2px solid grey}",
               ".yellow{background:#FFFCA6;}",
               ".small{font-size:.7em;}"
            };
            foreach(String s in css){
                header += s;
            }
            header += "</style></head>";
            return header;
        }


        public static String getFooter(DateTime timestamp)
        {
            String footer = "";
            footer += "<div class='small'>*Data Sources from Encompass. If report is incorrect, please update information in Encompass.* </div>";
            footer += "<div class='small'>Report completed in: " + DateTime.Now.Subtract(timestamp).ToString(@"ss\.fff") + " seconds</div> </body></html>";
            return footer;
        }
    }


}
