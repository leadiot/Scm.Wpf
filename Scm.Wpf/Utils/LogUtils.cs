using NLog;

namespace Com.Scm.Utils
{
    internal class LogUtils
    {
        private static readonly ILogger logger = LogManager.GetCurrentClassLogger();

        public static void Setup()
        {
        }

        public static void Debug(string message)
        {
            logger.Debug(message);
        }

        public static void Debug(string method, string message)
        {
            logger.Debug($"【{method}】{message}");
        }

        public static void Debug(string method, string message, string arg)
        {
            logger.Debug($"【{method}】{message}：{arg}");
        }

        public static void Info(string message)
        {
            logger.Info(message);
        }

        public static void Info(string message, string arg)
        {
            logger.Info($"{message}：{arg}");
        }

        public static void Error(string message)
        {
            logger.Error(message);
        }

        public static void Error(Exception exp)
        {
            logger.Error(exp);
        }

        public static void Fatal(Exception exp)
        {
            logger.Fatal(exp);
        }

        public static void Sql(string message)
        {
            logger.Debug(message);
        }
    }
}
