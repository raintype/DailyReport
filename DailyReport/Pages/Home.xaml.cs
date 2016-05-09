using DailyReport.Data;
using DailyReport.Entities;
using FirstFloor.ModernUI.Windows.Controls;
using System;
using System.Configuration;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Threading;

namespace DailyReport.Pages
{
    public partial class Home : UserControl
    {
        private ReportData reportData;
        private DispatcherTimer dispatcherTimer;
        private bool isChange;

        public Home()
        {
            InitializeComponent();
            reportData = new ReportData();

            datePicker.Text = DateTime.Now.ToString("yyyy-MM-dd");
            isChange = false;

            ((DailyReport.MainWindow)((System.Windows.Application.Current).Windows[0])).modernWindow.home = this;
            
            SetTextBoxDefaultTime(tbStartTime);
        }

        #region Event
        private void btnYesterday_Click(object sender, RoutedEventArgs e)
        {
            DatePickerAddDays(-1);
        }

        private void btnTommorrow_Click(object sender, RoutedEventArgs e)
        {
            DatePickerAddDays(+1);
        }

        private void btnOpenOutlook_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                SetTextBoxDefaultTime(tbEndTime);

                SaveData();

                Microsoft.Office.Interop.Outlook.Application oApp = new Microsoft.Office.Interop.Outlook.Application();
                Microsoft.Office.Interop.Outlook._MailItem oMailItem = (Microsoft.Office.Interop.Outlook._MailItem)oApp.CreateItem(Microsoft.Office.Interop.Outlook.OlItemType.olMailItem);

                string dept = ConfigurationManager.AppSettings["DeptName"];
                string userName = ConfigurationManager.AppSettings["UserName"];
                string sendDate = datePicker.SelectedDate.Value.ToString("yyyy/MM/dd").Replace('-', '/');
                
                oMailItem.To = ConfigurationManager.AppSettings["MailTo"];
                oMailItem.CC = ConfigurationManager.AppSettings["MailCc"];

                oMailItem.Subject = string.Format("[일일보고서] {0} {1} - {2}", dept, userName, sendDate);
                oMailItem.Body = "[요약]" + Environment.NewLine + tbSummary.Text + Environment.NewLine + Environment.NewLine
                                + "[출근] " + tbStartTime.Text + Environment.NewLine
                                + "[퇴근] " + tbEndTime.Text + Environment.NewLine
                                + Environment.NewLine + tbDetail.Text;

                oMailItem.Display(false);
            }
            catch
            {
                ModernDialog.ShowMessage("일일보고서를 불러오는 중 예외가 발생되었습니다.", "", MessageBoxButton.OK);
            }
        }

        private void datePicker_SelectedDateChanged(object sender, SelectionChangedEventArgs e)
        {
            DailyReportInfo dailyReportInfo = reportData.GetReportData(datePicker.SelectedDate);

            if (dailyReportInfo == null)
            {
                dailyReportInfo = new DailyReportInfo();
            }

            SetText(dailyReportInfo);

            StopAutoSave();
        }

        private void btnlately_Report_Click(object sender, RoutedEventArgs e)
        {
            DateTime? nowDate = DateTime.Now;

            for (int i = 0; i < 10; i++)
            {
                nowDate = nowDate.Value.AddDays(-1);

                DailyReportInfo dailyReportInfo = reportData.GetReportData(nowDate);

                if (dailyReportInfo != null)
                {
                    SetText(dailyReportInfo);
                    StopAutoSave();

                    return;
                }
            }
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {   
                SaveData();

                ModernDialog.ShowMessage("일일보고서가 저장 되었습니다.", "", MessageBoxButton.OK);
            }
            catch
            {
                ModernDialog.ShowMessage("일일보고서가 저장 중 예외가 발생되었습니다.", "", MessageBoxButton.OK);
            }
        }

        private void tbStartTime_GotFocus(object sender, RoutedEventArgs e)
        {
            SetTextBoxDefaultTime(tbStartTime);
        }

        private void tbEndTime_GotFocus(object sender, RoutedEventArgs e)
        {

            SetTextBoxDefaultTime(tbEndTime);
        }

        private void tbSummary_KeyDown(object sender, KeyEventArgs e)
        {
            StartAutoSave();
        }

        private void tbDetail_KeyDown(object sender, KeyEventArgs e)
        {
            StartAutoSave();
        }

        private void dispatcherTimer_Tick(object sender, EventArgs e)
        {
            if (!isChange)
            {
                return;
            }

            try
            {
                SaveData();
                tbNotice.Visibility = Visibility.Visible;
            }
            catch(Exception)
            {
            }

            
        }
        private void Canvas_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control && e.Key == Key.S)
            {
                try
                {
                    SaveData();

                    ModernDialog.ShowMessage("일일보고서가 저장 되었습니다.", "", MessageBoxButton.OK);
                }
                catch
                {
                    ModernDialog.ShowMessage("일일보고서가 저장 중 예외가 발생되었습니다.", "", MessageBoxButton.OK);
                }

                e.Handled = true;
            }
        }

        #endregion

        #region Private
        private void DatePickerAddDays(int days)
        {
            datePicker.Text = datePicker.SelectedDate.Value.AddDays(days).ToString("yyyy-MM-dd");
        }

        private void SetTextBoxDefaultTime(TextBox tbTime)
        {
            if (string.IsNullOrEmpty(tbTime.Text))
            {
                tbTime.Text = DateTime.Now.ToString("HH:mm");
            }
        }

        public void SaveData()
        {
            DailyReportInfo reportInfo = new DailyReportInfo
            {
                Summary = tbSummary.Text,
                Detail = tbDetail.Text,
                StartTime = tbStartTime.Text,
                EndTime = tbEndTime.Text,
                RegistDate = datePicker.SelectedDate.Value
            };

            reportData.SetReportDataData(reportInfo);

            StopAutoSave();
        }

        private void SetText(DailyReportInfo dailyReportInfo)
        {
            tbStartTime.Text = dailyReportInfo.StartTime;
            tbEndTime.Text = dailyReportInfo.EndTime;
            tbSummary.Text = dailyReportInfo.Summary;
            tbDetail.Text = dailyReportInfo.Detail;
        }

        private void StartAutoSave()
        {
            tbNotice.Visibility = Visibility.Hidden;

            if (isChange)
            {
                StopAutoSave();
            }

            int autoSaveMin = ConvertIntegerToString(ConfigurationManager.AppSettings["AutoSaveMin"]);
            if (autoSaveMin <= 0)
            {
                return;
            }

            isChange = true;

            dispatcherTimer = new System.Windows.Threading.DispatcherTimer();
            dispatcherTimer.Tick += new EventHandler(dispatcherTimer_Tick);
            dispatcherTimer.Interval = new TimeSpan(0, autoSaveMin, 0);
            dispatcherTimer.Start();
        }

        private int ConvertIntegerToString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return -1;
            }

            int resultValue;
            if (!int.TryParse(value, out resultValue))
            {
                return -2;
            }

            return resultValue;
        }

        private void StopAutoSave()
        {
            isChange = false;

            if (dispatcherTimer != null)
            {
                dispatcherTimer.Stop();
            }
        }
        #endregion
            }
}
