using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace AvariCapitalCRM.Models.Utils
{
    public static class CommonHelper
    {

        #region PostValue
        public static string GetPostValue(this string FormKey, int maxLength = 0, bool isClearHtml = false, bool isClearSql = false, bool isReservationSimpleTag = false, System.Web.Routing.RouteValueDictionary routeDataValue = null)
        {
            string RetValue = "";
            try
            {
                if (routeDataValue != null && routeDataValue.Keys.Contains(FormKey) && routeDataValue[FormKey].ToString("").Length > 0)
                {
                    RetValue = routeDataValue[FormKey].ToString("");
                }
                else
                {
                    if (HttpContext.Current.Request.Form[FormKey] != null)
                    {
                        RetValue = HttpContext.Current.Request.Form[FormKey].ToString();
                    }
                    else if (HttpContext.Current.Request.QueryString[FormKey] != null)
                    {
                        RetValue = HttpContext.Current.Request.QueryString[FormKey].ToString();
                    }
                }
                if (string.IsNullOrWhiteSpace(RetValue))
                    return string.Empty;

                RetValue = RetValue.Trim();
                if (isClearHtml)
                {
                    RetValue = StringNoHtml(RetValue, isReservationSimpleTag, maxLength);
                }
                if (isClearSql)
                {
                    RetValue = SqlFilterExt(RetValue);
                }
                if (maxLength > 0 && RetValue.Length > maxLength)
                {
                    RetValue = RetValue.Substring(0, maxLength);
                }
                RetValue = RetValue.Trim();
            }
            catch (Exception ex)
            {

            }
            if (RetValue == "*")
                RetValue = string.Empty;
            return RetValue.TrimEnd(';');
        }
        /// <summary>
        /// 去掉全部HTML的, 将 小于号=>  gt;,大于号=>  lt;,空格=>  nbsp 进行转换
        /// </summary>
        /// <param name="strHtml"></param>
        /// <param name="isReservationSimpleTag">是否保留大于小于号、制表和换行符</param>
        /// <param name="maxLength">截取的长度</param>
        /// <returns></returns>
        public static string StringNoHtml(string strHtml, bool isReservationSimpleTag = false, int maxLength = 0)
        {
            if (String.IsNullOrWhiteSpace(strHtml))
            {
                return string.Empty;
            }
            else
            {
                string[] aryReg ={
                @"<script[^>]*?>.*?</script>",
                @"<iframe[^>]*?>.*?</iframe>",
                @"<!--.*\n(-->)?",
                @"<(\/\s*)?(.|\n)*?(\/\s*)?>",
                @"<(\w|\s|""|'| |=|\\|\.|\/|#)*",
                @"([\r\n|\s])*",
                @"&(quot|#34);",
                @"&(amp|#38);",
                @"&(lt|#60);",
                @"&(gt|#62);",
                @"&(nbsp|#160);",
                @"&(iexcl|#161);",
                @"&(cent|#162);",
                @"&(pound|#163);",
                @"&(copy|#169);",
                @"%(27|32|3E|3C|3D|3F)",
                @"&#(\d+);"};

                string newReg = aryReg[0];
                string strOutput = strHtml.Replace("&nbsp;", " ");
                for (int i = 0; i < aryReg.Length; i++)
                {
                    Regex regex = new Regex(aryReg[i], RegexOptions.IgnoreCase);
                    strOutput = regex.Replace(strOutput, string.Empty);
                }
                if (!isReservationSimpleTag)
                {
                    strOutput = strOutput.Replace("<", "&gt;").Replace(">", "&lt;").Replace("\r", string.Empty).Replace("\n", string.Empty);
                }
                if (maxLength > 0 && strOutput.Length > maxLength)
                    strOutput = strOutput.Substring(0, maxLength);
                return strOutput.Replace(" ", "&nbsp;");
            }
        }
        /// <summary>
        /// 去除sql保留字
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public static string SqlFilterExt(string sql)
        {
            string sqlfilter = @"exec|insert|select|delete|update|chr|mid|master|or|and|truncate|char|declare|join|cmd|;|'|--";
            if (string.IsNullOrEmpty(sql))
                return string.Empty;
            foreach (string i in sqlfilter.Split('|'))
            {
                sql = sql.Replace(i + " ", string.Empty).Replace(" " + i, string.Empty).Replace(i + "%20", string.Empty).Replace("%20" + i, string.Empty);
            }
            sql = sql.Replace("'", "");
            return sql;
        }
        #endregion

        #region Create Guid
        /// <summary>
        /// Create Guid
        /// </summary>
        public static string CreateGuid(string type)
        {
            string result = string.Empty;
            switch (type.ToLower())
            {
                default:
                case "user":
                    result = "U" + Guid.NewGuid().ToString("N");
                    break;
            }
            return result;
        }
        #endregion

        #region MD5 Encrypt
        /// <summary>
        /// MD5
        /// </summary>
        /// <param name="input">input string</param>
        /// <returns>result</returns>
        public static string MD5Encrypt(string input)
        {
            System.Security.Cryptography.MD5 md5 = System.Security.Cryptography.MD5.Create();
            string encoded = BitConverter.ToString(md5.ComputeHash(Encoding.Default.GetBytes(input))).Replace("-", "");
            return encoded;
        }
        #endregion

        #region Passwrod
        public static string GetPasswrodWithTwiceEncode(string password, string salt)
        {
            string encryptedPassword = MD5Encrypt(password);//一次MD5加密
            string encryptedWithSaltPassword = MD5Encrypt(encryptedPassword + salt);//一次结果加盐后二次加密
            return encryptedWithSaltPassword;
        }


        private static string AESKey = "Abc123987qaz1234";

        /// <summary> 
        /// AES加密 
        /// </summary>
        public static string AESEncrypt(string value, string _aeskey = null)
        {
            if (string.IsNullOrEmpty(_aeskey))
            {
                _aeskey = AESKey;
            }

            byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
            byte[] toEncryptArray = Encoding.UTF8.GetBytes(value);

            RijndaelManaged rDel = new RijndaelManaged();
            rDel.Key = keyArray;
            rDel.Mode = CipherMode.ECB;
            rDel.Padding = PaddingMode.PKCS7;

            ICryptoTransform cTransform = rDel.CreateEncryptor();
            byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

            return Convert.ToBase64String(resultArray, 0, resultArray.Length);
        }

        /// <summary> 
        /// AES解密 
        /// </summary>
        public static string AESDecrypt(string value, string _aeskey = null)
        {
            try
            {
                if (string.IsNullOrEmpty(_aeskey))
                {
                    _aeskey = AESKey;
                }
                byte[] keyArray = Encoding.UTF8.GetBytes(_aeskey);
                byte[] toEncryptArray = Convert.FromBase64String(value);

                RijndaelManaged rDel = new RijndaelManaged();
                rDel.Key = keyArray;
                rDel.Mode = CipherMode.ECB;
                rDel.Padding = PaddingMode.PKCS7;

                ICryptoTransform cTransform = rDel.CreateDecryptor();
                byte[] resultArray = cTransform.TransformFinalBlock(toEncryptArray, 0, toEncryptArray.Length);

                return Encoding.UTF8.GetString(resultArray);
            }
            catch
            {
                return string.Empty;
            }
        }



        #endregion


        #region DateTime to String
        public static DateTime GetCurrentDateTime()
        {
            return DateTime.Now;
        }

        public static string DateTimeValue(this DateTime? dt, string format = "")
        {
            if (dt == null || dt == DateTime.MinValue || dt == DateTime.MaxValue)
                return format;
            return Convert.ToDateTime(dt).ToString("yyyy-MM-dd HH:mm");
        }
        #endregion


        #region Common
        public static string RandString(int length)
        {
            string chars = "0123456789ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz";
            Random randrom = new Random((int)DateTime.Now.Ticks);

            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                builder.Append(chars[randrom.Next(chars.Length)]);
            }
            return builder.ToString();
        }

        /// <summary>
        /// 生成随机数字
        /// </summary>
        /// <param name="Length">生成长度</param>
        /// <param name="Sleep">是否要在生成前将当前线程阻止以避免重复</param>
        /// <returns></returns>
        public static string RandNumber(int Length, bool Sleep = false)
        {
            if (Sleep)
            {
                System.Threading.Thread.Sleep(3);
            }
            string result = "";
            System.Random random = new Random();
            for (int i = 0; i < Length; i++)
            {
                result += random.Next(0, 10).ToString();
            }
            return result;
        }

        public static List<string> GetDateList(DateTime beginTime, DateTime endTime)
        {
            List<string> dateList = new List<string>();
            for (DateTime dt = beginTime; dt <= endTime; dt = dt.AddDays(1))
            {
                dateList.Add(dt.ToString("yyyy-MM-dd"));
            }
            return dateList;
        }

        #endregion

    }
}
