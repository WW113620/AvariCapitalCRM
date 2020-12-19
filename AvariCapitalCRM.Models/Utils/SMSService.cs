
using Newtonsoft.Json;
using AvariCapitalCRM.Models.ViewModels.SMSMessage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Configuration;

namespace AvariCapitalCRM.Models.Utils
{
    public class SMSService
    {
        public static string SendMessage(string message, string to)
        {
            string apiKey = WebConfigurationManager.AppSettings["smsApiKey"].Trim();
            string apiSecret = WebConfigurationManager.AppSettings["smsApiSecret"].Trim();
            string requestUrl = WebConfigurationManager.AppSettings["smsRequestUrl"].Trim();
            string from = WebConfigurationManager.AppSettings["smsFrom"];
            string auth = apiKey + ":" + apiSecret;
            string authBase64 = Base64Encode(auth);
            string messageEncoded = HttpContext.Current != null ? HttpContext.Current.Server.UrlEncode(message) : System.Web.HttpUtility.UrlEncode(message);
            string toEncoded = HttpContext.Current != null ? HttpContext.Current.Server.UrlEncode(to) : System.Web.HttpUtility.UrlEncode(to);
            string fromEncoded = HttpContext.Current != null ? HttpContext.Current.Server.UrlEncode(from) : System.Web.HttpUtility.UrlEncode(from);
            string query = "message=" + messageEncoded + "&to=" + toEncoded + "&from=" + fromEncoded;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(requestUrl);

            //Set values for the request back
            req.Method = "POST";
            req.ContentType = "application/x-www-form-urlencoded";
            req.ContentLength = query.Length;
            req.Headers.Add("Authorization", "Basic " + authBase64);

            try
            {
                StreamWriter streamOut = new StreamWriter(req.GetRequestStream(), System.Text.Encoding.ASCII);
                streamOut.Write(query);
                streamOut.Close();
                StreamReader streamIn = new StreamReader(req.GetResponse().GetResponseStream());
                string strResponse = streamIn.ReadToEnd();
                streamIn.Close();

                string result = string.Empty;
                SMSResponse response = JsonConvert.DeserializeObject<SMSResponse>(strResponse);
                if (response != null && response.errormsg != null)//send success
                {
                    result = "fail,info:" + response.errormsg;
                    if (result.Length > 200)
                        result = result.Substring(0, 190) + "...";
                }
                else
                {
                    result = "发送成功";
                }

                return result;
            }
            catch (Exception ex)
            {
                return "{ errormsg = \"" + ex.Message + "\" }";
            }
        }


        public static string SendChinaMessage(string message, string to)
        {
            try
            {
                string account = WebConfigurationManager.AppSettings["mxtongAccount"].Trim();
                string pswd = WebConfigurationManager.AppSettings["mxtongPswd"].Trim();
                string postUrl = WebConfigurationManager.AppSettings["mxtongRequestUrl"].Trim();
                string postStrTpl = "account={0}&pswd={1}&mobile={2}&msg={3}&needstatus =true";

                UTF8Encoding encoding = new UTF8Encoding();
                byte[] postData = encoding.GetBytes(string.Format(postStrTpl, account, pswd, to, message));
                System.GC.Collect();
                HttpWebRequest myRequest = (HttpWebRequest)HttpWebRequest.Create(postUrl);
                myRequest.KeepAlive = false;
                myRequest.Method = "POST";
                myRequest.ContentType = "application/x-www-form-urlencoded";
                myRequest.ContentLength = postData.Length;
                myRequest.Timeout = 5000;
                Stream newStream = myRequest.GetRequestStream();
                // Send the data.    
                newStream.Write(postData, 0, postData.Length);
                newStream.Flush();
                newStream.Close();

                HttpWebResponse myResponse = (HttpWebResponse)myRequest.GetResponse();

                StreamReader streamIn = new StreamReader(myResponse.GetResponseStream());
                string strResponse = streamIn.ReadToEnd();
                streamIn.Close();
                var arrMsg = strResponse.Split(',');
                if (arrMsg.Length == 2)
                {
                    int code = arrMsg[1].ToInt(1);
                    string msg = Enum.GetName(typeof(SendMessageEnums), code);
                    return msg;
                }
                else
                {
                    return "发送成功";
                }
            }
            catch (Exception e)
            {
                return "失败：" + e.Message;
            }

        }

        public static string Base64Encode(string plainText)
        {
            var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
            return System.Convert.ToBase64String(plainTextBytes);
        }

        public static string String2Unicode(string source)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(source);
            StringBuilder stringBuilder = new StringBuilder();
            for (int i = 0; i < bytes.Length; i += 2)
            {
                stringBuilder.AppendFormat("\\u{0}{1}", bytes[i + 1].ToString("x").PadLeft(2, '0'), bytes[i].ToString("x").PadLeft(2, '0'));
            }
            return stringBuilder.ToString();
        }
    }

}
