using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class DistributorMediaFileViewModel : DistributorMediaFile
    {
        public string UserName { get; set; }

        public string AddDateValue => AddTime.ToString("yyyy-MM-dd HH:mm");
        public string MediaTypeName => EnumHelper.GetDescription((MediaTypeEnums)MediaType);

        public string MediaViewUrl { get; set; }

        public string ShowSize => ConvertHelper.convertFileSize(MediaSize.ToLong(0));
    }

    public class DistributorMediaFileRequest : BaseReqestParams
    {
        public string FileName { get; set; }
        public string UserName { get; set; }
        public string LoginName { get; set; }
    }

}
