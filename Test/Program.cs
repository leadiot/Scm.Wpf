using Com.Scm.Utils;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var dict = new Dictionary<long, bool>();
        var i = 0;
        while (i < 40000)
        {
            i += 1;
            var id = NextId();
            if (dict.ContainsKey(id))
            {
                Console.WriteLine("ID重复！");
                continue;
            }
            dict[id] = false;
        }
        Console.WriteLine("OK");
    }

    private static long _LastTime = 0;
    private static long NextId()
    {
        var now = TimeUtils.GetUnixTime() * 1000;
        if (now >= _LastTime)
        {
            _LastTime = now;
        }
        _LastTime += 1;

        return _LastTime;
    }
}
