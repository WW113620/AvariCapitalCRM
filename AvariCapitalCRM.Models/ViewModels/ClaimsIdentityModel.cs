using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AvariCapitalCRM.Models.ViewModels
{
    public class ClaimsIdentityModel
    {
        public string userId { get; set; }
        public string userName { get; set; }
        public string roleType { get; set; }

        public string nickName { get; set; }
    }
}
