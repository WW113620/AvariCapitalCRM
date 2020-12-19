using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Service.DistributorMediaFile;
using AvariCapitalCRM.Web.Models;
using Qiniu.Http;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    public class DistributorUploadController : AdminBaseController
    {
        private readonly DataContextEntities _dataContext;
        private readonly IDistributorMediaFileService _fileService;

        public DistributorUploadController(DataContextEntities dataContext, IDistributorMediaFileService fileService)
        {
            this._dataContext = dataContext;
            this._fileService = fileService;
        }


        #region List
        public ActionResult Index()
        {
            return RedirectToAction("List");
        }

        public ActionResult List()
        {
            return View();
        }

        [HttpGet]
        public JsonResult GetList(DistributorMediaFileRequest request)
        {
            LayuiPageResult<DistributorMediaFileViewModel> result = new LayuiPageResult<DistributorMediaFileViewModel>() { code = 1 };
            try
            {
                int loginType = base.AdminUserType;
                if (loginType == (int)RoleEnums.超级管理员 || loginType == (int)RoleEnums.管理员)
                    request.LoginName = "";
                else
                    request.LoginName = base.AdminUserName;

                var response = this._fileService.GetFilesList(request);
                foreach (var item in response.data)
                {
                    //if (item.MediaType == (int)MediaTypeEnums.图片)
                    //{
                    //    item.MediaViewUrl = ConfigHelper.GetImageUrl(item.NewFileName, UploadTypeEnums.经销商);
                    //}
                    //else if (item.MediaType == (int)MediaTypeEnums.视频)
                    //{
                    //    item.MediaViewUrl = ConfigHelper.GetVideoUrl(item.NewFileName, UploadTypeEnums.经销商);
                    //}
                    item.MediaViewUrl = ConfigHelper.GetFileUrl(item.NewFileName);
                    // item.OriginalFileName = System.IO.Path.GetFileNameWithoutExtension(item.OriginalFileName);
                }
                result = new LayuiPageResult<DistributorMediaFileViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult Delete(long id)
        {
            var model = this._dataContext.DistributorMediaFiles.Find(id);
            if (model != null)
            {
                CloudStorageUpload.DeleteFileAsBlob(model.NewFileName);
                this._dataContext.DistributorMediaFiles.Remove(model);
                this._dataContext.SaveChanges();
            }
            return Json(new BaseResult(0, "OK"));
        }

        public ActionResult Upload()
        {
            return View();
        }
        #endregion

        #region Upload File

        [HttpPost]
        public JsonResult UploadFile(string remark)
        {
            HttpFileCollection Files = System.Web.HttpContext.Current.Request.Files;
            if (Files.Count > 0)
            {
                try
                {
                    var file = Files[0];
                    string fileName = file.FileName;
                    string fileExt = System.IO.Path.GetExtension(fileName).ToLower();

                    int MediaType = (int)MediaTypeEnums.文档;

                    string[] fileFilt = { ".gif", ".jpg", ".jpeg", ".png" };
                    string[] fileVideoFilt = { ".mp4", ".rmvb", ".avi", ".mpeg" };
                    string[] fileDocumentFilt = { ".zip", ".rar", ".pdf", ".txt", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx" };
                    if (fileFilt.Contains(fileExt))
                    {
                        MediaType = (int)MediaTypeEnums.图片;
                    }
                    else if (fileVideoFilt.Contains(fileExt))
                    {
                        MediaType = (int)MediaTypeEnums.视频;
                    }
                    else if (fileDocumentFilt.Contains(fileExt))
                    {
                        MediaType = (int)MediaTypeEnums.文档;
                    }
                    else
                    {
                        return Json(new BaseResult(1, "Uploading files with this suffix is not supported"));
                    }

                    long fileSize = file.ContentLength;
                    if (fileSize >= 200 * 1024 * 1024)
                        return Json(new BaseResult(1, "Size should not exceed 200MB"));

                    string saveName = string.Format("md_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), fileExt);

                    //文档保存七牛
                    //HttpResult result = QiniuHelper.UploadQiniuFile(file.InputStream, saveName);
                    //if (result != null && result.Code != 200)
                    //{
                    //    return Json(new BaseResult(1, result.Text));
                    //}

                    string url = CloudStorageUpload.UploadFileAsBlob(file.InputStream, saveName);
                    if (string.IsNullOrEmpty(url))
                    {
                        return Json(new BaseResult(1, "upload error"));
                    }

                    //string saveDirectory = ConfigHelper.MediaDistributorDocumentPath;
                    //if (!System.IO.Directory.Exists(saveDirectory))
                    //    Directory.CreateDirectory(saveDirectory);
                    //string savePath = Path.Combine(saveDirectory, saveName);
                    //file.SaveAs(savePath);

                    DistributorMediaFile media = new DistributorMediaFile()
                    {
                        UserId = base.AdminUserId.ToLong(0),
                        MediaType = MediaType,
                        NewFileName = saveName,
                        OriginalFileName = fileName,
                        MediaSize = fileSize,
                        MediaSort = 1,
                        MediaCloudId = url,
                        Remark = remark,
                        AddTime = DateTime.Now
                    };
                    AddMediaFile(media);
                    return Json(new BaseResult(0, saveName));

                }
                catch (Exception e)
                {
                    return Json(new BaseResult(1, e.Message));
                }
            }
            else
            {
                return Json(new BaseResult(1, "Please select the document to upload"));
            }
        }

        public void AddMediaFile(DistributorMediaFile media)
        {
            this._dataContext.DistributorMediaFiles.Add(media);
            this._dataContext.SaveChanges();
        }
        #endregion

        #region Admin
        public ActionResult AdminList()
        {
            if (base.AdminUserType != (int)RoleEnums.超级管理员&& base.AdminUserType != (int)RoleEnums.管理员)
                return Content("You do not have access to this page");

            return View();
        }
        #endregion

        #region Download
        public ActionResult Download(long id)
        {
            var model = this._dataContext.DistributorMediaFiles.Find(id);
            if (model == null || string.IsNullOrEmpty(model.NewFileName))
                return Content("The file does not exist or has been deleted");

            string fileUrl = ConfigHelper.GetFileUrl(model.NewFileName);
            string fileName = model.OriginalFileName;

            try
            {
                const long ChunkSize = 102400;
                byte[] buffer = new byte[ChunkSize];

                Response.Clear();
                WebClient wcClient = new WebClient();
                WebRequest webReq = WebRequest.Create(fileUrl);
                WebResponse webRes = webReq.GetResponse();
                long dataLengthToRead = webRes.ContentLength;
                Stream srm = webRes.GetResponseStream();
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                while (dataLengthToRead > 0 && Response.IsClientConnected)
                {
                    int lengthRead = srm.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
                    Response.OutputStream.Write(buffer, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                Response.Close();

                return new EmptyResult();
            }
            catch (Exception e)
            {
                return Content("下载出错：" + e.Message);
            }
        }

     

        public ActionResult Play(long id)
        {
            var model = this._dataContext.DistributorMediaFiles.Find(id);
            if (model == null || string.IsNullOrEmpty(model.NewFileName))
                return Content("The video does not exist or has been deleted");

            if (model.MediaType != (int)MediaTypeEnums.视频)
                return Content("Not a video file");

            ViewBag.VideoUrl = ConfigHelper.GetFileUrl(model.NewFileName);
            model.OriginalFileName = System.IO.Path.GetFileNameWithoutExtension(model.OriginalFileName);
            return View(model);
        }
        #endregion

    }
}