using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportFunded
{
    public class Row
    {
        List<Col> cols;
        Boolean header;
        Boolean warning;
        public Row()
        {
            this.cols = new List<Col>();
            this.header = false;
            this.warning = false;
        }

        public void add(String col)
        {
            this.cols.Add(new ReportFunded.Col(col));
        }
        public void add(String col, Boolean warn)
        {
            Col c = new ReportFunded.Col(col);
            if (warn)
            {
                c.setWarn(warn);
            }
            this.cols.Add(c);
        }
        public List<Col> getRow()
        {
            return cols;
        }
        public String toString()
        {
            return this.cols.ToString();
        }
        public void setHeader(Boolean isHeader)
        {
            this.header = isHeader;
        }
        public Boolean isHeader()
        {
            return this.header;
        }
        public void setWarn(Boolean warn)
        {
            this.warning = warn;
        }
        public Boolean isWarning()
        {
            return this.warning;
        }


    };
    public class Col{
        Boolean warning;
        String text;
        public Col(String text){
            this.warning = false;
            this.text = text;
        }
        public void add(String text)
        {
            this.text = text;
        }
        public String get()
        {
            return this.text;
        }
        public String toString()
        {
            return this.text;
        }
        public void setWarn(Boolean warn)
        {
            this.warning = warn;
        }
        public Boolean isWarning()
        {
            return this.warning;
        }

    }
}
