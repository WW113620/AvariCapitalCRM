using AvariCapitalCRM.Models.ViewModels;
using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Utils
{
    public class EmailHelper
    {
        private static ILog logger = LogManager.GetLogger(typeof(EmailHelper));
        private static string _serverUserName;
        private static string _serverUserPassword;

        public EmailHelper(string severUserName, string serverUserPassword)
        {
            _serverUserName = severUserName;
            _serverUserPassword = serverUserPassword;
        }


        public BaseResult Office365Send(string toEmail, string fromEmail, string subject, string body, bool isBodyHtml)
        {
            try
            {
                logger.Info("发送失败，用户：" + toEmail + " ，主题：" + subject);
                MailMessage msg = new MailMessage();
                msg.To.Add(new MailAddress(toEmail, toEmail));
                msg.From = new MailAddress(fromEmail, fromEmail);
                msg.Subject = subject;
                msg.Body = body;
                msg.Priority = MailPriority.High;
                msg.IsBodyHtml = isBodyHtml;
                msg.Headers.Add("X-Mailer", "Microsoft Outlook Express 6.00.2900.2869"); //防止成为垃圾邮件，披上outlook的马甲
                msg.BodyEncoding = Encoding.GetEncoding("GB2312");

                SmtpClient client = new SmtpClient();
                client.UseDefaultCredentials = false;
                client.Credentials = new System.Net.NetworkCredential(_serverUserName, _serverUserPassword);
                client.Port = 587;
                client.Host = "smtp.office365.com";
                client.TargetName = "STARTTLS/smtp.office365.com";
                //client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.EnableSsl = true;
                client.Send(msg);

                return new BaseResult { code = 0, msg = "发送成功" };
            }
            catch (Exception ex)
            {
                logger.Error("发送失败，用户：" + toEmail + "，内部错误：" + ex.Message);
                return new BaseResult { code = 1, msg = "Exception：" + ex.Message };
            }

        }


        public BaseResult Send(string toEmail, string fromEmail, string subject, string body, bool isBodyHtml)
        {
            try
            {
                logger.Info("发送失败，用户：" + toEmail + " ，主题：" + subject);

                SmtpClient client = new SmtpClient("smtp.gmail.com", 587);
                MailMessage msg =
                new MailMessage(fromEmail, toEmail, subject, body);
                client.UseDefaultCredentials = false;
                msg.IsBodyHtml = isBodyHtml;
                msg.BodyEncoding = Encoding.GetEncoding("GB2312");
                System.Net.NetworkCredential basicAuthenticationInfo =
                new System.Net.NetworkCredential(_serverUserName, _serverUserPassword);
                client.Credentials = basicAuthenticationInfo;
                client.EnableSsl = true;
                client.Send(msg);

                return new BaseResult { code = 0, msg = "OK" };
            }
            catch (Exception ex)
            {
                logger.Error("发送失败，用户：" + toEmail + "，内部错误：" + ex.Message);
                return new BaseResult { code = 1, msg = "Exception：" + ex.Message };
            }

        }

    }

}
