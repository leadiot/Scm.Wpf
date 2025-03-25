using System.Text.RegularExpressions;

internal class Program
{
    private static void Main(string[] args)
    {
        var uom = SizeUom.Parse("12");
        Console.WriteLine("Hello, World!");
    }

    public class SizeUom
    {
        /// <summary>
        /// 数值
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 未指定
        /// </summary>
        public bool IsNone { get; private set; }

        /// <summary>
        /// 填充
        /// </summary>
        public bool IsFill { get; private set; }

        /// <summary>
        /// 自动
        /// </summary>
        public bool IsAuto { get; private set; }

        public static SizeUom Parse(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return new SizeUom { IsNone = true };
            }

            value = value.Trim();
            if (value == "*")
            {
                return new SizeUom { IsFill = true };
            }

            var match = Regex.Match(value, @"(\d+)([^\d]*)");
            if (!match.Success)
            {
                return new SizeUom { IsNone = true };
            }

            var width = match.Groups[1].Value;
            var uom = match.Groups[2].Value.Trim();

            return new SizeUom { Width = double.Parse(width), Unit = uom };
        }
    }
}