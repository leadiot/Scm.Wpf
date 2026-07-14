using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Test
{
    public class DouyinParser
    {
        private static readonly HttpClient _httpClient = new HttpClient();

        /// <summary>
        /// 解析抖音分享文本，获取无水印视频地址
        /// </summary>
        /// <param name="shareText">包含抖音链接的分享文本</param>
        /// <returns>无水印的视频播放地址</returns>
        public static async Task<string> ParseNoWatermarkVideoUrlAsync(string shareText)
        {
            // 1. 使用正则表达式提取短链接
            string shortUrl = ExtractShortUrl(shareText);
            if (string.IsNullOrEmpty(shortUrl))
            {
                throw new Exception("未能从文本中提取到有效的抖音短链接。");
            }

            // 2. 获取重定向后的真实链接
            string realUrl = await GetRealUrlAsync(shortUrl);
            if (string.IsNullOrEmpty(realUrl))
            {
                throw new Exception("获取真实链接失败。");
            }

            // 3. 从真实链接中提取 video_id
            string videoId = ExtractVideoId(realUrl);
            if (string.IsNullOrEmpty(videoId))
            {
                throw new Exception("未能从真实链接中提取到 video_id。");
            }

            // 4. 构造无水印视频播放地址
            // 注意：抖音的接口可能会变动，此地址为常见的无水印格式
            string noWatermarkUrl = $"https://www.iesdouyin.com/aweme/v1/play/?video_id={videoId}&ratio=720p&line=0";

            return noWatermarkUrl;
        }

        // 提取短链接
        private static string ExtractShortUrl(string text)
        {
            // 匹配 https://v.douyin.com/ 后面跟随的字符，直到遇到空格或结束
            var match = Regex.Match(text, @"https://v\.douyin\.com/[^ ]+");
            return match.Success ? match.Value.Trim() : null;
        }

        // 获取重定向后的真实URL
        private static async Task<string> GetRealUrlAsync(string shortUrl)
        {
            // 创建一个不自动重定向的请求
            var handler = new HttpClientHandler() { AllowAutoRedirect = false };
            using (var client = new HttpClient(handler))
            {
                // 模拟浏览器请求头，防止被反爬
                client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/91.0.4472.124 Safari/537.36");

                try
                {
                    var response = await client.GetAsync(shortUrl);
                    if (response.Headers.Location != null)
                    {
                        // 返回重定向的地址
                        return response.Headers.Location.ToString();
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"请求出错: {ex.Message}");
                }
            }
            return null;
        }

        // 从真实链接中提取 video_id
        private static string ExtractVideoId(string url)
        {
            // 真实链接格式通常为 .../share/video/{video_id}/...
            var match = Regex.Match(url, @"/share/video/(\d+)/");
            return match.Success ? match.Groups[1].Value : null;
        }
    }
}
