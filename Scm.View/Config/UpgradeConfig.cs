using System.IO;
using System.Text.Json;

namespace Com.Scm.Config
{
    public class UpgradeConfig
    {
        public const string CONFIG_FILE = "upgrade.json";

        /// <summary>
        /// 应用图标，用于升级程序显示
        /// 默认：无
        /// 选项：可选
        /// </summary>
        public string Icon { get; set; }

        /// <summary>
        /// 升级程序显示标题
        /// 默认：Upgrade.Wpf更新
        /// 选项：可选
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 升级程序是否自动执行更新
        /// 默认：false
        /// 选项：可选
        /// </summary>
        public bool AutoStart { get; set; }

        /// <summary>
        /// 升级程序是否在升级完成后自动退出
        /// 默认：false
        /// 选项：可选
        /// </summary>
        public bool AutoClose { get; set; }

        /// <summary>
        /// 是否展示升级步骤列表
        /// 默认：false
        /// 选项：可选
        /// </summary>
        public bool ShowSteps { get; set; }

        /// <summary>
        /// 应用信息配置，用于应用信息的展示
        /// 默认：null
        /// 选项：可选
        /// </summary>
        public string AppInfo { get; set; }

        /// <summary>
        /// 版本信息配置，用于版本信息的展示
        /// 默认：null
        /// 选项：可选
        /// </summary>
        public string VerInfo { get; set; }

        /// <summary>
        /// 现有版本，用于升级界面展示
        /// 默认：null，
        /// 选项：可选
        /// </summary>
        public string OldVersion { get; set; }

        /// <summary>
        /// 目标版本，用于升级界面展示
        /// 默认：null，
        /// 选项：可选
        /// </summary>
        public string NewVersion { get; set; }

        /// <summary>
        /// 升级步骤，系统按此步骤执行升级过程
        /// 默认：null
        /// 选项：必选
        /// </summary>
        public List<StepConfig> Steps { get; set; }

        public void LoadDefault()
        {
            Title = "Upgrade.Wpf更新";
            VerInfo = "这是版本更新说明！";
        }

        public static UpgradeConfig Load()
        {
            var file = Path.Combine(AppContext.BaseDirectory, CONFIG_FILE);
            if (File.Exists(file))
            {
                var json = File.ReadAllText(file);
                var options = new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                };
                return JsonSerializer.Deserialize<UpgradeConfig>(json, options);
            }

            return null;
        }

        public void Save(string baseDir)
        {
            var file = Path.Combine(AppContext.BaseDirectory, CONFIG_FILE);
            var json = JsonSerializer.Serialize(this);
            File.WriteAllText(file, json);
        }

