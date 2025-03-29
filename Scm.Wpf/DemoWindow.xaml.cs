using Com.Scm.Utils;
using Com.Scm.Wpf.Config;
using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
using PaddleOCRSharp;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Windows;

namespace Com.Scm.Wpf
{
    /// <summary>
    /// DemoWindow.xaml 的交互逻辑
    /// </summary>
    public partial class DemoWindow : HandyControl.Controls.Window
    {
        private AbInfo _Dvo = new AbInfo();

        public DemoWindow()
        {
            InitializeComponent();

            this.DataContext = _Dvo;
        }

        private void BtDemo_Click(object sender, RoutedEventArgs e)
        {
            if (!AppSettings.Load())
            {
                MessageBox.Show("配置文件加载失败！");
                return;
            }
            var config = AppSettings.GetSection<AbConfig>("Demo");

            Ocr(config);
        }

        private string AnmengDir = @"D:\Anmeng";
        private string AnmengSuccessDir = @"";
        private string AnmengFailureDir = @"";
        private HttpClient _HttpClient;
        private CsvWriter _Writer;

        private async void Ocr(AbConfig appConfig)
        {
            _Dvo.Message = "开始耳标识别…";
            LogUtils.Setup();

            if (!string.IsNullOrEmpty(appConfig.output))
            {
                AnmengDir = appConfig.output;
            }
            if (!Directory.Exists(AnmengDir))
            {
                Directory.CreateDirectory(AnmengDir);
            }

            var body = new Dictionary<string, string>();
            var head = new Dictionary<string, string>()
            {
                ["authorization"] = appConfig.token
            };

            OCRModelConfig config = null;
            OCRParameter oCRParameter = new OCRParameter();
            _Engine = new PaddleOCREngine(config, oCRParameter);

            _HttpClient = new HttpClient();

            var csv = Path.Combine(AnmengDir, "anmeng.csv");
            if (File.Exists(csv))
            {
                File.Delete(csv);
            }

            var stream = new StreamWriter(csv);
            _Writer = new CsvWriter(stream, CultureInfo.InvariantCulture);
            for (var i = appConfig.from; i <= appConfig.to; i += 1)
            {
                _Dvo.Message = $"正在读取第{i}页";
                await OcrPage(body, head, i);

                _Writer.Flush();
                stream.Flush();

                _Dvo.Items.Clear();
            }

            _Writer.Flush();
            _Writer.Dispose();

            stream.Flush();
            stream.Dispose();

            _Dvo.Message = "耳标识别完成！";
        }

        private async Task<bool> OcrPage(Dictionary<string, string> body, Dictionary<string, string> head, int page)
        {
            var url = $"http://1.94.139.166/prod-api/business/MarkingRecord/list?pageNum={page}&pageSize=10&deptId=254";
            var result = await HttpUtils.GetStringAsync(url, body, head);
            if (string.IsNullOrWhiteSpace(result))
            {
                return false;
            }

            var response = result.AsJsonObject<BaseResponse>();
            foreach (var row in response.rows)
            {
                await LoadImages(row);
            }
            return true;
        }

        private async Task<bool> LoadImages(AbData row)
        {
            _Dvo.Message = "正在处理耳标：" + row.tagNum;

            var dir = Path.Combine(AnmengDir, row.tagNum);
            if (Directory.Exists(dir))
            {
                Directory.Delete(dir, true);
            }
            Directory.CreateDirectory(dir);
            var doneDir = Path.Combine(dir, "Done");
            Directory.CreateDirectory(doneDir);
            var noneDir = Path.Combine(dir, "None");
            Directory.CreateDirectory(noneDir);

            var dvo = new AbItem();
            dvo.Tag = row.tagNum;
            _Dvo.Items.Add(dvo);

            var images = row.GetImages();
            if (images != null)
            {
                foreach (var image in images)
                {
                    var idx = image.LastIndexOf('/');
                    if (idx < 0)
                    {
                        LogUtils.Info("无效的图片路径：" + image);
                        continue;
                    }

                    var name = image.Substring(idx + 1);
                    var file = Path.Combine(dir, name);
                    var url = "http://1.94.139.166/prod-api" + image;
                    var result = await LoadImage(url, file);
                    if (!result)
                    {
                        LogUtils.Info("图片下载失败：" + image);
                        continue;
                    }

                    var find = OcrImage(file, name, dvo);
                    if (!find)
                    {
                        File.Move(file, Path.Combine(noneDir, name));
                        continue;
                    }

                    dvo.File = name;

                    LogUtils.Info("耳标识别成功：" + image);
                    if (row.tagNum.EndsWith(dvo.OcrEnd))
                    {
                        dvo.Match = true;

                        dvo.Message = "匹配成功";
                        _Dvo.Message = "耳标匹配成功：" + row.tagNum;
                        //File.Move(file, Path.Combine(doneDir, name));
                        break;
                    }
                    else
                    {
                        dvo.Message = "匹配失败";
                        _Dvo.Message = "耳标匹配失败：" + row.tagNum;
                    }
                }
            }

            LogUtils.Info("耳标匹配：" + dvo.ToJsonString(), "db");
            _Writer.WriteRecord(dvo);
            _Writer.NextRecord();
            return true;
        }

