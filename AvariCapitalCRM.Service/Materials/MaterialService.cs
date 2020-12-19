using Dapper;
using AvariCapitalCRM.Models.Dapper;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.Materials
{
    public class MaterialService : IMaterialService
    {
        public MaterialViewModel Get(long Id)
        {
            string sql = @" SELECT a.*,b.Name as MaterialName,b.Type,c.Name as CategoryName
							FROM [MediaFile] as a
						    INNER JOIN [Materials] as b ON a.MaterialId=b.Id
							LEFT JOIN [Category] as c ON b.CategoryId=c.Id WHERE a.Id=@Id ";
            var model = DapperHelper.Get<MaterialViewModel>(sql, new { Id = Id });
            return model;
        }

        public PageResult<MaterialFileViewModel> GetList(MaterialRequest request, long userId, RoleEnums loginType, List<long> cateIdArray)
        {
            PageResult<MaterialFileViewModel> result = new PageResult<MaterialFileViewModel>();
            string sql = @"  SELECT a.*,b.Name as MaterialName,b.Type,b.ClientId,c.Name as CategoryName,d.UserName
							FROM [MediaFile] as a
						    INNER JOIN [Materials] as b ON a.MaterialId=b.Id
							LEFT JOIN [Category] as c ON b.CategoryId=c.Id 
							LEFT JOIN [Account] as d ON b.ClientId=d.Id 
							WHERE a.Id>0 {0} ";
            string sort = " AddTime DESC ";

            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder();

            if (loginType == RoleEnums.顾问)
            {
                sqlWhere.Append(" AND b.CategoryId IN @cateIdArray ");
                param.Add("cateIdArray", cateIdArray);
            }
            else if (loginType == RoleEnums.客户 || request.UserId > 0)
            {
                sqlWhere.Append(" AND ( b.CategoryId IN @cateIdArray OR b.ClientId=@ClientId ) ");
                param.Add("cateIdArray", cateIdArray);
                param.Add("ClientId", userId);
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                string Name = $"%{request.Name.Trim()}%";
                sqlWhere.Append(" AND b.Name like @Name ");
                param.Add("Name", Name);
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                sqlWhere.Append(" AND b.Status = @Status ");
                param.Add("Status", request.Status.ToInt(0));
            }

            if (!string.IsNullOrEmpty(request.Type))
            {
                sqlWhere.Append(" AND b.Type = @Type ");
                param.Add("Type", request.Type.ToInt(0));
            }

            if (!string.IsNullOrEmpty(request.FirstCateId))
            {
                sqlWhere.Append(" AND b.CategoryId = @FirstCateId ");
                param.Add("FirstCateId", request.FirstCateId.ToLong(0));
            }


            sql = string.Format(sql, sqlWhere.ToString());
            result = DapperHelper.GetPageList<MaterialFileViewModel>(sql, sort, request, param);
            return result;
        }


        public PageResult<MediaFileViewModel> GetFilesList(MediaFileRequest request)
        {
            PageResult<MediaFileViewModel> result = new PageResult<MediaFileViewModel>();
            string sql = @" SELECT * FROM [dbo].[MediaFile] where Id>0 {0} ";
            string sort = " [Id] DESC ";

            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder();

            if (request.MaterialId > 0)
            {
                sqlWhere.Append(" AND MaterialId = @MaterialId ");
                param.Add("MaterialId", request.MaterialId);
            }

            sql = string.Format(sql, sqlWhere.ToString());
            result = DapperHelper.GetPageList<MediaFileViewModel>(sql, sort, request, param);
            return result;

        }

        public string GetUserBelongCates(long userId)
        {
            string sql = @" SELECT [AccessIds] FROM [Account] WHERE Id=@Id ";
            string result = DapperHelper.Get<string>(sql, new { Id = userId });
            return result;
        }

        public int UpdateMediaFile(long materialId, List<long> fileIds)
        {
            string sql = @" UPDATE [dbo].[MediaFile] SET [MaterialId]=@materialId WHERE [Id] IN @fileIds ";
            return DapperHelper.Execute(sql, new { materialId = materialId, fileIds = fileIds });
        }

    }
}
