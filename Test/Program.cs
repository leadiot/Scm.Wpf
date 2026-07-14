using Com.Scm.Utils;
using Test;

internal class Program
{
    private static async Task Main(string[] args)
    {
        string inputText = "4.30 复制打开抖音，看看【中式养生之道的作品】每天多听听 释放多巴胺 # 五音疗愈# 养心曲推荐... https://v.douyin.com/O-LRr3kA1Dg/ :7pm SlP:/ 05/04 g@b.nQ";

        try
        {
            string videoUrl = await DouyinParser.ParseNoWatermarkVideoUrlAsync(inputText);
            Console.WriteLine("解析成功！无水印视频地址：");
            Console.WriteLine(videoUrl);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"解析失败：{ex.Message}");
        }
    }
}
