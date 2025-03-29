using Com.Scm.Http;
using Com.Scm.Utils;
using PaddleOCRSharp;

internal class Program
{
    private static void Main(string[] args)
    {
        Ocr();
    }

    public static async void Ocr()
    {
        var url = "http://1.94.139.166/prod-api/business/MarkingRecord/list?pageNum=1&pageSize=10&deptId=254";

        var request = new ScmHttpRequest();
        request.Url = url;
        var client = new ScmHttpClient(url);
        client.AddHeadParam("authorization", "Bearer eyJhbGciOiJIUzUxMiJ9.eyJsb2dpbl91c2VyX2tleSI6IjFlMGZkMjg0LWZhNjItNGVmNi04OThkLWJhNmJhNWJjNWU1NyJ9.VsD-3JS3tCCowrLdu55bc12uxY7xV9jC30ikazAOGSoDKNmNJ_5Dn1l98qhBTd1Hx46SiEaavetKX7Ge2Aodrg");
        var result = await client.GetTextAsync(request);

        string file = @"D:\20250326103001_20250326103025A742.jpg";

        OCRModelConfig config = null;
        OCRParameter oCRParameter = new OCRParameter();
        OCRResult ocrResult = new OCRResult();
        //建议程序全局初始化一次即可，不必每次识别都初始化，容易报错。
        PaddleOCREngine engine = new PaddleOCREngine(config, oCRParameter);
        {
            ocrResult = engine.DetectText(file);
        }
        if (ocrResult != null)
        {
            Console.WriteLine(ocrResult.Text, "识别结果");
        }
    }
}