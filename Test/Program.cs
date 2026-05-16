using Com.Scm.Utils;

internal class Program
{
    private static void Main(string[] args)
    {
        var url = "http://localhost:5000/api/NasSync/Dir";
        var header = new Dictionary<string, string>
        {
            ["appToken"] = "MjA1NTQ0NjM2OTg0MTMxOTkzNjoxNzc4ODkyMTUyMjE5OmI1YjU0Mjg1ZTE2YmRmNWE0ZTRlYjQ2ZmI3YjczMWYy"
        };
        var text = HttpUtils.GetString(url, null, header);
        Console.WriteLine(text);
    }
}
