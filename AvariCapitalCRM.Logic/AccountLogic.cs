
using Newtonsoft.Json;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Service.Accounts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AvariCapitalCRM.Models.Data;

namespace AvariCapitalCRM.Logic
{
    public class AccountLogic
    {
        private readonly DataContextEntities _dataContext;
        private readonly IAccountService _accountService;

        public AccountLogic(DataContextEntities dataContext, IAccountService accountService)
        {
            this._dataContext = dataContext;
            this._accountService = accountService;
        }

        public BaseResult Login(LoginRequest model, ref UserViewModel userView)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                    return new BaseResult(1, "用户名不能为空");

                if (string.IsNullOrEmpty(model.Password))
                    return new BaseResult(1, "密码不能为空");

                var user = this._dataContext.Accounts.FirstOrDefault(p => p.UserName == model.UserName);
                if (user == null)
                    return new BaseResult(2, "用户不存在");

                //if (!Enum.IsDefined(typeof(RoleEnums), user.RoleType))
                //    return new BaseResult(1, "此用户无法登录系统");
                if (user.State == (int)UserStatusEnums.待审核)
                    return new BaseResult(1, "此用户未通过审核");
                if (user.State == (int)UserStatusEnums.已锁定)
                    return new BaseResult(1, "此用户已被锁定");
                string encryptedWithSaltPassword = CommonHelper.GetPasswrodWithTwiceEncode(model.Password, user.PasswordSalt);
                if (user.Password != encryptedWithSaltPassword)
                    return new BaseResult(1, "密码不对");

