using SqlSugar;

namespace Com.Scm.Views.Tasks
{
    public abstract class ATask
    {
        /// <summary>
        /// 任务名称
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// 使用说明
        /// 包含参数配置，输出结果等信息。
        /// </summary>
        public abstract string Help { get; }

        /// <summary>
        /// 任务方法
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// 连接数据库
        /// </summary>
        /// <param name="type">数据库类型</param>
        /// <param name="path">数据库路径</param>
        /// <param name="user">数据库用户</param>
        /// <param name="pass">数据库密码</param>
        /// <returns></returns>
        protected SqlSugarClient GetClient(DbType type, string path, string user = null, string pass = null)
        {
            var ConnectionString = "";
            switch (type)
            {
                case DbType.Sqlite:
                    ConnectionString = $"Data Source={path}";
                    break;
                default:
                    ConnectionString = "";
                    break;
            }

            var db = new SqlSugarClient(new ConnectionConfig()
            {
                ConnectionString = ConnectionString, // SQLite 连接字符串
                DbType = type, // 指定数据库类型为 SQLite
                IsAutoCloseConnection = true, // 自动关闭连接
                InitKeyType = InitKeyType.Attribute//从实体特性中读取主键自增列信息
            });

            return db;
        }
    }
}
