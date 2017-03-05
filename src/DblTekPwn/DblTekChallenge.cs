using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace DblTekPwn
{
    public class DblTekChallenge
    {
        public static string ComputeChallenge(int challenge)
        {
            string modified = (challenge + 20139 + (challenge >> 3)).ToString();

            byte[] buffer = new byte[64];

            for (int i = 0; i < modified.Length; i++)
                buffer[i] = (byte)modified[i];

            MD5 md5 = MD5.Create();
            byte[] hash = md5.ComputeHash(buffer);

            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < 6; i++)
                sb.Append(hash[i].ToString("X"));

            return sb.ToString().ToLower();
        }
    }
}
