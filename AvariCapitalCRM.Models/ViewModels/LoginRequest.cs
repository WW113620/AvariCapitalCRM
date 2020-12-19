using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace  AvariCapitalCRM.Models.ViewModels
{
    public class LoginRequest
    {
        public int PhonePrefix { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public int Remember { get; set; }

         public string code { get; set; }
    }

  
}
