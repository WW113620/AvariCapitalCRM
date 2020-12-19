
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Models.ViewModels.Charts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.BaseCommon
{
    public interface ICommonService:IDependency
    {
      
        List<HomeChartModels> GetMediaFileLineData();
        List<KeyValue> GetMaterialPieData();
        List<ChartModels> GetUserLineData();

        PageResult<DownloadStatisticsModel> GetDownloadStatistics(BaseReqestParams request,string userName, RoleEnums loginType);

        bool ExistDiscountCategoryUserId(long UserId, long CategoryId);
        bool DeleteUserDiscountCategorys(long userId);
        bool UserDiscountCategorys(long userId, List<AccountViewModel> cates);
    }
}
