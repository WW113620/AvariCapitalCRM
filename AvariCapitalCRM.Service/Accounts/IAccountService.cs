
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.Accounts
{
    public interface IAccountService : IDependency
    {
        int GetSendMessageCount(string phone, SysMessageEnums type, DateTime start, DateTime end);
        PageResult<AccountViewModel> GetAdminList(AdminRequest request, string loginName, RoleEnums loginType);
        PageResult<AccountViewModel> GetUserList(AccountRequest request, string loginName, RoleEnums loginType);

        PageResult<RoleViewModel> GetUserRoleList(RoleRequest request);
        AccountViewModel Get(long Id);

        int Delete(int id);

        int BatchDelete(List<string> idList);


    }
}
