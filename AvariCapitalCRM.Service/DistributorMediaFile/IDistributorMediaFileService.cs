using AvariCapitalCRM.Models.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Service.DistributorMediaFile
{

    public interface IDistributorMediaFileService : IDependency
    {
        PageResult<DistributorMediaFileViewModel> GetFilesList(DistributorMediaFileRequest request);
    }
}
