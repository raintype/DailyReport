using DailyReport.Data;
using DailyReport.Entities;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Configuration;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace DailyReport.Pages
{
    /// <summary>
    /// Interaction logic for Settings.xaml
    /// </summary>
    public partial class Weekly : UserControl
    {
        private WeeklyReportData reportData;

        private DateTime now;

        public Weekly()
        {
            InitializeComponent();

            reportData = new WeeklyReportData();

            // 주간보고 일이 금요일임으로 DayWeek는 금요일 기준으로 한다.
            now = DateTime.Now;

            int dayWeek = (int)now.DayOfWeek;
            int fridayDayWeek = (int)DayOfWeek.Friday;

            if (dayWeek < fridayDayWeek)
            {
                now = now.AddDays(dayWeek % fridayDayWeek);
            }


            txtWeek.Text = getWeekString(now);



            LoadDate(now);
        }
        
        private int WeeksOfMonth(DateTime date)
        {
            DateTime beginningOfMonth = new DateTime(date.Year, date.Month, 1);

            while (date.Date.AddDays(1).DayOfWeek != CultureInfo.CurrentCulture.DateTimeFormat.FirstDayOfWeek)
                date = date.AddDays(1);

            return (int)Math.Truncate((double)date.Subtract(beginningOfMonth).TotalDays / 7f) + 1;
        }

        private string getWeekString(DateTime date)
        {
            int month = date.Month;
            int weeksOfMonth = WeeksOfMonth(date);

            return string.Format("{0}월 {1}주", month.ToString(), weeksOfMonth.ToString());
        }

        public void SaveData()
        {
            WeeklyReportInfo reportInfo = new WeeklyReportInfo
            {
                RegistDate = now,
                ThisWeek = tbThisWeek.Text,
                New = tbNew.Text,
                Result = tbResult.Text,
                NextWeek = tbNextWeek.Text,
                Comment = tbComment.Text
            };


            reportData.SetReportDataData(reportInfo);
        }

        private void SetText(WeeklyReportInfo reportInfo)
        {
            tbThisWeek.Text = reportInfo.ThisWeek;
            tbNew.Text = reportInfo.New;
            tbResult.Text = reportInfo.Result;
            tbNextWeek.Text = reportInfo.NextWeek;
            tbComment.Text = reportInfo.Comment;
        }


        public void LoadDate(DateTime date)
        {
            WeeklyReportInfo reportInfo = reportData.GetReportData(date);

            if (reportInfo == null)
            {
                reportInfo = new WeeklyReportInfo();

                // 이전 주 주간 목표 불러와서 넣기
                WeeklyReportInfo preWeekreportInfo = reportData.GetReportData(date.AddDays(-7));

                if (preWeekreportInfo != null)
                {
                    reportInfo.ThisWeek = preWeekreportInfo.NextWeek;
                }
            }

            SetText(reportInfo);
        }


        private void btnPreWeek_Click(object sender, RoutedEventArgs e)
        {
            now = now.AddDays(-7);
            txtWeek.Text = getWeekString(now);

            LoadDate(now);
        }

        private void btnNextWeek_Click(object sender, RoutedEventArgs e)
        {
            now = now.AddDays(7);
            txtWeek.Text = getWeekString(now);

            LoadDate(now);
        }

        private void btnOpenOutlook_Click(object sender, RoutedEventArgs e)
        {
            try
            {

                SaveData();

                Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook._MailItem oMailItem = (Microsoft.Office.Interop.Outlook._MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

                string dept = ConfigurationManager.AppSettings["DeptName"];
                string userName = ConfigurationManager.AppSettings["UserName"];
                
                oMailItem.To = ConfigurationManager.AppSettings["MailTo"];
                oMailItem.CC = ConfigurationManager.AppSettings["MailCc"];

                oMailItem.Subject = string.Format("[주간보고서] {0} {1}, {2} 업무보고", dept, userName,  txtWeek.Text);
                oMailItem.Body = "[" + txtThisWeek.Text + "]" + Environment.NewLine + tbThisWeek.Text + Environment.NewLine
                                + "[" + txtNew.Text + "]" + Environment.NewLine + tbNew.Text + Environment.NewLine
                                + "[" + txtResult.Text + "]" + Environment.NewLine + tbResult.Text + Environment.NewLine
                                + "[" + txtNextWeek.Text + "]" + Environment.NewLine + tbNextWeek.Text + Environment.NewLine
                                + "[" + txtComment.Text + "]" + Environment.NewLine + tbComment.Text + Environment.NewLine;
                                

                oMailItem.Display(false);
            }
            catch
            {
                ModernDialog.ShowMessage("보고서를 불러오는 중 예외가 발생되었습니다.", "", MessageBoxButton.OK);
            }

        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SaveData();

                ModernDialog.ShowMessage("보고서가 저장 되었습니다.", "", MessageBoxButton.OK);
            }
            catch
            {
                ModernDialog.ShowMessage("보고서가 저장 중 예외가 발생되었습니다.", "", MessageBoxButton.OK);
            }
        }

        private void Canvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                try
                {
                    SaveData();

                    ModernDialog.ShowMessage("보고서가 저장 되었습니다.", "", MessageBoxButton.OK);
                }
                catch
                {
                    ModernDialog.ShowMessage("보고서가 저장 중 예외가 발생되었습니다.", "", MessageBoxButton.OK);
                }

                e.Handled = true;
            }
        }

    }
}
