
using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class AccountViewModel:Account
    {
        public string AddDateValue => AddTime.ToString("yyyy-MM-dd HH:mm");
        public string RoleName => EnumHelper.GetDescription((RoleEnums)RoleType);
        public string StateName => EnumHelper.GetDescription((UserStatusEnums)State);
        public string AccessName { get; set; }
        public string AdviserName { get; set; }
    }

    public class AdminRequest : BaseReqestParams
    {
        public string UserName { get; set; }
    }

    public class AccountRequest : BaseReqestParams
    {
        public string UserName { get; set; }
      
        public string Phone { get; set; }
        public string Email { get; set; }
        public string RoleType { get; set; }
        public string State { get; set; }
    }
}
