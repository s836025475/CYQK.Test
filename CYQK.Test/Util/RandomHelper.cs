using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CYQK.Test.Util
{
    public static class RandomHelper
    {
        const string _CHARS = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZ";
        const string _CHARNUMS = "0123456789";

        /// <summary>
        /// 生成指定长度的随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomString(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException("length");
            byte[] buffer1 = new byte[length];
            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(buffer1);
            char[] rs = new char[length];
            for (int i = 0; i < length; i++) rs[i] = _CHARS[buffer1[i] % _CHARS.Length];
            return new string(rs);
        }
        /// <summary>
        /// 生成指定长度数字随机字符串
        /// </summary>
        /// <param name="length"></param>
        /// <returns></returns>
        public static string RandomNumberString(int length)
        {
            if (length <= 0) throw new ArgumentOutOfRangeException("length");
            byte[] buffer1 = new byte[length];
            System.Security.Cryptography.RandomNumberGenerator.Create().GetBytes(buffer1);
            char[] rs = new char[length];
            for (int i = 0; i < length; i++) rs[i] = _CHARNUMS[buffer1[i] % _CHARNUMS.Length];
            return new string(rs);
        }
        /// <summary>
        /// 将给定的Int32类型转换成Char36字符串（如小于0，将返回空字符串）。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string Int32ToChar36(long value)
        {
            if (value <= 0) return "";
            string str = "";
            while (true)
            {
                if (value < 1) break;
                else if (value < _CHARS.Length)
                {
                    int index = (int)value - 1;
                    str = _CHARS[index] + str; break;
                }
                str = _CHARS[(int)((value - 1) % _CHARS.Length)] + str;
                value = (value - 1) / _CHARS.Length;
            }
            return str;
        }

        /// <summary>
        /// 将给定的Char36字符串转换成Int32类型（如字符串不能转换，将返回0）。
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static long Char36ToInt32(string value)
        {
            if (string.IsNullOrEmpty(value) || !System.Text.RegularExpressions.Regex.IsMatch(value, "^[0-9A-Z]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase)) return 0;
            value = value.ToUpper();
            long r = 0;
            for (int i = value.Length - 1; i >= 0; i--) r += (_CHARS.IndexOf(value[i]) + 1) * (long)Math.Pow(36d, value.Length - i - 1);
            return r;
        }

        /// <summary>
        /// 递增一个给定的36进位编码数。
        /// </summary>
        /// <param name="value">要递增的编码数。</param>
        /// <returns></returns>
        public static string IncreaseChar36(string value)
        {
            return Int32ToChar36(Char36ToInt32(value) + 1);
        }
    }
}
