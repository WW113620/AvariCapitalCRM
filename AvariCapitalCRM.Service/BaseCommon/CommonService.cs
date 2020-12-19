using Dapper;
using AvariCapitalCRM.Models.Dapper;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Models.ViewModels.Charts;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.BaseCommon
{
    public class CommonService : ICommonService
    {

        public List<HomeChartModels> GetMediaFileLineData()
        {
            string sql = @" SELECT CONVERT(varchar(100), [AddTime], 23) AS ShowDate,
                            SUM(CASE WHEN MediaType IN (1) THEN 1 ELSE 0 END) as VideoCount,
                            SUM(CASE WHEN MediaType IN (3) THEN 1 ELSE 0 END) as PictureCount,
                            SUM(CASE WHEN MediaType IN (4) THEN 1 ELSE 0 END) as DocumentCount
                            FROM [dbo].[MediaFile]
                            WHERE MaterialId>0 AND [AddTime] BETWEEN  DATEADD(month,-1, CONVERT(varchar(100), @NowDate, 23)) AND @NowDate
                            GROUP BY CONVERT(varchar(100), [AddTime], 23) ";
            return DapperHelper.Query<HomeChartModels>(sql, new { NowDate = DateTime.Now }).ToList();
        }

        public List<KeyValue> GetMaterialPieData()
        {
            string sql = @"	SELECT t.CategoryName as [name],COUNT(0) as [value] FROM (
							SELECT a.*,b.Name as MaterialName,b.Type,c.Name as CategoryName
							FROM [MediaFile] as a
						    INNER JOIN [Materials] as b ON a.MaterialId=b.Id
							LEFT JOIN [Category] as c ON b.CategoryId=c.Id WHERE a.Id>0 ) as t
							GROUP BY t.CategoryName ";
            return DapperHelper.Query<KeyValue>(sql).ToList();
        }

        public List<ChartModels> GetUserLineData()
        {
            string sql = @" SELECT CONVERT(varchar(100), [AddTime], 23) AS ShowDate,
                            SUM(CASE WHEN RoleType IN (3,4) THEN 1 ELSE 0 END) as UserCount
                            FROM [dbo].[Account] as a
                            WHERE [AddTime] BETWEEN  DATEADD(month,-1, CONVERT(varchar(100), @NowDate , 23)) AND @NowDate
                            GROUP BY CONVERT(varchar(100), [AddTime], 23)  ";
            return DapperHelper.Query<ChartModels>(sql, new { NowDate = DateTime.Now }).ToList();
        }



        public PageResult<DownloadStatisticsModel> GetDownloadStatistics(BaseReqestParams request, string userName, RoleEnums loginType)
        {
            PageResult<DownloadStatisticsModel> result = new PageResult<DownloadStatisticsModel>();
            string sql = @" SELECT * FROM (
                            SELECT MediaFileName,UserName,Count(0) as DownloadCount
                            FROM [dbo].[DownloadLog]
                            WHERE Id>0 {0}
                            GROUP BY MediaFileName,UserName ) as t ";

            string sort = " DownloadCount ASC ";
            DynamicParameters param = new DynamicParameters();
            StringBuilder sqlWhere = new StringBuilder();
            if (loginType == RoleEnums.超级管理员 || loginType == RoleEnums.管理员)
            {
            }
            else
            {
                sqlWhere.Append(" AND UserName=@loginName  ");
                param.Add("loginName", userName);
            }

            sql = string.Format(sql, sqlWhere.ToString());
            result = DapperHelper.GetPageList<DownloadStatisticsModel>(sql, sort, request, param);
            return result;

        }


        public bool ExistDiscountCategoryUserId(long UserId, long CategoryId)
        {
            string sql = @" SELECT COUNT(0) AS iCount FROM [dbo].CateDisCountRelate WHERE UserId=@UserId AND CategoryId=@CategoryId ";
            int i = DapperHelper.Get<int>(sql, new { UserId = UserId, CategoryId = CategoryId });
            return i > 0;
        }

        public bool DeleteUserDiscountCategorys(long userId)
        {
            string sql = @" DELETE FROM CateDisCountRelate WHERE UserId=@UserId ";
            int isDel = DapperHelper.Execute(sql, new { UserId = userId });
            return isDel > 0;
        }


        public bool UserDiscountCategorys(long userId, List<AccountViewModel> cates)
        {
            using (IDbConnection conn = DapperHelper.GetConnection(string.Empty))
            {
                conn.Open();
                var transaction = conn.BeginTransaction();
                try
                {
                    #region 删除
                    string sql = @" DELETE FROM CateDisCountRelate WHERE UserId=@UserId ";
                    int isDel = DapperHelper.Execute(sql, new { UserId = userId }, transaction);

                    #endregion

                    #region 插入

                    string sqlText = @" INSERT INTO CateDisCountRelate
                                               (UserId
                                               ,CategoryId
                                               ,DisCountType
                                               ,DisCountVal
                                               ,AddTime
                                               )
                                         VALUES
                                    {0} ";
                    var whereText = string.Empty;
                    var whereStrList = new List<string>();
                    var parameters = new DynamicParameters();
                    for (int i = 0; i < cates.Count; i++)
                    {
                        whereStrList.Add($"(@UserId{i} ,@CategoryId{i} ,@DisCountType{i} ,@DisCountVal{i} ,@AddTime{i})");

                        //parameters.Add($"UserId{i}", cates[i].UserId);
                        //parameters.Add($"CategoryId{i}", cates[i].CategoryId);
                        //parameters.Add($"DisCountType{i}", cates[i].DisCountType);
                        //parameters.Add($"DisCountVal{i}", cates[i].DisCountVal);
                        //parameters.Add($"AddTime{i}", cates[i].AddTime);
                    }
                    whereText = string.Join(",", whereStrList);
                    sqlText = string.Format(sqlText, whereText);
                    int result = DapperHelper.Execute(sqlText, parameters, transaction);
                    if (result <= 0)
                    {
                        transaction.Rollback();
                        return false;
                    }

                    #endregion

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    transaction.Rollback();
                    return false;
                }
            }
        }

    }
}
