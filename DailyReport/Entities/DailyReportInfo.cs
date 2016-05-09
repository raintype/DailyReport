using System;

namespace DailyReport.Entities
{
    public class DailyReportInfo
    {
        public DateTime RegistDate { get; set; }
        public string StartTime { get; set; }
        public string EndTime { get; set; }
        public string Summary { get; set; }
        public string Detail { get; set; }
    }
}
