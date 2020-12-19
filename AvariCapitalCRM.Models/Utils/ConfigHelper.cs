using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Utils
{
    public class ConfigHelper
    {
        public static string GetConfigValue(string sKey)
        {
            string sValue = null;
            if ((sValue = System.Configuration.ConfigurationManager.AppSettings[sKey]) == null)
            {
                sValue = "";
            }
            return sValue;
        }

        public static string JwtSecretKey = "QAZ123wsx456#@$";

        public static string UploadDocumentRoot = GetConfigValue("UploadDocumentRoot");

        public static string WebDomainAction = GetConfigValue("WebDomainAction").TrimEnd('/');


        public static string MediaVideoPath
        {
            get { return $"{UploadDocumentRoot}/Videos"; }
        }


        public static string MediaImagePath
        {
            get { return $"{UploadDocumentRoot}/Images"; }
        }

        public static string MediaDocumentPath
        {
            get { return $"{UploadDocumentRoot}/Documents"; }
        }

        public static string MediaDistributorDocumentPath
        {
            get { return $"{UploadDocumentRoot}/Distributor"; }
        }


        public static string GetImageUrl(string fileName, UploadTypeEnums uploadType)
        {
            return $"{WebDomainAction.TrimEnd('/')}/api/media/image/{(int)uploadType}/{fileName}";
        }


        public static string GetVideoUrl(string fileName, UploadTypeEnums uploadType)
        {
            return $"{WebDomainAction.TrimEnd('/')}/api/media/video/{(int)uploadType}/{fileName}";
        }

        public static string GetQiniuFileUrl(string fileName)
        {
            return $"{QiniuHelper.BtkDomain.TrimEnd('/')}/{fileName}";
        }

        public static string GetFileUrl(string fileName)
        {
            return $"{GetConfigValue("UploadDomain").TrimEnd('/')}/{fileName}";
        }


    }
}
