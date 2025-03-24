using Com.Scm.Oidc;
using Com.Scm.Utils;
using System.Security.Cryptography;
using System.Text;

namespace Com.Scm.Response
{
    /// <summary>
    /// 握手响应
    /// </summary>
    public class HandshakeResponse : OidcResponse
    {
        public TicketInfo Ticket { get; set; }
    }

    public class TicketInfo
    {
        public string Code { get; set; }
        public string Salt { get; set; }
        public string Nonce { get; set; }

        public string GetDigest()
        {
            var alg = SHA256.Create();
            var input = Nonce + ":" + Salt;
            byte[] bytes = alg.ComputeHash(Encoding.UTF8.GetBytes(input));
            return TextUtils.ToHexString(bytes);
        }
    }

    public enum ListenHandle : byte
    {
        None,
        Todo,
        Doing,
        Done
    }

    public enum ListenResult : byte
    {
        None,
        Failure,
        Success
    }
}
