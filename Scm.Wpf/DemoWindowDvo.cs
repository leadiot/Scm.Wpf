using Com.Scm.Dvo;
using Com.Scm.Enums;
using Com.Scm.Nas.Dao.Res;
using Com.Scm.Nas.Dvo;
using Com.Scm.Nas.Helper;
using Com.Scm.Nas.Sync;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Com.Scm
{
    public class DemoWindowDvo : ScmDvo
    {
        /// <summary>
        /// 驱动列表
        /// </summary>
        public ObservableCollection<NasResFileDvo> FileList { get; private set; } = new ObservableCollection<NasResFileDvo>();

        /// <summary>
        /// 本地路径
        /// </summary>
        private string nativePath;
        public string NativePath { get { return nativePath; } set { SetProperty(ref nativePath, value); } }

        private bool lastEnabled = false;
        public bool LastEnabled { get { return lastEnabled; } set { SetProperty(ref lastEnabled, value); } }

        /// <summary>
        /// 当前选中驱动
        /// </summary>
        private NasResFileDvo currentFile;
        public NasResFileDvo CurrentFile { get { return currentFile; } set { SetProperty(ref currentFile, value); } }

        public ICommand OpenFileCommand { get; }

        private Stack<NasResFileDvo> _Stack = new Stack<NasResFileDvo>();

        private int pageIndex = 0;
        private int pageCount = 20;

        private NasResFileDvo rootFile;

        private NasManager _NasManager = new NasManager();

        public long FolderId;

        private ScmWindow _Window;

        public DemoWindowDvo()
        {
            OpenFileCommand = new ScmCommand(OpenFile);
        }

        public void Init(ScmWindow window)
        {
            _Window = window;

            rootFile = new NasResFileDvo();
            rootFile.Id = 0;
            rootFile.Name = "";
            rootFile.Path = "";
        }

        private void ChangeNativePath()
        {
            NativePath = "";
            for (var i = _Stack.Count - 2; i >= 0; i -= 1)
            {
                var item = _Stack.ElementAt(i);
                NativePath += '/' + item.Name;
            }
        }

        /// <summary>
        /// 加载本地首页
        /// </summary>
        public async void LoadHome()
        {
            _Stack.Clear();
            _Stack.Push(rootFile);
            LastEnabled = false;

            pageIndex = 0;
            FileList.Clear();

            ChangeNativePath();

            await LoadNative(_Stack.Peek(), pageIndex);
        }

        /// <summary>
        /// 
        /// </summary>
        public async void LoadLast()
        {
            var item = _Stack.Pop();
            LastEnabled = _Stack.Count > 1;

            pageIndex = 0;
            FileList.Clear();

            ChangeNativePath();

            await LoadNative(_Stack.Peek(), pageIndex);
        }

        public async void Reload()
        {
            pageIndex = 0;
            FileList.Clear();

            ChangeNativePath();

            await LoadNative(_Stack.Peek(), pageIndex);
        }

        /// <summary>
        /// 切换本地目录
        /// </summary>
        /// <param name="dvo"></param>
        public async void Change(NasResFileDvo dvo)
        {
            if (dvo == null)
            {
                return;
            }

            _Stack.Push(dvo);
            LastEnabled = _Stack.Count > 1;

            pageIndex = 0;
            FileList.Clear();

            ChangeNativePath();

            await LoadNative(_Stack.Peek(), pageIndex);
        }

        /// <summary>
        /// 读取本地后续分页
        /// </summary>
        public async void LoadNextPage()
        {
            pageIndex++;
            await LoadNative(_Stack.Peek(), pageIndex);
        }

        private async Task LoadNative(NasResFileDvo parentDvo, int pageIndex)
        {
            if (parentDvo == null)
            {
                return;
            }

            pageIndex++;
            var daoList = _NasManager.ListResFileDaoByPage(FolderId, parentDvo.Id, "", pageIndex, pageCount);
            if (daoList.Count == 0)
            {
                FileList.Add(new NasResFileDvo { Name = "123", Type = ScmFileTypeEnum.Dir });
                //MessageBox.Show("没有更多数据了", "提示", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }
            foreach (var dao in daoList)
            {
                var dvo = NasResFileDvo.LoadFromDao(dao);
                FileList.Add(dvo);
            }
        }

        public string GetNativePath(NasResFileDvo dvo)
        {
            var folderDvo = NasHelper.GetFolder(dvo.FolderId);
            if (folderDvo == null)
            {
                return null;
            }

            return folderDvo.GetNativePath(CurrentFile.Path);
        }

        public string GetNativePath(NasResFileDao dao)
        {
            var folderDvo = NasHelper.GetFolder(dao.folder_id);
            if (folderDvo == null)
            {
                return null;
            }

            return folderDvo.GetNativePath(CurrentFile.Path);
        }

        public List<NasResFileDao> ListDoc(long dirId, ScmFileKindEnum kind)
        {
            return _NasManager.ListDocDaoByKind(dirId, kind);
        }

        private void OpenFile(object sender)
        {
            _Window.ShowAlert("OK");
        }
    }
}
