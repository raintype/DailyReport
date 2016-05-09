using DailyReport.Pages;
using FirstFloor.ModernUI.Windows.Controls;

namespace DailyReport
{
    /// <summary>
    /// MainWindow.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class MainWindow : ModernWindow
    {
        public Home home;
        public MainWindow()
        {
            InitializeComponent();

            this.Closing += new System.ComponentModel.CancelEventHandler(Window4_Closing);
        }

        //
        // 요약:
        //     System.Windows.Window.Closing 이벤트를 발생시킵니다.
        //
        // 매개 변수:
        //   e:
        //     이벤트 데이터가 들어 있는 System.ComponentModel.CancelEventArgs입니다.
        void Window4_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            home.SaveData();

            //e.Cancel = true;
        }
    }
}
