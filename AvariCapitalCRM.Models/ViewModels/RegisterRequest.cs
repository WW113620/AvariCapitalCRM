using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class RegisterRequest
    {
        public int PhonePrefix { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string VerifyCode { get; set; }

    }

    public class ValidPhoneModel
    {
        public bool IsPhone { get; set; }
        public string Phone { get; set; }

        public PhoneTypeEnums PhoneType { get; set; }

    }

    public class ResetPasswordModel
    {
        public int PhonePrefix { get; set; }
        public string Phone { get; set; }
        public string Password { get; set; }
        public string RepeatPassword { get; set; }
    }

}
