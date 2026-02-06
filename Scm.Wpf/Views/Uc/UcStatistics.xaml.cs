using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Com.Scm.Wpf.Views.Uc
{
    /// <summary>
    /// UcStatistics.xaml 的交互逻辑
    /// </summary>
    public partial class UcStatistics : UserControl
    {
        public UcStatistics()
        {
            InitializeComponent();
        }

        public string Title { get; set; }

        public string Value { get; set; }
    }
}
