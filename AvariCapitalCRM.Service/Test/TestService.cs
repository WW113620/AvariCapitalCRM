using Dapper;
using AvariCapitalCRM.Models.Dapper;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.Test
{
    public class TestService : ITestService
    {
        public string GetName()
        {
            return "123";
        }


        public List<UserViewModel> GetUserList()
        {
            string sql = @" SELECT * FROM TestUser WHERE Id>@Id ";
            return DapperHelper.Query<UserViewModel>(sql, new { Id = 0 });
        }

       

    }
}