                userView = new UserViewModel { UserId = user.Id, UserName = user.UserName, NickName = user.FirstName};
                return new BaseResult(0, "登录成功");
            }
            catch (Exception e)
            {
                return new BaseResult(1, "Exception:" + e.Message);
            }

        }

        public BaseResult WebLogin(LoginRequest model, ref UserViewModel userView)
        {
            try
            {
                if (string.IsNullOrEmpty(model.UserName))
                    return new BaseResult(1, "手机号不能为空");

                if (string.IsNullOrEmpty(model.Password))
                    return new BaseResult(1, "密码不能为空");

                var user = this._dataContext.Accounts.FirstOrDefault(p => p.UserName == model.UserName);
                if (user == null)
                {
                    user = this._dataContext.Accounts.FirstOrDefault(p => p.Phone == model.UserName);
                    if (user == null)
                        return new BaseResult(2, "用户不存在");
                }
               
                if (user.State == (int)UserStatusEnums.待审核)
                    return new BaseResult(1, "此用户未通过审核");
                if (user.State == (int)UserStatusEnums.已锁定)
                    return new BaseResult(1, "此用户已被锁定");
                string encryptedWithSaltPassword = CommonHelper.GetPasswrodWithTwiceEncode(model.Password, user.PasswordSalt);
                if (user.Password != encryptedWithSaltPassword)
                    return new BaseResult(1, "密码不对");

                userView = new UserViewModel { UserId = user.Id, UserName = user.UserName, NickName = user.FirstName};
                return new BaseResult(0, "登录成功");
            }
            catch (Exception e)
            {
                return new BaseResult(1, "Exception:" + e.Message);
            }

        }

        public string SendMessage(string code, string to, PhoneTypeEnums phoneType = PhoneTypeEnums.中国)
        {
            string sendResult = string.Empty;
            DateTime now = DateTime.Now;
            string message = $"验证码为：{code} (有效期10分钟)，如非本人操作，请忽略本短信【毛毛雨健康生活馆】";
            var smsLog = new SMSMessageLog { Message = message, SendTo = to, SendFromGuid = Guid.NewGuid().ToString(), SendTime = now, Result = "", SendType = (int)SysMessageEnums.用户注册 };
            try
            {
                DateTime end = now.AddMinutes(5);
                int count = this._accountService.GetSendMessageCount(to, SysMessageEnums.用户注册, now, end);
                if (count > 10)
                    return "短信验证码发送太频繁，稍后再发送";
                if (phoneType == PhoneTypeEnums.中国)
                {
                    sendResult = SMSService.SendChinaMessage(message, to);
                }
                else
                {
                    sendResult = SMSService.SendMessage(message, to);
                }
                smsLog.Result = sendResult;
            }
            catch (Exception e)
            {
                smsLog.Result = "fail exception:" + e.Message;
            }
            this._dataContext.SMSMessageLogs.Add(smsLog);
            this._dataContext.SaveChanges();
            return sendResult;
        }


        public ValidPhoneModel ValidPhone(PhoneTypeEnums phoneType, string phone)
        {
            if (phoneType == PhoneTypeEnums.中国)
            {
                if (!ConvertHelper.IsMobilePhone(phone))
                    return new ValidPhoneModel { IsPhone = false };

                return new ValidPhoneModel { IsPhone = true, Phone = phone, PhoneType = PhoneTypeEnums.中国 };
            }
            else if (phoneType == PhoneTypeEnums.澳洲)
            {
                if (!ConvertHelper.IsAuPhone(phone))
                    return new ValidPhoneModel { IsPhone = false };
                return new ValidPhoneModel { IsPhone = true, Phone = phone, PhoneType = PhoneTypeEnums.澳洲 };
            }

            return new ValidPhoneModel { IsPhone = false };
        }


        public BaseResult ChangePassword(AccountPwdModel model)
        {
            if (model.password != model.repassword)
                return new BaseResult(1, "The confirm password and the new password do not match");

            var user = this._dataContext.Accounts.Find(model.Id);
            if (user == null)
                return new BaseResult(2, "The user does not exist");

            string encryptedWithSaltPassword = CommonHelper.GetPasswrodWithTwiceEncode(model.oldPassword, user.PasswordSalt);
            if (user.Password != encryptedWithSaltPassword)
                return new BaseResult(1, "The old password is incorrect");

            string encryptedNewPassword = CommonHelper.GetPasswrodWithTwiceEncode(model.password, user.PasswordSalt);
            if (user.Password == encryptedNewPassword)
                return new BaseResult(1, "The new password cannot be equal to the old password");

            var salt = Guid.NewGuid().ToString("N").Substring(12);
            string encryptedWithSaltNewPassword = CommonHelper.GetPasswrodWithTwiceEncode(model.password, salt);
            user.Password = encryptedWithSaltNewPassword;
            user.PasswordSalt = salt;
            this._dataContext.SaveChanges();

            return new BaseResult(0, "Change Successfully");
        }

        public BaseResult UserResetPassword(ResetPasswordModel model)
        {
            string Phone = model.Phone.Trim();
            Account user = new Account();
            user = this._dataContext.Accounts.FirstOrDefault(p => p.UserName == Phone && p.State == (int)UserStatusEnums.已通过);
            if (user == null)
            {
                user = this._dataContext.Accounts.FirstOrDefault(p => p.Phone == Phone && p.State == (int)UserStatusEnums.已通过);
                if (user == null)
                    return new BaseResult(1, "手机号不存在，请先注册");
            }

            if (string.IsNullOrEmpty(model.Password))
                return new BaseResult(1, "密码不能为空");

            model.Password = model.Password.Trim();
            if (!ConvertHelper.IsValidPassword(model.Password))
                return new BaseResult(1, "密码必须6到20位，且不能出现空格");
            if (model.Password != model.RepeatPassword)
                return new BaseResult(1, "两次密码不匹配");

            var salt = Guid.NewGuid().ToString("N").Substring(12);
            string encryptedWithSaltNewPassword = CommonHelper.GetPasswrodWithTwiceEncode(model.Password, salt);
            user.Password = encryptedWithSaltNewPassword;
            user.PasswordSalt = salt;
            this._dataContext.SaveChanges();

            return new BaseResult(0, "修改成功");
        }







    }
}
