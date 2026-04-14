using Com.Scm.Dao;

namespace Com.Scm.Samples.Helper
{
    public class SamplesDbHelper : ScmDbHelper
    {
        private const int VER = 1;
        private const string DATE = "2026-03-01";

        public override string GetAppCode()
        {
            return "scm.samples";
        }

        public override int GetVer()
        {
            return VER;
        }

        public override string GetDate()
        {
            return DATE;
        }
    }
}
