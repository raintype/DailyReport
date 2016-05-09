using System.Windows;
using System.Windows.Controls;
using FirstFloor.ModernUI.Windows.Controls;
using System.Configuration;

namespace DailyReport.Pages
{
    /// <summary>
    /// Setting.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class Settings : UserControl
    {
        public Settings()
        {
            InitializeComponent();

            tbDeptName.Text = ConfigurationManager.AppSettings["DeptName"];
            tbUserName.Text = ConfigurationManager.AppSettings["UserName"];
            tbMailTo.Text = ConfigurationManager.AppSettings["MailTo"];
            tbMailCc.Text = ConfigurationManager.AppSettings["MailCc"];

            string autoSaveMinString = ConfigurationManager.AppSettings["AutoSaveMin"];
            int autoSaveMin = 0;
            if (!string.IsNullOrEmpty(autoSaveMinString))
            {
                if (!int.TryParse(autoSaveMinString, out autoSaveMin))
                {
                    autoSaveMin = 0;
                }
            }

            tbAutoSaveMin.Text = autoSaveMin.ToString();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Configuration config = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);

                config.AppSettings.Settings["DeptName"].Value = tbDeptName.Text;
                config.AppSettings.Settings["UserName"].Value = tbUserName.Text;
                config.AppSettings.Settings["MailTo"].Value = tbMailTo.Text;
                config.AppSettings.Settings["MailCc"].Value = tbMailCc.Text;
                                
                if (ConvertIntegerToString(tbAutoSaveMin.Text) < 0)
                {
                    tbAutoSaveMin.Text = "0";
                }

                if (string.IsNullOrEmpty(ConfigurationManager.AppSettings["AutoSaveMin"]))
                {
                    config.AppSettings.Settings.Remove("AutoSaveMin");
                    config.AppSettings.Settings.Add("AutoSaveMin", tbAutoSaveMin.Text);
                }
                else
                {
                    config.AppSettings.Settings["AutoSaveMin"].Value = tbAutoSaveMin.Text;
                }

                config.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection("appSettings");

                ModernDialog.ShowMessage("설정이 정상적으로 변경 되었습니다.", "", MessageBoxButton.OK);
            }
            catch
            {
                ModernDialog.ShowMessage("설정 변경 중 예외가 발생되었습니다.", "", MessageBoxButton.OK);
            }
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
    }
}
