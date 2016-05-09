using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReport.Entities
{
    public class WeeklyReportInfo
    {
        public DateTime RegistDate { get; set; }
        public string ThisWeek { get; set; }
        public string New { get; set; }
        public string Result { get; set; }
        public string NextWeek { get; set; }
        public string Comment { get; set; }
    }
}
