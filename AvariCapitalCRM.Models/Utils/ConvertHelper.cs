using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Utils
{
    public static class ConvertHelper
    {
        #region Convert
        public static string ToEmpty(this string data, string res = "")
        {
            if (string.IsNullOrEmpty(data))
                return res;
            return data;
        }
        /// <summary>
        /// To Int
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static int ToInt(this object data, int RtnData)
        {
            int rtnData = RtnData;
            try
            {
                rtnData = Convert.ToInt32(data);
            }
            catch
            {
                rtnData = RtnData;
            }
            return rtnData;
        }

        /// <summary>
        /// To Long
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static long ToLong(this object data, long RtnData)
        {
            long rtnData = RtnData;
            try
            {
                if (data != null && data.ToString() != "")
                    rtnData = Convert.ToInt64(data);
            }
            catch
            {
                rtnData = RtnData;
            }
            return rtnData;
        }

        /// <summary>
        /// To Float
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static float ToFloat(this object data, float RtnData)
        {
            float rtnData = RtnData;
            try
            {
                if (data != null)
                    rtnData = Convert.ToSingle(data);
            }
            catch
            {
                rtnData = RtnData;
            }
            return rtnData;
        }

        /// <summary>
        /// To Money
        /// </summary>
        /// <param name="Money"></param>
        /// <returns></returns>
        public static string ToMoney(this object Money)
        {
            double floatMoney = Money.ToDouble(0);
            if (floatMoney <= 0)
            {
                return "0";
            }
            else
            {
                floatMoney = Math.Round(floatMoney, 2, MidpointRounding.AwayFromZero);
                string rtnStr = floatMoney.ToString("0.00");
                return rtnStr;
            }
        }
        /// <summary>
        /// To Double
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static double ToDouble(this object data, double RtnData)
        {
            double rtnData = RtnData;
            try
            {
                if (data != null)
                    rtnData = Convert.ToDouble(data);
            }
            catch
            {
                rtnData = RtnData;
            }
            return rtnData;
        }
        /// <summary>
        /// To Decimal
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static decimal ToDecimal(this object data, decimal RtnData)
        {
            decimal rtnData = RtnData;
            try
            {
                if (data != null)
                    rtnData = Convert.ToDecimal(data);
            }
            catch
            {
                rtnData = RtnData;
            }
            return rtnData;
        }


        /// <summary>
        /// To String
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static string ToString(this object data, string RtnData)
        {
            string rtnData = RtnData;
            try
            {
                if (data != null && data.ToString().Length > 0)
                    rtnData = Convert.ToString(data);
            }
            catch
            {
                rtnData = RtnData;
            }
            return rtnData;
        }

        /// <summary>
        /// To Char
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static char ToChar(this object data, char RtnData)
        {
            char rtnData = RtnData;
            try
            {
                if (data != null && data.ToString().Length > 0)
                    rtnData = Convert.ToChar(data);
            }
            catch
            {
                rtnData = RtnData;
            }
            return rtnData;
        }

        /// <summary>
        /// To DateTime String
        /// </summary>
        /// <param name="format">format</param>
        /// <returns></returns>
        public static string ToDateTime(this object data, string RtnData, string format)
        {
            string rtnData = RtnData;
            try
            {
                if (data != null && data.ToString().Length > 0 && Convert.ToDateTime(data).ToString("yyyy") != "1900")
                    rtnData = Convert.ToDateTime(data).ToString(format);
            }
            catch
            {
            }
            return rtnData;
        }

        /// <summary>
        /// To DateTime
        /// </summary>
        /// <param name="data"></param>
        /// <param name="RtnData"></param>
        /// <returns></returns>
        public static DateTime ToDateTime(this object data, DateTime RtnData)
        {
            DateTime rtnData = RtnData;
            try
            {
                if (data != null && data.ToString().Length > 0 && Convert.ToDateTime(data).ToString("yyyy") != "1900")
                    rtnData = Convert.ToDateTime(data);
            }
            catch
            {
            }
            return rtnData;
        }

        public static byte[] StreamToBytes(System.IO.Stream stream)
        {
            if (stream == null)
                return null;
            byte[] bytes = new byte[stream.Length];
            stream.Read(bytes, 0, bytes.Length);
            stream.Seek(0, System.IO.SeekOrigin.Begin);
            return bytes;
        }
        public static System.IO.Stream BytesToStream(byte[] bytes)
        {
            if (bytes == null)
                return null;
            System.IO.Stream stream = new System.IO.MemoryStream(bytes);
            return stream;
        }

        public static bool ObjToBool(this object obj)
        {
            bool flag;
            if (obj == null)
            {
                return false;
            }
            if (obj.Equals(DBNull.Value))
            {
                return false;
            }
            return (bool.TryParse(obj.ToString(), out flag) && flag);
        }
        #endregion

        #region 正则
        public static bool IsPhone(string input)
        {
            string pattern = "^[0-9]{9,11}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 中国手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsMobilePhone(string input)
        {
            Regex regex = new Regex(@"^1[3456789]\d{9}$");
            return regex.IsMatch(input);
        }

        /// <summary>
        /// 澳洲手机号
        /// </summary>
        /// <param name="input"></param>
        /// <returns></returns>
        public static bool IsAuPhone(string input)
        {
            Regex regex = new Regex(@"^\d{9}$");
            return regex.IsMatch(input);
        }

        public static string TrimEmpty(this string input)
        {
            return Regex.Replace(input, @"\s", "");
        }

        public static bool IsEmail(string strValue)
        {
            if (string.IsNullOrEmpty(strValue))
                return false;
            Regex regex = new Regex(@"^[a-zA-Z0-9][\.a-zA-Z0-9_\-]*@[a-zA-Z0-9_\-]+(\.[a-zA-Z0-9_\-]+)+$", RegexOptions.IgnoreCase);
            return regex.IsMatch(strValue);
        }

        public static bool IsUserName(string strValue)
        {
            if (string.IsNullOrEmpty(strValue))
                return false;
            Regex regex = new Regex(@"^(?:[a-zA-Z0-9]+[-_.a-zA-Z0-9]+@[-_a-zA-Z0-9]+(?:\.[-_a-zA-Z0-9]+)+|[\u4e00-\u9fa5a-zA-Z_]\w){2,20}$", RegexOptions.IgnoreCase);
            return regex.IsMatch(strValue);
        }


        public static bool IsValidUserName(string userName)
        {
            return Regex.IsMatch(userName, @"^[A-Za-z0-9]\w{2,20}$");
        }

        /// <summary>
        /// 密码有效性
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public static bool IsValidPassword(string password)
        {
            return Regex.IsMatch(password, @"^[\S]{6,20}$");
        }

        /// <summary>
        /// 正整数
        /// </summary>
        /// <param name="strValue"></param>
        /// <returns></returns>
        public static bool IsIntegerNumber(string strValue)
        {
            return Regex.IsMatch(strValue, @"^[0-9]*[1-9][0-9]*$");
        }
        /// <summary>
        /// 身份证正则
        /// </summary>
        /// <param name="idCard"></param>
        /// <returns></returns>
        public static bool IsIDCard(string idCard)
        {
            return Regex.IsMatch(idCard, @"^[1-9]\d{7}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}$|^[1-9]\d{5}[1-9]\d{3}((0\d)|(1[0-2]))(([0|1|2]\d)|3[0-1])\d{3}([0-9]|X)$");
        }
        #endregion

        public static byte[] FileToBytes(string fileName)
        {
            FileStream fileStream = new FileStream(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
            byte[] bytes = new byte[fileStream.Length];
            fileStream.Read(bytes, 0, bytes.Length);
            fileStream.Close();
            return bytes;

        }

        public static int GetMediaType(string fileExt)
        {
            if (string.IsNullOrEmpty(fileExt))
                return 0;

            fileExt = System.IO.Path.GetExtension(fileExt).ToLower();

            string[] fileFilt = { ".gif", ".jpg", ".jpeg", ".png", ".bmp", ".heic", ".icon" };
            if (fileFilt.Contains(fileExt))
                return (int)MediaTypeEnums.图片;

            string[] fileVideoFilt = { ".mp4", ".rmvb", ".avi", ".mpeg", ".mov", ".flv", ".3gp", ".asf", ".mkv" };
            if (fileVideoFilt.Contains(fileExt))
                return (int)MediaTypeEnums.视频;

            return (int)MediaTypeEnums.文档;
        }

        public static string convertFileSize(long size)
        {
            const long GB = 1024 * 1024 * 1024;
            const long MB = 1024 * 1024;
            const long KB = 1024;

            if (size <= 0)
                return "";

            if (size / GB >= 1)
            {
                return Math.Round(size / (double)GB, 2) + "GB";
            }

            if (size / MB >= 1)
            {
                return Math.Round(size / (double)MB, 2) + "MB";
            }

            if (size / KB >= 1)
            {
                return Math.Round(size / (double)KB, 2) + "KB";
            }

            return size + "B";

        }

    }
}