        public void Add(StepConfig config)
        {
            if (Steps == null)
            {
                Steps = new List<StepConfig>();
            }
            Steps.Add(config);
        }
    }

    public enum UpgradeOption
    {
        None,
        Download,
        Command,
        Zip,
        Unzip,
        MoveDir,
        MoveDoc,
        CopyDir,
        CopyDoc,
        CreateDir,
        CreateDoc,
        DeleteDir,
        DeleteDoc,
        RenameDir,
        RenameDoc
    }

    public class StepConfig
    {
        #region 公共属性
        /// <summary>
        /// 步骤标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 步骤说明
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        /// 操作类型
        /// </summary>
        public UpgradeOption Option { get; set; }

        /// <summary>
        /// （操作完成后）等待时间
        /// </summary>
        public int WaitTime { get; set; }

        /// <summary>
        /// 错误时是否继续
        /// </summary>
        public bool ContinueOnError { get; set; }

        /// <summary>
        /// 重试次数
        /// </summary>
        public int RetryCount { get; set; }

        /// <summary>
        /// 重试等待时间（单位毫秒）
        /// </summary>
        public int RetryDelay { get; set; } = 1000;
        #endregion

        #region 扩展属性
        /// <summary>
        /// 来源文件或路径
        /// </summary>
        public string Source { get; set; }

        /// <summary>
        /// 目的文件或路径
        /// </summary>
        public string Destination { get; set; }

        /// <summary>
        /// 目标文件
        /// </summary>
        public string File { get; set; }

        /// <summary>
        /// 目标路径
        /// </summary>
        public string Path { get; set; }

        /// <summary>
        /// 下载链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 执行命令
        /// </summary>
        public string Command { get; set; }

        /// <summary>
        /// 命令参数
        /// </summary>
        public string Args { get; set; }

        /// <summary>
        /// 原名称
        /// </summary>
        public string OldName { get; set; }

        /// <summary>
        /// 新名称
        /// </summary>
        public string NewName { get; set; }

        /// <summary>
        /// 是否覆盖目标
        /// </summary>
        public bool Overwrite { get; set; } = true;
        #endregion

        /// <summary>
        /// 下载步骤
        /// </summary>
        /// <param name="url">下载链接</param>
        /// <param name="file">保存路径</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewDownloadStep(string title, string url, string file)
        {
            return new StepConfig
            {
                Option = UpgradeOption.Download,
                Title = title,
                Url = url,
                File = file
            };
        }

        /// <summary>
        /// 命令步骤
        /// </summary>
        /// <param name="command">要执行的命令</param>
        /// <param name="args">命令参数</param>
        /// <param name="path">工作目录（可选，默认为当前目录）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewCommandStep(string title, string command, string args, string path = null)
        {
            return new StepConfig
            {
                Option = UpgradeOption.Command,
                Title = title,
                Command = command,
                Args = args,
                Path = path,
            };
        }

        /// <summary>
        /// 压缩步骤
        /// </summary>
        /// <param name="source">源文件或目录路径</param>
        /// <param name="destination">目标压缩文件路径</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewZipStep(string title, string source, string destination)
        {
            return new StepConfig
            {
                Option = UpgradeOption.Zip,
                Title = title,
                Source = source,
                Destination = destination
            };
        }

        /// <summary>
        /// 解压步骤
        /// </summary>
        /// <param name="source">源压缩文件路径</param>
        /// <param name="destination">解压目标目录</param>
        /// <param name="overwrite">是否覆盖已存在的文件（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewUnzipStep(string title, string source, string destination, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.Unzip,
                Title = title,
                Source = source,
                Destination = destination,
                Overwrite = overwrite
            };
        }

        /// <summary>
        /// 移动目录步骤
        /// </summary>
        /// <param name="source">源目录路径</param>
        /// <param name="destination">目标目录路径</param>
        /// <param name="overwrite">是否覆盖已存在的文件（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewMoveDirStep(string title, string source, string destination, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.MoveDir,
                Title = title,
                Source = source,
                Destination = destination,
                Overwrite = overwrite
            };
        }

        /// <summary>
        /// 移动文件步骤
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="destination">目标文件路径</param>
        /// <param name="overwrite">是否覆盖已存在的文件（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewMoveDocStep(string title, string source, string destination, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.MoveDoc,
                Title = title,
                Source = source,
                Destination = destination,
                Overwrite = overwrite
            };
        }

        /// <summary>
        /// 复制目录步骤
        /// </summary>
        /// <param name="source">源目录路径</param>
        /// <param name="destination">目标目录路径</param>
        /// <param name="overwrite">是否覆盖已存在的文件（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewCopyDirStep(string title, string source, string destination, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.CopyDir,
                Title = title,
                Source = source,
                Destination = destination,
                Overwrite = overwrite
            };
        }

        /// <summary>
        /// 复制文件步骤
        /// </summary>
        /// <param name="source">源文件路径</param>
        /// <param name="destination">目标文件路径</param>
        /// <param name="overwrite">是否覆盖已存在的文件（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewCopyDocStep(string title, string source, string destination, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.CopyDoc,
                Title = title,
                Source = source,
                Destination = destination,
                Overwrite = overwrite
            };
        }

        /// <summary>
        /// 创建目录步骤
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewCreateDirStep(string title, string path)
        {
            return new StepConfig
            {
                Option = UpgradeOption.CreateDir,
                Title = title,
                Path = path
            };
        }

        /// <summary>
        /// 创建文件步骤
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <param name="overwrite">文件已存在时是否覆盖（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewCreateDocStep(string title, string path, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.CreateDoc,
                Title = title,
                Path = path,
                Overwrite = overwrite
            };
        }

        /// <summary>
        /// 删除目录步骤
        /// </summary>
        /// <param name="path">目录路径</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewDeleteDirStep(string title, string path)
        {
            return new StepConfig
            {
                Option = UpgradeOption.DeleteDir,
                Title = title,
                Path = path
            };
        }

        /// <summary>
        /// 删除文件步骤
        /// </summary>
        /// <param name="path">文件路径</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewDeleteDocStep(string title, string path)
        {
            return new StepConfig
            {
                Option = UpgradeOption.DeleteDoc,
                Title = title,
                Path = path
            };
        }

        /// <summary>
        /// 重命名目录步骤
        /// </summary>
        /// <param name="oldName">原目录路径</param>
        /// <param name="newName">新目录路径</param>
        /// <param name="overwrite">新目录已存在时是否覆盖（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewRenameDirStep(string title, string oldName, string newName, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.RenameDir,
                Title = title,
                OldName = oldName,
                NewName = newName,
                Overwrite = overwrite
            };
        }

        /// <summary>
        /// 重命名文件步骤
        /// </summary>
        /// <param name="oldName">原文件路径</param>
        /// <param name="newName">新文件路径</param>
        /// <param name="overwrite">新文件已存在时是否覆盖（默认true）</param>
        /// <returns>步骤配置</returns>
        public static StepConfig NewRenameDocStep(string title, string oldName, string newName, bool overwrite = true)
        {
            return new StepConfig
            {
                Option = UpgradeOption.RenameDoc,
                Title = title,
                OldName = oldName,
                NewName = newName,
                Overwrite = overwrite
            };
        }
    }
}
