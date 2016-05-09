using DailyReport.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReport.Data
{
    public class WeeklyReportData
    {
        private const string BASE_DIR = "Data";

        public WeeklyReportInfo GetReportData(DateTime date)
        {
            try
            {
                string path = GetPath(date);
                string file = GetFilePath(path, date);

                if (File.Exists(file))
                {
                    string jsonData = File.ReadAllText(file);

                    WeeklyReportInfo weeklyReportInfo = JsonConvert.DeserializeObject<WeeklyReportInfo>(jsonData);

                    return weeklyReportInfo;
                }
                else
                {
                    return null;
                }
            }
            catch
            {
                return null;
            }
        }

        public void SetReportDataData(WeeklyReportInfo reportInfo)
        {
            string path = GetPath(reportInfo.RegistDate);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string jsonResult = JsonConvert.SerializeObject(reportInfo);

            System.IO.File.WriteAllText(GetFilePath(path, reportInfo.RegistDate), jsonResult);
        }

        private int WeeksOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        private string GetPath(DateTime date)
        {
            string path = string.Format(".\\{0}\\{1}\\{2}\\", BASE_DIR, date.Year, date.Month);

            return path;
        }
        private string getWeekString(DateTime date)
        {
            int month = date.Month;
            int weeksOfMonth = WeeksOfMonth(date);

            return string.Format("{0}_{1}", month.ToString(), weeksOfMonth.ToString());
        }


        private string GetFilePath(string path, DateTime date)
        {


            string filePath = string.Format("{0}{1}.txt", path, getWeekString(date));

            return filePath;
        }
    }
}
