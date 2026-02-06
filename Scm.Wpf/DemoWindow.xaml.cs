using Com.Scm.Config;
using Com.Scm.Utils;
using CommunityToolkit.Mvvm.ComponentModel;
using CsvHelper;
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
            var config = AppSettings.Instance.GetSection<AbConfig>("Demo");

            Ocr(config);
        }

        private string AnmengDir = @"D:\Anmeng";
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
        private async void getToken()
        {
            var url = @"https://aip.baidubce.com/oauth/2.0/token";
            var body = new Dictionary<string, string>();
            body["grant_type"] = "client_credentials";
            body["client_id"] = "onW30VyPk69VFHjlqo9TgIZ1";
            body["client_secret"] = "55XovWjWC1emakGBHXR18Ejo4V33kzvZ";
            var result = await HttpUtils.PostFormStringAsync(url, body);
            _OAuth = result.AsJsonObject<BaiduOAuth>();
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
                    var result = await DownloadAsync(url, file);
                    if (!result)
                    {
                        LogUtils.Info("图片下载失败：" + image);
                        continue;
                    }

                    var find = await OcrBaidu(file, name, dvo);
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

        /// <summary>
        /// 文件下载
        /// </summary>
        /// <param name="url">下载地址</param>
        /// <param name="file">保存文件</param>
        /// <param name="append">是否支持续传</param>
        /// <returns></returns>
        public async Task<bool> DownloadAsync(string url, string file, bool append = false)
        {
            try
            {
                long existingLength = 0;

                // 设置 Range 请求头（断点续传）
                if (append)
                {
                    // 检查目标文件是否存在，获取已下载的字节数
                    if (File.Exists(file))
                    {
                        existingLength = new FileInfo(file).Length;
                    }

                    _HttpClient.DefaultRequestHeaders.Range = new System.Net.Http.Headers.RangeHeaderValue(existingLength, null);
                }

                // 发送 GET 请求
                var response = await _HttpClient.GetAsync(url, HttpCompletionOption.ResponseHeadersRead);

                // 确保响应成功
                response.EnsureSuccessStatusCode();

                // 获取总文件长度（从 Content-Length 头）
                var totalBytes = response.Content.Headers.ContentLength ?? 0;

                // 如果服务器不支持断点续传，重新开始下载
                if (existingLength > 0 && totalBytes <= existingLength)
                {
                    File.Delete(file);
                    existingLength = 0;
                }

                // 以流式方式写入文件（避免内存溢出）
                var mode = append ? FileMode.Append : FileMode.Create;
                using (FileStream fileStream = new FileStream(file, mode, FileAccess.Write, FileShare.None))
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

        private BaiduOAuth _OAuth;

        private async Task<bool> OcrBaidu(string file, string name, AbItem item)
        {
            LogUtils.Debug("文件：" + name);
            //var url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/general_basic?access_token=" + _OAuth.access_token;
            var url = @"https://aip.baidubce.com/rest/2.0/ocr/v1/accurate_basic?access_token=" + _OAuth.access_token;
            var body = new Dictionary<string, string>();
            body["url"] = file;
            var head = new Dictionary<string, string>();
            head["Content-Type"] = "application/x-www-form-urlencoded";
            var result = await HttpUtils.PostFormObjectAsync<BaiduOcrBase>(url, body, head);
            if (result == null || result.words_result == null)
            {
                return false;
            }

            var preRegex = new Regex(@"^AB\d{4}$");
            var endRegex = new Regex(@"^\d{7}$");
            bool endFind = false;
            var pre = "";
            var end = "";
            var all = "";
            foreach (var block in result.words_result)
            {
                var text = block.words.Trim();
                if (string.IsNullOrEmpty(text))
                {
                    continue;
                }

                if (preRegex.IsMatch(text))
                {
                    pre = text;
                    continue;
                }
                if (endRegex.IsMatch(text))
                {
                    endFind = true;
                    end = text;
                    continue;
                }
                var match = Regex.Match(text, @"(AB\d{11})");
                if (match.Success)
                {
                    all = match.Groups[1].Value.Trim();
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

    public class BaiduOAuth
    {
        public string refresh_token { get; set; }
        public string session_key { get; set; }
        public string access_token { get; set; }
        public string scope { get; set; }
        public string session_secret { get; set; }
        public long expires_in { get; set; }
    }

    public class BaiduOcrBase
    {
        public int direction { get; set; }
        public string log_id { get; set; }
        public int words_result_num { get; set; }

        public List<BaiduOcrWord> words_result { get; set; }
    }

    public class BaiduOcrWord
    {
        public string words { get; set; }
        public object probability { get; set; }
        public object paragraphs_result { get; set; }
        public object words_result_idx { get; set; }
        public object paragraphs_result_num { get; set; }
        public int language { get; set; }
    }
}