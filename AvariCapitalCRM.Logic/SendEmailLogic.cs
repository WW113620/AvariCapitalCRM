using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Logic
{
    public class SendEmailLogic
    {
        private static string _serverUserName = ConfigHelper.GetConfigValue("EmailServerUserName");
        private static string _serverUserPassword = ConfigHelper.GetConfigValue("EmailServerUserPassword");
        public static BaseResult Send(String toEmail, string title, String content, bool isBodyHtml = false)
        {
            if (string.IsNullOrEmpty(content))
            {
                return new BaseResult(1, "发送内容为空");
            }
            EmailHelper emailHelper = new EmailHelper(_serverUserName, _serverUserPassword);
            BaseResult baseResult = emailHelper.Send(toEmail, _serverUserName, title, content, isBodyHtml);
            return baseResult;

        }


     

    }

}
