using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class CurrentUserModel
    {
        public long UserId { get; set; } = 0;

        public string LoginName { get; set; } = "";

        public int LoginType { get; set; } = 0;

        public RoleEnums Role => (RoleEnums)LoginType;
    }
}
