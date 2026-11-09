using Com.Scm.Models;
using Com.Scm.Utils;
using HandyControl.Controls;
using Microsoft.Win32;
using MiniExcelLibs;
using MiniExcelLibs.Attributes;
using MiniExcelLibs.OpenXml;
using Newtonsoft.Json;
using System.Collections;
using System.IO;
using System.Text;

namespace Com.Scm.Helper
{
    /// <summary>
    /// 列表数据导出服务
    /// </summary>
    public static class GridExporter
    {
        /// <summary>
        /// 弹出保存对话框并按扩展名导出数据
        /// </summary>
        /// <param name="itemSource">数据列表</param>
        /// <param name="columns">导出列配置</param>
        /// <returns></returns>
        public static async Task<bool> ExportAsync(IEnumerable itemSource, IEnumerable<ScmColumnInfo> columns)
        {
            if (columns == null || !columns.Any())
            {
                Growl.Error("请选择需要导出的列!");
                return false;
            }

            var dialog = new SaveFileDialog
            {
                Filter = "Excel 文件(*.xlsx)|*.xlsx|Excel 97-2003 文件(*.xls)|*.xls|CSV (*.csv)|*.csv|SQL (*.sql)|*.sql|JSON (*.json)|*.json"
            };
            if (dialog.ShowDialog() != true)
            {
                return false;
            }

            var fileName = dialog.FileName;
            if (File.Exists(fileName))
            {
                try
                {
                    File.Delete(fileName);
                }
                catch (Exception exp)
                {
                    Growl.Error(exp.Message);
                    return false;
                }
            }

            bool result;
            try
            {
                if (fileName.EndsWith(".json"))
                {
                    result = await ExportJson(itemSource, columns, fileName);
                }
                else if (fileName.EndsWith(".csv"))
                {
                    result = await ExportCsv(itemSource, columns, fileName);
                }
                else if (fileName.EndsWith(".sql"))
                {
                    result = await ExportSql(itemSource, columns, fileName);
                }
                else
                {
                    result = await ExportXls(itemSource, columns, fileName);
                }
            }
            catch (Exception ex)
            {
                Growl.Error($"数据导出失败:{ex.Message}");
                return false;
            }

            if (result)
            {
                Growl.Success("数据导出成功！");
            }
            return result;
        }

        /// <summary>
        /// 导出CSV
        /// </summary>
        public static async Task<bool> ExportCsv(IEnumerable itemSource, IEnumerable<ScmColumnInfo> columns, string file)
        {
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = BuildExcelColumns(columns)
            };

            var items = GetValues(itemSource, columns);
            await MiniExcel.SaveAsAsync(file, items);
            return true;
        }

        /// <summary>
        /// 导出JSON
        /// </summary>
        public static async Task<bool> ExportJson(IEnumerable itemSource, IEnumerable<ScmColumnInfo> columns, string file)
        {
            var items = GetValues(itemSource, columns);
            var json = JsonConvert.SerializeObject(items, Formatting.Indented);
            await File.WriteAllTextAsync(file, json);
            return true;
        }

        /// <summary>
        /// 导出SQL
        /// </summary>
        public static async Task<bool> ExportSql(IEnumerable itemSource, IEnumerable<ScmColumnInfo> columns, string file)
        {
            var items = GetValues(itemSource, columns);
            var exportColumns = columns.Where(c => c.Export).ToList();

            var builder = new StringBuilder();
            builder.Append("INSERT INTO table_name (");
            builder.Append(string.Join(", ", exportColumns.Select(c => c.Value)));
            builder.Append(") VALUES ");
            var insert = builder.ToString();

            builder.Clear();
            foreach (var item in items)
            {
                builder.Append(insert);
                builder.Append('(');
                builder.Append(string.Join(", ", exportColumns.Select(c => FormatSqlValue(item.TryGetValue(c.Label, out var val) ? val : null))));
                builder.AppendLine(");");
            }

            await File.WriteAllTextAsync(file, builder.ToString());
            return true;
        }

        /// <summary>
        /// 导出XLS
        /// </summary>
        public static async Task<bool> ExportXls(IEnumerable itemSource, IEnumerable<ScmColumnInfo> columns, string file)
        {
            var config = new OpenXmlConfiguration
            {
                DynamicColumns = BuildExcelColumns(columns)
            };

            var items = GetValues(itemSource, columns);
            await MiniExcel.SaveAsAsync(file, items);
            return true;
        }

        /// <summary>
        /// 生成 MiniExcel 动态列配置
        /// </summary>
        private static DynamicExcelColumn[] BuildExcelColumns(IEnumerable<ScmColumnInfo> columns)
        {
            var objs = new List<DynamicExcelColumn>();
            var index = 0;
            foreach (var column in columns)
            {
                var tmp = new DynamicExcelColumn(column.Label) { Index = index++ };

                var uom = ScmColumnSize.Parse(column.Width);
                if (!uom.IsNone)
                {
                    tmp.Width = uom.Width;
                }
                objs.Add(tmp);
            }
            return objs.ToArray();
        }

        /// <summary>
        /// 格式化 SQL 字面值
        /// </summary>
        private static string FormatSqlValue(object value)
        {
            if (value == null || value == DBNull.Value)
            {
                return "NULL";
            }
            if (value is bool flag)
            {
                return flag ? "1" : "0";
            }
            if (value is DateTime time)
            {
                return $"'{time:yyyy-MM-dd HH:mm:ss}'";
            }
            return "'" + value.ToString().Replace("'", "''") + "'";
        }

        /// <summary>
        /// 转换为导出数据
        /// </summary>
        private static List<Dictionary<string, object>> GetValues(IEnumerable source, IEnumerable<ScmColumnInfo> columns)
        {
            var data = new List<Dictionary<string, object>>();
            foreach (var item in source)
            {
                data.Add(GetValue(item, columns));
            }
            return data;
        }

        /// <summary>
        /// 获取单条数据的导出值
        /// </summary>
        private static Dictionary<string, object> GetValue(object obj, IEnumerable<ScmColumnInfo> columns)
        {
            var dic = new Dictionary<string, object>();
            try
            {
                var type = obj.GetType();
                foreach (var column in columns)
                {
                    if (!column.Export)
                    {
                        continue;
                    }

                    var prop = type.GetProperty(column.Value);
                    if (prop == null)
                    {
                        continue;
                    }

                    var tmp = prop.GetValue(obj, null);
                    if (tmp is long)
                    {
                        dic[column.Label] = tmp.ToString();
                        continue;
                    }
                    if (tmp is DateTime time)
                    {
                        dic[column.Label] = TimeUtils.FormatDateTime(time);
                        continue;
                    }

                    dic[column.Label] = tmp;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
            return dic;
        }
    }
}
