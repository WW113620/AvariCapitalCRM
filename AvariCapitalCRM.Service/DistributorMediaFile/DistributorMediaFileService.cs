using Dapper;
using AvariCapitalCRM.Models.Dapper;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.DistributorMediaFile
{
    public class DistributorMediaFileService: IDistributorMediaFileService
    {
        public PageResult<DistributorMediaFileViewModel> GetFilesList(DistributorMediaFileRequest request)
        {
            PageResult<DistributorMediaFileViewModel> result = new PageResult<DistributorMediaFileViewModel>();
            string sql = @"  SELECT a.*,b.UserName FROM [dbo].[DistributorMediaFile] AS a
                             LEFT JOIN [dbo].[Account] AS b ON a.UserId=b.Id
                             WHERE a.Id>0 {0} ";
            string sort = " [Id] DESC ";

            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder();

            if (!string.IsNullOrEmpty(request.UserName))
            {
                string UserName = $"%{request.UserName.Trim()}%";
                sqlWhere.Append(" AND b.UserName like @UserName ");
                param.Add("UserName", UserName);
            }

            if (!string.IsNullOrEmpty(request.FileName))
            {
                string FileName = $"%{request.FileName.Trim()}%";
                sqlWhere.Append(" AND a.OriginalFileName like @FileName ");
                param.Add("FileName", FileName);
            }

            if (!string.IsNullOrEmpty(request.LoginName))
            {
                sqlWhere.Append(" AND b.UserName = @LoginName ");
                param.Add("LoginName", request.LoginName);
            }

            sql = string.Format(sql, sqlWhere.ToString());
            result = DapperHelper.GetPageList<DistributorMediaFileViewModel>(sql, sort, request, param);
            return result;

        }

    }
}
