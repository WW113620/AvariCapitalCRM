
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.Test
{

    public interface ITestService : IDependency
    {
        string GetName();
      
        List<UserViewModel> GetUserList();
    }
}
