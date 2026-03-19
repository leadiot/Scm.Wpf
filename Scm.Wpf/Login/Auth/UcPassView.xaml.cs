using Com.Scm.Views;
using System.Windows.Controls;

namespace Com.Scm.Login.Auth
{
    /// <summary>
    /// 验证码登录
    /// </summary>
    public partial class UcPassView : UserControl
    {
        /// <summary>
        /// 父窗体
        /// </summary>
        private OperatorWindow _Owner;

        private ScmOperator _ScmOperator;

        private UcPassViewDvo _Dvo;

        public UcPassView()
        {
            InitializeComponent();
        }

        public void Init(OperatorWindow owner, ScmOperator scmOperator)
        {
            _Owner = owner;
            _ScmOperator = scmOperator;

            _Dvo = new UcPassViewDvo();
            _Dvo.ChangeVCode(_ScmOperator.GetApiUrl(""));
            this.DataContext = _Dvo;
        }

        private void BtVcode_PreviewMouseUp(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            if (e.ChangedButton != System.Windows.Input.MouseButton.Left)
            {
                return;
            }

            _Dvo.ChangeVCode(_ScmOperator.GetApiUrl(""));
        }

        private void BtVerify_Click(object sender, System.Windows.RoutedEventArgs e)
        {
            DoAuthAsync();
        }

        private async void DoAuthAsync()
        {
            _Dvo.Pass = TbPass.Password;

            var body = _Dvo.GetLogin();
            if (body == null)
            {
                return;
            }

            var result = await _ScmOperator.SignInAsync(body);
            if (!result)
            {
                MessageWindow.ShowDialog(_Owner, _ScmOperator.ErrorMessage);
                return;
            }

            await _Owner.ShowMain(_ScmOperator);
        }
    }
}
