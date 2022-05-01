using System;
using System.Collections.Generic;
using System.Text;

namespace Hzdtf.Utility.Utils
{
    /// <summary>
    /// 数字辅助类
    /// @ 黄振东
    /// </summary>
    public static class NumberUtil
    {
        /// <summary>
        /// 随机对象
        /// </summary>
        private static readonly Random ran = new Random();

        /// <summary>
        /// ASC码值数组
        /// </summary>
        private static readonly int[,] ascVals = new int[,]
        {
            { 48, 58 },
            { 65, 91 },
            { 97, 123 },
        };

        /// <summary>
        /// 分转换为元
        /// </summary>
        /// <param name="fen">分</param>
        /// <returns>元</returns>
        public static decimal? FenToYuan(this long? fen)
        {
            if (fen != null)
            {
                return FenToYuan(fen.GetValueOrDefault());
            }

            return null;
        }

        /// <summary>
        /// 分转换为元
        /// </summary>
        /// <param name="fen">分</param>
        /// <returns>元</returns>
        public static decimal FenToYuan(this long fen)
        {
            if (fen == 0)
            {
                return fen;
            }

            string str = fen.ToString();
            int splitIndex = str.Length - 2;
            string pre = str.Substring(0, splitIndex);

            string last = str.Substring(splitIndex, 2);
            if (Convert.ToSByte(last) == 0)
            {
                return Convert.ToDecimal(pre);
            }
            if ('0'.Equals(last[1]))
            {
                last = last[0].ToString();
            }

            return Convert.ToDecimal($"{pre}.{last}");
        }

        /// <summary>
        /// 随机生成数字字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>随机数字字符串</returns>
        public static string Random(int length = 4)
        {
            var str = new StringBuilder();
            var maxLengthStr = new StringBuilder("1");
            for (var i = 0; i < length; i++)
            {
                maxLengthStr.Append("0");
            }
            str.Append(ran.Next(0, Convert.ToInt32(maxLengthStr.ToString())));
            var tempLength = length - str.Length;
            for (int i = 0; i < tempLength; i++)
            {
                str.Insert(0, "0");
            }

            return str.ToString();
        }

        /// <summary>
        /// 随机生成英文数字字符串
        /// </summary>
        /// <param name="length">长度</param>
        /// <returns>英文数字字符串</returns>
        public static string EnNumRandom(int length = 4)
        {
            StringBuilder str = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                var rowIndex = ran.Next(0, 3);
                var min = ascVals[rowIndex, 0];
                var max = ascVals[rowIndex, 1];

                str.Append((char)ran.Next(min, max));
            }

            return str.ToString();
        }

        /// <summary>
        /// 生成固定长度的字符串
        /// 如果不足长度则前面补0
        /// </summary>
        /// <param name="num">数字</param>
        /// <param name="length">长度</param>
        /// <returns>固定长度的字符串</returns>
        public static string FixedLengthString(this int num, byte length)
        {
            string numStr = num.ToString();
            if (numStr.Length >= length)
            {
                return numStr;
            }

            StringBuilder result = new StringBuilder();
            for (int i = 0; i < length - numStr.Length; i++)
            {
                result.Append("0");
            }

            return $"{result.ToString()}{numStr}";
        }

        /// <summary>
        /// 编码进行与运算转换为掩码
        /// </summary>
        /// <param name="code">编码</param>
        /// <returns>掩码</returns>
        public static int CodeAndToMask(params int[] code)
        {
            int result = 0;
            if (code.IsNullOrLength0())
            {
                return 0;
            }
            foreach (var n in code)
            {
                result |= n;
            }

            return result;
        }

        /// <summary>
        /// 将数字对掩码进行或运算并得到是否匹配
        /// </summary>
        /// <param name="mask">掩码</param>
        /// <param name="code">编码</param>
        /// <returns>数字对掩码进行或运算并得到是否匹配</returns>
        public static bool IsCodeOrMaskEqual(int mask, int code)
        {
            return (code & mask) > 0;
        }

        /// <summary>
        /// 将字符串转换为长整型数字，如果字符串为空，则默认返回0。其他非长整型转换会发生异常
        /// </summary>
        /// <param name="longStr">长整型字符串</param>
        /// <returns>长整型数字</returns>
        public static long ToLong(this string longStr)
        {
            if (string.IsNullOrWhiteSpace(longStr))
            {
                return 0;
            }

            return Convert.ToInt64(longStr);
        }
    }
}
