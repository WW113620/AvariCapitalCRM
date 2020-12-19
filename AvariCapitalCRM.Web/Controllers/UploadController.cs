using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Web.Models;
using Qiniu.Http;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AvariCapitalCRM.Web.Controllers
{
    public class UploadController : Controller
    {
        private static string MediaVideoPath = ConfigHelper.MediaVideoPath;
        private static string MediaImagePath = ConfigHelper.MediaImagePath;
        private static string MediaDocumentPath = ConfigHelper.MediaDocumentPath;

        private readonly DataContextEntities _dataContext;

        public UploadController(DataContextEntities dataContext)
        {
            this._dataContext = dataContext;
        }


        public ActionResult Index()
        {
            return View();
        }

        #region Images
        [HttpPost]
        public JsonResult UploadImage(long materialId = 0)
        {
            HttpFileCollection Files = System.Web.HttpContext.Current.Request.Files;
            if (Files.Count > 0)
            {
                try
                {
                    var file = Files[0];
                    string fileName = file.FileName;
                    string fileExt = System.IO.Path.GetExtension(fileName).ToLower();
                    string[] fileFilt = { ".gif", ".jpg", ".jpeg", ".png" };
                    if (!fileFilt.Contains(fileExt))
                        return Json(new BaseResult(1, "Please upload images in JPG, JPEG, GIF, PNG format"));

                    long fileSize = file.ContentLength;
                    if (fileSize >= 500 * 1024 * 1024)
                        return Json(new BaseResult(1, "The image size should not exceed 500MB"));

                    string saveName = string.Format("ma_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), fileExt);

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

                    //图片保存到本地
                    //string saveDirectory = MediaImagePath;
                    //if (!System.IO.Directory.Exists(saveDirectory))
                    //    Directory.CreateDirectory(saveDirectory);
                    //string savePath = Path.Combine(saveDirectory, saveName);
                    //file.SaveAs(savePath);

                    if (materialId > 0)
                    {
                        MediaFile media = new MediaFile()
                        {
                            MaterialId = materialId,
                            MediaType = (int)MediaTypeEnums.图片,
                            NewFileName = saveName,
                            OriginalFileName = fileName,
                            MediaSize = fileSize,
                            MediaSort = 1,
                            MediaCloudId = url,
                            AddTime = DateTime.Now
                        };
                        AddMedia(media);
                    }
                    return Json(new BaseResult(0, saveName));

                }
                catch (Exception e)
                {
                    return Json(new BaseResult(1, e.Message));
                }
            }
            else
            {
                return Json(new BaseResult(1, "Please select the image to upload"));
            }
        }

        #endregion

        #region Document

        [HttpPost]
        public JsonResult UploadDocument(long materialId = 0)
        {
            HttpFileCollection Files = System.Web.HttpContext.Current.Request.Files;
            if (Files.Count > 0)
            {
                try
                {
                    var file = Files[0];
                    string fileName = file.FileName;
                    string fileExt = System.IO.Path.GetExtension(fileName).ToLower();
                    string[] fileFilt = { ".zip", ".rar", ".pdf", ".txt", ".doc", ".docx", ".ppt", ".pptx", ".xls", ".xlsx" };
                    if (!fileFilt.Contains(fileExt))
                        return Json(new BaseResult(1, "Please upload documents in ZIP,RAR,PDF, DOC, DOCX, TXT, PPT, PPTX, XLS, XLSX format"));

                    long fileSize = file.ContentLength;
                    if (fileSize >= 500 * 1024 * 1024)
                        return Json(new BaseResult(1, "The document size should not exceed 500MB"));

                    string saveName = string.Format("ma_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), fileExt);

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

                    //文档保存七牛
                    //string saveDirectory = MediaDocumentPath;
                    //if (!System.IO.Directory.Exists(saveDirectory))
                    //    Directory.CreateDirectory(saveDirectory);
                    //string savePath = Path.Combine(saveDirectory, saveName);
                    //file.SaveAs(savePath);

                    if (materialId > 0)
                    {
                        MediaFile media = new MediaFile()
                        {
                            MaterialId = materialId,
                            MediaType = (int)MediaTypeEnums.文档,
                            NewFileName = saveName,
                            OriginalFileName = fileName,
                            MediaSize = fileSize,
                            MediaSort = 1,
                            MediaCloudId = url,
                            AddTime = DateTime.Now
                        };
                        AddMedia(media);
                    }
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

        #endregion

        #region Videos

        [HttpPost]
        public JsonResult UploadVideo(long materialId = 0)
        {
            HttpFileCollection Files = System.Web.HttpContext.Current.Request.Files;
            if (Files.Count > 0)
            {
                try
                {
                    var file = Files[0];
                    string fileName = file.FileName;
                    string fileExt = System.IO.Path.GetExtension(fileName).ToLower();
                    string[] fileFilt = { ".mp4", ".rmvb", ".avi", ".mpeg" };
                    if (!fileFilt.Contains(fileExt))
                        return Json(new BaseResult(1, "Please upload documents in MP4,RMVB,AVI,MPEG format"));

                    long fileSize = file.ContentLength;
                    if (fileSize >= 1000 * 1024 * 1024)
                        return Json(new BaseResult(1, "The video size should not exceed 1000MB"));

                    string saveName = string.Format("ma_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), fileExt);

                    string url = CloudStorageUpload.UploadFileAsBlob(file.InputStream, saveName);
                    if (string.IsNullOrEmpty(url))
                    {
                        return Json(new BaseResult(1, "upload error"));
                    }

                    if (materialId > 0)
                    {
                        MediaFile media = new MediaFile()
                        {
                            MaterialId = materialId,
                            MediaType = (int)MediaTypeEnums.视频,
                            NewFileName = saveName,
                            OriginalFileName = fileName,
                            MediaSize = fileSize,
                            MediaSort = 1,
                            MediaCloudId = url,
                            AddTime = DateTime.Now
                        };
                        AddMedia(media);
                    }
                    return Json(new BaseResult(0, saveName));

                }
                catch (Exception e)
                {
                    return Json(new BaseResult(1, e.Message));
                }
            }
            else
            {
                return Json(new BaseResult(1, "Please select the video to upload"));
            }
        }

        //[HttpPost]
        //public JsonResult UploadVideo(long materialId, string fileName, string newFileName, long size)
        //{
        //    if (materialId > 0)
        //    {
        //        MediaFile media = new MediaFile()
        //        {
        //            MaterialId = materialId,
        //            MediaType = (int)MediaTypeEnums.视频,
        //            NewFileName = newFileName,
        //            OriginalFileName = fileName,
        //            MediaSize = size,
        //            MediaSort = 1,
        //            MediaCloudId = "",
        //            AddTime = DateTime.Now
        //        };
        //        AddMedia(media);
        //    }
        //    return Json(new BaseResult(0, newFileName));
        //}
        #endregion

        #region Methods
        public void AddMedia(MediaFile media)
        {
            this._dataContext.MediaFiles.Add(media);
            this._dataContext.SaveChanges();
        }
        #endregion

    }
}

