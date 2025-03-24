using Com.Scm.Oidc;
using Com.Scm.Oidc.Response;

namespace Com.Scm.Response
{
    /// <summary>
    /// 侦听响应
    /// </summary>
    public class ListenResponse : OidcResponse
    {
        public string Salt { get; set; }
        public ListenHandle Handle { get; set; }
        public ListenResult Result { get; set; }

        public string access_token { get; set; }
        public string refresh_token { get; set; }
        public long expires_in { get; set; }

        public OidcUserInfo User { get; set; }
    }
}
