﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ReportFunded
{
    public class Row
    {
        List<String> cols;
        Boolean header;
        Boolean warning;
        public Row()
        {
            this.cols = new List<String>();
            this.header = false;
            this.warning = false;
        }

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
}
