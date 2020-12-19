using Dapper;
using AvariCapitalCRM.Models.Dapper;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.Accounts
{
    public class AccountService : IAccountService
    {
        public int GetSendMessageCount(string phone, SysMessageEnums type, DateTime start, DateTime end)
        {
            string sql = @" SELECT COUNT(0) AS iCount FROM [SMSMessageLog] WHERE SendTo=@phone AND [SendType]=@type AND [SendTime] BETWEEN @start AND @end ";
            int count = DapperHelper.Get<int>(sql, new { phone = phone, type = (int)type, start = start, end = end });
            return count;
        }

        public AccountViewModel Get(long Id)
        {
            string sql = @" SELECT * FROM [dbo].[Account] WHERE Id=@Id ";
            return DapperHelper.Get<AccountViewModel>(sql, new { Id = Id });
        }

        public PageResult<AccountViewModel> GetAdminList(AdminRequest request, string loginName, RoleEnums loginType)
        {
            PageResult<AccountViewModel> result = new PageResult<AccountViewModel>();
            string sql = @" SELECT a.*,b.RoleName as GroupName,b.Remark
                            FROM [dbo].[Account] as a
                            LEFT JOIN [dbo].[Role] as b ON a.RoleId=b.Id
                            WHERE a.RoleType IN (1,2) {0}";

            string sort = " RoleType ASC ";
            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder();
            if (!string.IsNullOrEmpty(request.UserName))
            {
                string UserName = $"%{request.UserName.Trim()}%";
                sqlWhere.Append(" AND (a.UserName like @UserName OR a.FirstName like @UserName OR a.LastName like @UserName ) ");
                param.Add("UserName", UserName.Trim());
            }
            if (loginType == RoleEnums.管理员 && !string.IsNullOrEmpty(loginName))
            {
                sqlWhere.Append(" AND a.UserName=@loginName ");
                param.Add("loginName", loginName);
            }
            sql = string.Format(sql, sqlWhere.ToString());
            result = DapperHelper.GetPageList<AccountViewModel>(sql, sort, request, param);
            return result;

        }

        public PageResult<AccountViewModel> GetUserList(AccountRequest request, string loginName, RoleEnums loginType)
        {
            string sql = @" SELECT a.*,b.UserName as AdviserName
                            FROM [dbo].[Account] AS a
							LEFT JOIN [dbo].[Account] AS b ON a.RoleId=b.Id
                            WHERE a.RoleType NOT IN (1,2) {0} ";

            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder();
            if (loginType == RoleEnums.顾问 && !string.IsNullOrEmpty(loginName))
            {
                sqlWhere.Append(" AND b.UserName=@loginName ");
                param.Add("loginName", loginName);
            }
            return GetList(request, sql, sqlWhere, param);
        }

        private PageResult<AccountViewModel> GetList(AccountRequest request, string sql, StringBuilder sqlWhere, DynamicParameters param)
        {
            PageResult<AccountViewModel> result = new PageResult<AccountViewModel>();

            if (!string.IsNullOrEmpty(request.UserName))
            {
                string UserName = $"%{request.UserName.Trim()}%";
                sqlWhere.Append(" AND (a.UserName like @UserName OR a.FirstName like @UserName OR a.LastName like @UserName ) ");
                param.Add("UserName", UserName.Trim());
            }

            if (!string.IsNullOrEmpty(request.Phone))
            {
                string Phone = $"%{request.Phone.Trim()}%";
                sqlWhere.Append(" AND a.Phone like @Phone ");
                param.Add("Phone", Phone.Trim());
            }

            if (!string.IsNullOrEmpty(request.Email))
            {
                string Email = $"%{request.Email.Trim()}%";
                sqlWhere.Append(" AND a.Email like @Email ");
                param.Add("Email", Email.Trim());
            }

            if (!string.IsNullOrEmpty(request.RoleType))
            {
                int RoleType = request.RoleType.ToInt(0);
                sqlWhere.Append(" AND a.RoleType = @RoleType ");
                param.Add("RoleType", RoleType);
            }

            if (!string.IsNullOrEmpty(request.State))
            {
                sqlWhere.Append(" AND a.State = @State ");
                param.Add("State", request.State.ToInt(0));
            }

            string sort = " Id DESC ";

            sql = string.Format(sql, sqlWhere.ToString());
            result = DapperHelper.GetPageList<AccountViewModel>(sql, sort, request, param);
            return result;
        }

        public PageResult<RoleViewModel> GetUserRoleList(RoleRequest request)
        {
            PageResult<RoleViewModel> result = new PageResult<RoleViewModel>();

            string sql = @" SELECT * FROM [dbo].[Role] WHERE IsDel=0 {0} ";
            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder();

            if (!string.IsNullOrEmpty(request.Name))
            {
                string RoleName = $"%{request.Name.Trim()}%";
                sqlWhere.Append(" AND RoleName like @RoleName ");
                param.Add("RoleName", RoleName.Trim());
            }
            string sort = " Id ASC ";
            sql = string.Format(sql, sqlWhere.ToString());
            result = DapperHelper.GetPageList<RoleViewModel>(sql, sort, request, param);
            return result;
        }

        public int Delete(int id)
        {
            if (id == 0)
            {
                return 0;
            }
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                dynamicParameters.Add("@ID", id);
                String sql = "Update Account set State=3 where Id=@ID";
                return DapperHelper.Execute(sql.ToString(), dynamicParameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

        }

        public int BatchDelete(List<string> idList)
        {
            try
            {
                DynamicParameters dynamicParameters = new DynamicParameters();
                StringBuilder stringBuilder = new StringBuilder();
                int i = 0;
                foreach (var item in idList)
                {
                    stringBuilder.AppendFormat("@ID{0},", i);
                    dynamicParameters.Add("@ID" + i, item);
                    i++;
                }
                string sql = "Update Account set State=3 where Id in (" + stringBuilder.ToString().TrimEnd(',') + ")";
                return DapperHelper.Execute(sql.ToString(), dynamicParameters);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return 0;
            }

        }


    }

}
