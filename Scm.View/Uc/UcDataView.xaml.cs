using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcDataView.xaml 的交互逻辑
    /// </summary>
    public partial class UcDataView : UserControl
    {
        public UcDataView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// 显示更多搜索
        /// </summary>
        [Category("Behavior")]
        public event RoutedEventHandler SearchClick;

        /// <summary>
        /// 追加按钮事项
        /// </summary>
        public event RoutedEventHandler AppendClick;

        /// <summary>
        /// 启用禁用事项
        /// </summary>
        public event RoutedEventHandler ChangeSatusClick;

        /// <summary>
        /// 删除按钮事项
        /// </summary>
        public event RoutedEventHandler DeleteClick;

        private void BtAppend_Click(object sender, RoutedEventArgs e)
        {
            DrSide.IsOpen = true;
        }

        private void BtEnable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtDisable_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtDelete_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtSearch_Click(object sender, RoutedEventArgs e)
        {

        }

        private void BtMore_Click(object sender, RoutedEventArgs e)
        {
            if (SearchClick != null)
            {
                SearchClick(this, e);
            }
        }
    }
}