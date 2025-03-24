using System;

namespace Com.Scm.Exceptions
{
    /// <summary>
    /// OIDC异常
    /// </summary>
    public class OidcException : Exception
    {
        public OidcException() { }

        public OidcException(string message) : base(message)
        {
        }
    }
}
