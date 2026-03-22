using Com.Scm.Utils;
using SqlSugar;
using System.IO;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Com.Scm.Dao
{
    public class DbHelper
    {
        private const int MAJOR = 3;
        private const int MINOR = 0;
        private const int PATCH = 0;
        private const string BUILD = "2026020601";
        private const string RELEASE_DATE = "2026-02-06";

        protected ISqlSugarClient _SqlClient;
        protected string _BaseDir;

        public void Init(ISqlSugarClient sqlClient, string baseDir)
        {
            _SqlClient = sqlClient;
            _BaseDir = baseDir;
        }

        /// <summary>
        /// 清空数据库
        /// </summary>
        /// <param name="baseDir"></param>
        /// <returns></returns>
        public virtual bool DropDb()
        {
            return DropTable(Assembly.GetExecutingAssembly());
        }

        /// <summary>
        /// 重建数据库
        /// </summary>
        /// <param name="baseDir"></param>
        /// <returns></returns>
        public virtual bool InitDb()
        {
            var key = "nas.net";

            var verDao = ReadDbVer(key);
            if (verDao == null)
            {
                verDao = new ScmVerDao();
                verDao.key = key;
                verDao.create_time = TimeUtils.GetUnixTime();
            }

            InitTable(Assembly.GetExecutingAssembly());

            InitDml(verDao);

            var ddlFile = Path.Combine(_BaseDir, "ddl.sql");
            ExecuteSql(ddlFile, verDao.major);

            var dmlFile = Path.Combine(_BaseDir, "dml.sql");
            ExecuteSql(dmlFile, verDao.major);

            verDao.major = MAJOR;
            verDao.minor = MINOR;
            verDao.patch = PATCH;
            verDao.build = BUILD;
            verDao.release_date = RELEASE_DATE;
            verDao.update_time = TimeUtils.GetUnixTime();
            SaveDbVer(verDao);
            return true;
        }

        /// <summary>
        /// 读取数据库版本
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        protected ScmVerDao ReadDbVer(string key)
        {
            try
            {
                _SqlClient.CodeFirst.InitTables(typeof(ScmVerDao));

                return _SqlClient.Queryable<ScmVerDao>().First(a => a.key == key);
            }
            catch (Exception ex)
            {
                return null;
            }
        }

        /// <summary>
        /// 保存数据库版本
        /// </summary>
        /// <param name="verDao"></param>
        protected void SaveDbVer(ScmVerDao verDao)
        {
            if (verDao.id == 0)
            {
                verDao.PrepareCreate();
                _SqlClient.Insertable(verDao).ExecuteCommand();
            }
            else
            {
                verDao.PrepareUpdate();
                _SqlClient.Updateable(verDao).ExecuteCommand();
            }
        }

        /// <summary>
        /// 新增记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        protected void InsertDao<T>(T dao) where T : ScmDao, new()
        {
            dao.PrepareCreate();
            _SqlClient.Insertable(dao).ExecuteCommand();
        }

        /// <summary>
        /// 更新记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        protected void UpdateDao<T>(T dao) where T : ScmDao, new()
        {
            dao.PrepareUpdate();
            _SqlClient.Updateable(dao).ExecuteCommand();
        }

        /// <summary>
        /// 删除记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        protected void DeleteDao<T>(T dao) where T : ScmDao, new()
        {
            _SqlClient.Deleteable(dao).ExecuteCommand();
        }

        /// <summary>
        /// 保存记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        protected void SaveDao<T>(T dao) where T : ScmDao, new()
        {
            var tmpDao = _SqlClient.Queryable<T>().First(a => a.id == dao.id);
            if (tmpDao != null)
            {
                tmpDao = dao.Adapt(tmpDao);
                tmpDao.PrepareUpdate();
                _SqlClient.Updateable(tmpDao).ExecuteCommand();
                return;
            }

            dao.PrepareCreate();
            _SqlClient.Insertable(dao).ExecuteCommand();
        }

        /// <summary>
        /// 清空记录
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="dao"></param>
        protected void TruncateDao<T>(T dao) where T : ScmDao, new()
        {
            _SqlClient.DbMaintenance.TruncateTable(dao.GetType());
        }

        /// <summary>
        /// 清空记录
        /// </summary>
        /// <param name="table"></param>
        protected void TruncateDao(string table)
        {
            _SqlClient.DbMaintenance.TruncateTable(table);
        }

        /// <summary>
        /// 执行外部脚本
        /// </summary>
        /// <param name="file"></param>
        /// <param name="major"></param>
        protected void ExecuteSql(string file, int major)
        {
            if (!File.Exists(file))
            {
                return;
            }

            var lines = File.ReadAllLines(file);
            var inComment = false;
            var needRun = false;
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    continue;
                }

                var sql = line.Trim();
                if (sql.StartsWith("/*"))
                {
                    inComment = true;
                }

                if (inComment)
                {
                    if (!needRun)
                    {
                        var ver = GetSqlVer(sql);
                        if (ver > major)
                        {
                            needRun = true;
                        }
                    }

                    if (sql.EndsWith("*/"))
                    {
                        inComment = false;
                    }

                    continue;
                }

                if (!needRun)
                {
                    return;
                }

                _SqlClient.Ado.ExecuteCommand(line);
            }
        }

        /// <summary>
        /// 获取脚本版本
        /// </summary>
        /// <param name="text"></param>
        /// <returns></returns>
        private static int GetSqlVer(string text)
        {
            var match = Regex.Match(text, @"[Vv]er[:]\s*(\d+)");
            if (!match.Success)
            {
                return 0;
            }
            if (match.Groups.Count < 2)
            {
                return 0;
            }
            var ver = match.Groups[1].Value;
            if (TextUtils.IsInteger(ver))
            {
                return int.Parse(ver);
            }

            return 0;
        }


        /// <summary>
        /// 删除表格
        /// </summary>
        protected bool DropTable(Assembly assembly)
        {
            var scmDao = typeof(ScmDao);
            var daoType = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType && u.Name.EndsWith("Dao")).ToList();
            foreach (var item in daoType.Where(s => !s.IsInterface))
            {
                if (!CommonUtils.HasImplementedRawGeneric(item, scmDao))
                {
                    continue;
                }

                var tableAttr = item.GetCustomAttribute<SugarTable>();
                if (tableAttr == null)
                {
                    continue;
                }

                var infos = _SqlClient.DbMaintenance.GetColumnInfosByTableName(tableAttr.TableName, false);
                if (infos.Count > 0)
                {
                    _SqlClient.DbMaintenance.DropTable(item);
                }
            }

            return true;
        }

        /// <summary>
        /// 数据库定义
        /// </summary>
        /// <param name="sqlClient"></param>
        protected bool InitTable(Assembly assembly)
        {
            var scmDao = typeof(ScmDao);
            var daoType = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType && u.Name.EndsWith("Dao")).ToList();
            var daoList = new List<Type>();
            foreach (var item in daoType.Where(s => !s.IsInterface))
            {
                if (CommonUtils.HasImplementedRawGeneric(item, scmDao))
                {
                    daoList.Add(item);
                }
            }
            _SqlClient.CodeFirst.InitTables(daoList.ToArray());
            return true;
        }

        /// <summary>
        /// 清空表格
        /// </summary>
        protected void TruncateTable(Assembly assembly)
        {
            var scmDao = typeof(ScmDao);
            var daoType = assembly.GetTypes().Where(u => u.IsClass && !u.IsAbstract && !u.IsGenericType && u.Name.EndsWith("Dao")).ToList();
            var daoList = new List<Type>();
            foreach (var item in daoType.Where(s => !s.IsInterface))
            {
                if (!CommonUtils.HasImplementedRawGeneric(item, scmDao))
                {
                    continue;
                }

                var tableAttr = item.GetCustomAttribute<SugarTable>();
                if (tableAttr == null)
                {
                    continue;
                }

                var infos = _SqlClient.DbMaintenance.GetColumnInfosByTableName(tableAttr.TableName, false);
                if (infos.Count > 0)
                {
                    daoList.Add(item);
                }
            }

            _SqlClient.DbMaintenance.TruncateTable(daoList.ToArray());
        }


        /// <summary>
        /// 数据库操作
        /// </summary>
        private void InitDml(ScmVerDao verDao)
        {
            if (verDao.major == 1)
            {
                _SqlClient.Ado.ExecuteCommand("UPDATE nas_res_file SET type= type * 10;");
                _SqlClient.Ado.ExecuteCommand("UPDATE nas_log_file SET type= type * 10;");
            }

            if (verDao.major == 2)
            {
                _SqlClient.Ado.ExecuteCommand("ALTER TABLE nas_cfg_drive DROP COLUMN folder_id;");
                _SqlClient.Ado.ExecuteCommand("ALTER TABLE nas_cfg_drive RENAME TO nas_cfg_folder;");
                _SqlClient.Ado.ExecuteCommand("ALTER TABLE nas_cfg_folder RENAME COLUMN last_id TO log_id;");
                _SqlClient.Ado.ExecuteCommand("ALTER TABLE nas_log_file RENAME COLUMN drive_id TO folder_id;");
                _SqlClient.Ado.ExecuteCommand("ALTER TABLE nas_res_file RENAME COLUMN drive_id TO folder_id;");
            }
        }
    }
}
