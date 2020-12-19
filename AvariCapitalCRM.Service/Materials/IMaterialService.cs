using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.Materials
{

    public interface IMaterialService : IDependency
    {
        MaterialViewModel Get(long Id);

        PageResult<MaterialFileViewModel> GetList(MaterialRequest request,long userId, RoleEnums loginType, List<long> cateIdArray);

        PageResult<MediaFileViewModel> GetFilesList(MediaFileRequest request);

        string GetUserBelongCates(long userId);

        int UpdateMediaFile(long materialId, List<long> fileIds);
    }
}
