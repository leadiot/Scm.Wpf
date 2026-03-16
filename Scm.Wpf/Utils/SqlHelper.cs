using SqlSugar;

namespace Com.Scm.Utils
{
    public class SqlHelper
    {
        public static void Setup(string file)
        {
            var config = new ConnectionConfig
            {
                // 数据库类型
                DbType = DbType.Sqlite,
                // 连接字符串（包含连接池配置）
                ConnectionString = $"Data Source={file};",
                // 自动释放连接（关键：确保连接用完后归还给池）
                IsAutoCloseConnection = false
            };

            Instance = new SqlSugarClient(config);
            Instance.Aop.OnLogExecuting = (sql, pars) =>
            {
                foreach (var p in pars)
                {
                    sql = sql.Replace(p.ParameterName, "'" + p.Value + "'");
                }
                LogUtils.Sql(sql);
            };
            Instance.Open();
        }

        // 对外提供单例实例
        private static ISqlSugarClient Instance;

        public static ISqlSugarClient GetSqlClient()
        {
            return Instance;
        }

        public static void Close()
        {
            if (Instance != null)
            {
                Instance.Dispose();
                //Instance.Close();
            }
        }
    }
}
