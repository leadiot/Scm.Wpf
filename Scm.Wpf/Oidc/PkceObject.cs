using Com.Scm.Utils;
using System;
using System.Security.Cryptography;
using System.Text;

namespace Com.Scm
{
    public class PkceObject
    {
        public const string METHOD_PLAIN = "PLAIN";
        public const string METHOD_S256 = "S256";

        public string code_verifier { get; private set; }
        public string code_challenge { get; private set; }
        public string code_challenge_method { get; private set; }

        public bool Generate(string method = METHOD_S256, int length = 32)
        {
            if (!GenVerifier(length))
            {
                return false;
            }

            return GenChallenge(method);
        }

        private bool GenVerifier(int length = 32)
        {
            if (length < 32)
            {
                length = 32;
            }
            if (length > 128)
            {
                length = 128;
            }

            var random = new Random();
            var characters = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-._~";
            var builder = new StringBuilder();
            for (var i = 0; i < length; i++)
            {
                builder.Append(characters[random.Next(characters.Length)]);
            }
            code_verifier = builder.ToString();

            return true;
        }

        private bool GenChallenge(string method)
        {
            if (method == null)
            {
                return false;
            }

            method = method.ToUpper();
            if (method == METHOD_PLAIN)
            {
                code_challenge = code_verifier;
                code_challenge_method = method;
                return true;
            }

            if (method == METHOD_S256)
            {
                code_challenge = Sha256(code_verifier);
                code_challenge_method = method;
                return true;
            }

            return false;
        }

        private string Sha256(string verifier)
        {
            var sha = SHA256.Create();
            var bytes = Encoding.UTF8.GetBytes(verifier);
            bytes = sha.ComputeHash(bytes);
            var result = HttpUtils.ToBase64String(bytes);
            return result.TrimEnd('=');
        }

        public bool IsValid(string method, string challenge, string verifier)
        {
            if (method == null)
            {
                return false;
            }

            method = method.ToUpper().Trim();

            if (method == METHOD_PLAIN)
            {
                return challenge == verifier;
            }

            if (method == METHOD_S256)
            {
                if (string.IsNullOrWhiteSpace(challenge) || challenge.Length != 43 || string.IsNullOrWhiteSpace(verifier))
                {
                    return false;
                }

                return challenge == Sha256(verifier);
            }

            return false;
        }
    }
}
