
using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class UserViewModel
    {
        public long UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public int RoleType { get; set; }

        public RoleEnums Role => (RoleEnums)RoleType;
    }

    public class UserRequest : BaseReqestParams
    {
        public string keyWord { get; set; }
    }

    public class LoginUser : UserViewModel
    {
        public bool IsAdmin { get; set; }
        public DateTime ExpiryDateTime { get; set; }
    }

    public class UserStringViewModel
    {
        public string UserId { get; set; }

        public string UserName { get; set; }

        public string NickName { get; set; }

        public string RoleType { get; set; }
    }
}
