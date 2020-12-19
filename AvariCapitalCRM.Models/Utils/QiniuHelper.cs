using Qiniu.Common;
using Qiniu.Http;
using Qiniu.IO;
using Qiniu.IO.Model;
using Qiniu.RS;
using Qiniu.Util;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvariCapitalCRM.Models.Utils
{
    public class QiniuHelper
    {
        public readonly static string AccessKey = ConfigHelper.GetConfigValue("AccessKey");
        public readonly static string SecretKey = ConfigHelper.GetConfigValue("SecretKey");
        public readonly static string Bucket = ConfigHelper.GetConfigValue("Bucket");
        public readonly static string BtkDomain = ConfigHelper.GetConfigValue("BtkDomain");
        public readonly static int SExpireTime = ConfigHelper.GetConfigValue("SExpireTime").ToInt(3600);


        public readonly static string UploadDomain = ConfigHelper.GetConfigValue("UploadDomain");
        /// <summary>
        /// 字节上传普通
        /// </summary>
        /// <param name="materialId"></param>
        /// <returns></returns>
        public static HttpResult UploadQiniuFile(Stream sm, String saveName)
        {

            byte[] data = new byte[sm.Length];
            sm.Read(data, 0, (int)sm.Length);

            Mac mac = new Mac(AccessKey, SecretKey);

            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = Bucket;
            putPolicy.SetExpires(SExpireTime);
            putPolicy.InsertOnly = 1;

            // 生成上传凭证
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);


            Config.SetZone(ZoneID.US_North, false);
            FormUploader fu = new FormUploader();
            HttpResult result = fu.UploadData(data, saveName, token);
            return result;
        }



        /// <summary>
        /// 删除文件
        /// </summary>
        public static HttpResult DeleteFile(string filename)
        {
            try
            {
                Mac mac = new Mac(AccessKey, SecretKey);
                BucketManager bm = new BucketManager(mac);
                HttpResult result = bm.Delete(Bucket, filename);
                return result;
            }
            catch (Exception e)
            {
                return null;
            }

        }
        /// <summary>
        /// 生成上传凭证
        /// </summary>
        /// <returns></returns>
        public static string CreateToken()
        {
            Mac mac = new Mac(AccessKey, SecretKey);

            PutPolicy putPolicy = new PutPolicy();
            putPolicy.Scope = Bucket;
            putPolicy.SetExpires(SExpireTime);
            putPolicy.InsertOnly = 1;
            Config.SetZone(ZoneID.US_North, false);
            // 生成上传凭证
            string jstr = putPolicy.ToJsonString();
            string token = Auth.CreateUploadToken(mac, jstr);
            return token;
        }

    }
}
