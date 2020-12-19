using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.ViewModels
{
    public class MaterialViewModel : Material
    {
        public string AddDateValue => AddTime.ToString("yyyy-MM-dd HH:mm");
        public long ThirdCateId { get; set; }
        public long SecondCateId { get; set; }
        public long FirstCateId { get; set; }

        public string ThirdCateName { get; set; }
        public string SecondCateName { get; set; }
        public string FirstCateName { get; set; }
        public string StatusName => EnumHelper.GetDescription((MaterialStatusEnums)Status);
        public string ShowCategoryName => $"{FirstCateName} -> {SecondCateName} -> {ThirdCateName}";
    }

    public class MaterialRequest : BaseReqestParams
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public long UserId { get; set; }
        public string Type { get; set; }
        public string FirstCateId { get; set; }
    }

    public class MaterialFileViewModel : MediaFile
    {
        public int Type { get; set; }
        public int MediaTypeName => ConvertHelper.GetMediaType(NewFileName);
        public string MaterialName { get; set; }
        public string CategoryName { get; set; }
        public string UserName { get; set; }

        public string AccessName => string.IsNullOrEmpty(CategoryName) ? UserName : CategoryName;

        public string AddDateValue => AddTime.ToString("yyyy-MM-dd HH:mm");
        public string MediaViewUrl => ConfigHelper.GetFileUrl(NewFileName);

        public string ShowSize => ConvertHelper.convertFileSize(MediaSize.ToLong(0));
    }

}
