using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AvariCapitalCRM.Models.ViewModels
{
    /// <summary>
    /// 常量
    /// </summary>
    public class ConstValue
    {
        /// <summary>
        /// 全局过滤器名称
        /// </summary>
        public const string IsDeletedFilterName = "ACMIsDeletedFilter";

        /// <summary>
        /// 登陆用户凭证Key
        /// </summary>
        public const string LoginUserIdKey = "ACMLoginUserId";

        
        public const string LoginUserRoleType = "ACMLoginRoleType";

        /// <summary>
        /// 认证cookie名称
        /// </summary>
        public const string JuCheapAuthorizeCookieName = "My.Authorize.AvariCapitalCRM";
    }
}
