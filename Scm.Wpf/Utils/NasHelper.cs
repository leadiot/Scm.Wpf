using Com.Scm.Enums;

namespace Com.Scm.Utils
{
    public class NasHelper
    {
        public static Dictionary<ScmFileKindEnum, List<string>> _ExtList = new Dictionary<ScmFileKindEnum, List<string>>();

        public static void Setup()
        {
            _ExtList[ScmFileKindEnum.Text] = new List<string> { "txt", "log", "json" };
            _ExtList[ScmFileKindEnum.Image] = new List<string> { "png", "jpg", "jpeg", "jpe", "jp2", "webp", "svg", "bmp", "gif",
                "tiff", "tif", "heic", "heif", "ai", "eps", "cdr", "ico", "psd" };
            _ExtList[ScmFileKindEnum.Audio] = new List<string> { "wav", "flac", "ape", "m4a", "aiff", "aif", "mp3", "aac", "m4a",
                "ogg", "wma", "amr", "mid", "midi", "dsf", "dff", "aac" };
            _ExtList[ScmFileKindEnum.Vedio] = new List<string> { "mp4", "mkv", "avi", "mov", "flv", "webm", "m3u8", "wmv", "ts",
                "mov", "3gp", "vob" };
            _ExtList[ScmFileKindEnum.Office] = new List<string> { "doc", "docx", "wps", "wpt", "pdf", "odt", "xls", "xlsx", "csv",
                "et", "ett", "ods", "ppt", "pptx", "dps", "dot", "odp", "vsx", "vsdx", "draw", "xmind" };
            _ExtList[ScmFileKindEnum.Archive] = new List<string> { "zip", "rar", "7z", "gz", "tgz", "tar", "bz", "bz2", "dmg", "cab",
                "iso", "lzma", "zipx", "arc", "jar", "war" };
            _ExtList[ScmFileKindEnum.Code] = new List<string> { "py", "js", "mjs", "cjs", "php", "php5", "phtml", "sh", "bash",
                "zsh", "c", "h", "cpp", "cc", "cxx", "hpp", "h", "cs", "java", "go", "html", "htm", "css", "scss", "sass",
                "less", "vue", "jsx", "tsx","ts","sql","json","yml","xml","yaml" ,"rb","swfit","kt","kts","rs"};
        }

        public static bool IsValid(ScmFileKindEnum doc, string ext)
        {
            if (!_ExtList.ContainsKey(doc))
            {
                return false;
            }

            var extList = _ExtList[doc];
            if (extList == null)
            {
                return false;
            }

            ext = ext.ToLower().TrimStart('.');
            return extList.Contains(ext);
        }

        public static bool IsImage(string ext)
        {
            return IsValid(ScmFileKindEnum.Image, ext);
        }

        public static bool IsAudio(string ext)
        {
            return IsValid(ScmFileKindEnum.Audio, ext);
        }

        public static bool IsVedio(string ext)
        {
            return IsValid(ScmFileKindEnum.Vedio, ext);
        }

        public static bool IsOffice(string ext)
        {
            return IsValid(ScmFileKindEnum.Office, ext);
        }

        public static bool IsScript(string ext)
        {
            return IsValid(ScmFileKindEnum.Code, ext);
        }

        public static bool IsArchive(string ext)
        {
            return IsValid(ScmFileKindEnum.Archive, ext);
        }

        public static ScmFileKindEnum GetKind(string ext)
        {
            if (ext != null)
            {
                foreach (var key in _ExtList.Keys)
                {
                    if (IsValid(key, ext))
                    {
                        return key;
                    }
                }
            }

            return ScmFileKindEnum.None;
        }
    }
}