        public async Task<bool> LoadImage(string url, string file)
        {
            try
            {
                // 检查目标文件是否存在，获取已下载的字节数
                long existingLength = 0;
                if (File.Exists(file))
                {
                    existingLength = new FileInfo(file).Length;
                }

                // 设置 Range 请求头（断点续传）
                _HttpClient.DefaultRequestHeaders.Range =
                    new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);

                // 发送 GET 请求
                HttpResponseMessage response = await _HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                // 确保响应成功
                response.EnsureSuccessStatusCode();

                // 获取总文件长度（从 Content-Length 头）
                long totalBytes = response.Content.Headers.ContentLength ?? 0;

                // 如果服务器不支持断点续传，重新开始下载
                if (existingLength > 0 && totalBytes <= existingLength)
                {
                    File.Delete(file);
                    existingLength = 0;
                }

                // 以流式方式写入文件（避免内存溢出）
                using (FileStream fileStream = new FileStream(
                    file,
                    FileMode.Append, // 断点续传用追加模式
                    FileAccess.Write,
                    FileShare.None))
                {
                    await response.Content.CopyToAsync(fileStream);
                }

                LogUtils.Info($"下载完成！文件大小：{totalBytes} 字节");
                return true;
            }
            catch (HttpRequestException ex)
            {
                LogUtils.Error($"请求错误: {ex.Message}");
                return false;
            }
            catch (IOException ex)
            {
                LogUtils.Error($"文件操作错误: {ex.Message}");
                return false;
            }
        }

        private PaddleOCREngine _Engine;

        private bool OcrImage(string file, string name, AbItem item)
        {
            LogUtils.Debug("文件：" + name);
            _Dvo.Message = "正在识别文件：" + name;
            var result = _Engine.DetectText(file);
            if (result == null || result.TextBlocks == null)
            {
                return false;
            }

            var preRegex = new Regex(@"^AB\d{4}$");
            var endRegex = new Regex(@"^\d{7}$");
            bool endFind = false;
            var pre = "";
            var end = "";
            var all = "";
            foreach (var block in result.TextBlocks)
            {
                var text = block.Text.Trim();
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                if (preRegex.IsMatch(text))
                {
                    pre = block.Text;
                    continue;
                }
                if (endRegex.IsMatch(text))
                {
                    endFind = true;
                    end = block.Text;
                    continue;
                }
                var match = Regex.Match(text, @"(AB\d{11})");
                if (match.Success)
                {
                    all = match.Groups[1].Value;
                    continue;
                }
            }

            item.OcrPre = pre;
            item.OcrEnd = end;
            item.Ocr = all;
            item.Find = endFind;

            LogUtils.Info("未找到标签数据！");
            return endFind;
        }
    }

    public class BaseResponse
    {
        public int total { get; set; }

        public List<AbData> rows { get; set; }
    }

    public class AbData
    {
        public string id { get; set; }
        public string idCard { get; set; }
        public string farmerId { get; set; }
        public string farmerName { get; set; }
        public string createBy { get; set; }
        public string otherPhoto { get; set; }
        public long deptId { get; set; }

        public string markingName { get; set; }
        public string tagNum { get; set; }

        public string[] GetImages()
        {
            if (string.IsNullOrWhiteSpace(otherPhoto))
            {
                return null;
            }
            return otherPhoto.Split(',');
        }
    }


    public partial class AbInfo : Com.Scm.Wpf.Dvo.ScmDvo
    {
        [ObservableProperty]
        private ObservableCollection<AbItem> items = new ObservableCollection<AbItem>();

        [ObservableProperty]
        private string message;
    }

    public partial class AbItem : Com.Scm.Wpf.Dvo.ScmDvo
    {
        [ObservableProperty]
        private string tag;

        [ObservableProperty]
        private bool find;

        [ObservableProperty]
        private bool match;

        [ObservableProperty]
        private string ocrPre;
        [ObservableProperty]
        private string ocrEnd;
        [ObservableProperty]
        private string ocr;

        [ObservableProperty]
        private string file;

        [ObservableProperty]
        private string message;
    }

    public class AbConfig
    {
        public int from { get; set; }
        public int to { get; set; }

        public string token { get; set; }

        public string output { get; set; }
    }
}