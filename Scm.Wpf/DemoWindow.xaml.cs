using Com.Scm.Dao;
using System.Windows;
using System.Windows.Input;

namespace Com.Scm
{
    /// <summary>
    /// DemoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DemoWindow : HandyControl.Controls.Window
    {
        private DemoWindowDvo _Dvo;

        public DemoWindow()
        {
            InitializeComponent();

            var dbFile = "Data\\scm.db";
            SqlHelper.Setup(dbFile);

            _Dvo = new DemoWindowDvo();
            _Dvo.Init(null);

            this.DataContext = _Dvo;
        }

        public void Init(ScmWindow window)
        {
            _Dvo = new DemoWindowDvo();
            _Dvo.Init(window);

            this.DataContext = _Dvo;
        }

        private void LbFile_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void MiExplorer_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MiCreateDir_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MiCreateDoc_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MiEdit_Click(object sender, RoutedEventArgs e)
        {

        }

        private void MiDelete_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}