using AvariCapitalCRM.Models.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class RoleViewModel : Role
    {
        public string AddDateValue => AddTime.ToString("yyyy-MM-dd HH:mm");
    }

    public class RoleRequest : BaseReqestParams
    {
        public string Name { get; set; }
    }

}
