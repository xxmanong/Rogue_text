using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace okcopy.Common
{
    /// <summary>
    /// MD5加密帮助类
    /// </summary>
    public class MD5er
    {
        /// <summary>
        /// 32位加密转大写
        /// </summary>
        /// <param name="clearText">明文</param>
        /// <returns>32位大写密文</returns>
        public static string Encrypt(string clearText)
        {
            StringBuilder sb = new StringBuilder(32);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(Encoding.GetEncoding("utf-8").GetBytes(clearText));
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString().ToUpper();
        }

        /// <summary>
        /// 32位加密转大写(适用于表情字符进行加密)
        /// </summary>
        /// <param name="clearText">byte数组</param>
        /// <returns>32位大写密文</returns>
        public static string EncryptByte(byte[] clearText)
        {
            StringBuilder sb = new StringBuilder(32);

            MD5 md5 = new MD5CryptoServiceProvider();
            byte[] t = md5.ComputeHash(clearText);
            for (int i = 0; i < t.Length; i++)
            {
                sb.Append(t[i].ToString("x").PadLeft(2, '0'));
            }

            return sb.ToString().ToUpper();
        }
    }
}
