using Com.Scm.Views.Samples.Native;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Samples.Views.Native
{
    /// <summary>
    /// CustomView.xaml 的交互逻辑
    /// </summary>
    public partial class CustomView : UserControl
    {
        private EditView _EditControl;
        private EditViewDvo _EditDvo;

        public CustomView()
        {
            InitializeComponent();
        }

        #region 事件处理
        private void BtAppend_Click(object sender, RoutedEventArgs e)
        {
            if (_EditControl == null)
            {
                _EditControl = new EditView();
            }

            _EditDvo = new EditViewDvo();
            _EditControl.Init(_EditDvo);

            //PgGrid.ShowEdit(_EditControl, SaveData);
        }

        private bool SaveData()
        {
            _EditDvo.ValidateAllProperties();
            if (_EditDvo.HasErrors)
            {
                return false;
            }

            if (!_EditDvo.IsValid())
            {
                return false;
            }

            return true;
        }

        private void BtEnable_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var dvo in _Response.Items)
            //{
            //    if (dvo.IsChecked == true)
            //    {
            //        dvo.row_status = ScmRowStatusEnum.Enabled;
            //    }
            //}
        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {
            //foreach (var dvo in _Response.Items)
            //{
            //    if (dvo.IsChecked == true)
            //    {
            //        dvo.row_status = ScmRowStatusEnum.Disabled;
            //    }
            //}
        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {
        }

        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {
            //_Dvo.SearchAsync();
        }

        private void BtMore_Click(object sender, RoutedEventArgs e)
        {
            //if (_SearchView == null)
            //{
            //    _SearchView = new SearchView();
            //}

            //PgData.ShowSearch(_SearchView);
        }
        #endregion
    }
}
