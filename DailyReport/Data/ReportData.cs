using DailyReport.Entities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DailyReport.Data
{
    public class ReportData
    {
        private const string BASE_DIR = "Data";

        public DailyReportInfo GetReportData(DateTime? date)
        {
            try
            {
                string path = GetPath(date.Value);
                string file = GetFilePath(path, date.Value);

                if (File.Exists(file))
                {
                    string jsonData = File.ReadAllText(file);

                    DailyReportInfo dailyReportInfo = JsonConvert.DeserializeObject<DailyReportInfo>(jsonData);

                    return dailyReportInfo;
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

        public void SetReportDataData(DailyReportInfo reportInfo)
        {
            string path = GetPath(reportInfo.RegistDate);

            if (Directory.Exists(path) == false)
            {
                Directory.CreateDirectory(path);
            }

            string jsonResult = JsonConvert.SerializeObject(reportInfo);

            System.IO.File.WriteAllText(GetFilePath(path, reportInfo.RegistDate), jsonResult);
        }

        private string GetPath(DateTime date)
        {
            string path = string.Format(".\\{0}\\{1}\\{2}\\", BASE_DIR, date.Year, date.Month);

            return path;
        }

        private string GetFilePath(string path, DateTime date)
        {
            string filePath = string.Format("{0}{1}.txt", path, date.ToString("yyyyMMdd"));

            return filePath;
        }
    }
}
