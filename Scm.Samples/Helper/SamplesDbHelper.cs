using Com.Scm.Dao;

namespace Com.Scm.Samples.Helper
{
    public class SamplesDbHelper : DbHelper
    {
        private const int MAJOR = 1;
        private const int MINOR = 0;
        private const int PATCH = 0;
        private const string BUILD = "2026030101";
        private const string RELEASE_DATE = "2026-03-01";

        public override string GetAppCode()
        {
            return "scm.samples";
        }

        public override int GetMajor()
        {
            return MAJOR;
        }

        public override int GetMinor()
        {
            return MINOR;
        }

        public override int GetPatch()
        {
            return PATCH;
        }

        public override string GetBuild()
        {
            return BUILD;
        }

        public override string GetRelease()
        {
            return RELEASE_DATE;
        }

        public override string GetVersion()
        {
            return "";
        }
    }
}
