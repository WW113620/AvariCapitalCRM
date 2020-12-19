using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using AvariCapitalCRM.Models.ViewModels;
using AvariCapitalCRM.Service.Materials;
using AvariCapitalCRM.Web.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;

namespace AvariCapitalCRM.Web.Areas.Admin.Controllers
{
    public class MaterialsController : AdminBaseController
    {
        private static string MediaImagePath = ConfigHelper.MediaImagePath;
        private readonly DataContextEntities _dataContext;
        private readonly IMaterialService _materialService;
        private readonly RedisHelper _redisHelper;
        private readonly static int SExpireTime = ConfigHelper.GetConfigValue("SExpireTime").ToInt(0);

        public MaterialsController(DataContextEntities dataContext, IMaterialService materialService, RedisHelper redisHelper)
        {
            this._dataContext = dataContext;
            this._materialService = materialService;
            this._redisHelper = redisHelper;
        }

        #region Report
        public ActionResult Index(long userId = 0)
        {
            //if (base.AdminUserType != (int)RoleEnums.超级管理员 && base.AdminUserType != (int)RoleEnums.管理员)
            //    return Content("You do not have access to this page");

            var categoryList = this._dataContext.Categories.Where(p => p.ParentId == 0 && p.Level == 1).ToList();
            ViewBag.CategoryList = categoryList;

            ViewBag.UserId = userId;
            ViewBag.AdminUserType = base.AdminUserType;
            return View();
        }

        public ActionResult Distributor()
        {
            var categoryList = this._dataContext.Categories.Where(p => p.ParentId == 0 && p.Level == 1).ToList();
            ViewBag.CategoryList = categoryList;
            return View();
        }

        [HttpGet]
        public JsonResult GetList(MaterialRequest request)
        {
            LayuiPageResult<MaterialFileViewModel> result = new LayuiPageResult<MaterialFileViewModel>() { code = 1 };
            try
            {
                long userId = base.AdminUserId.ToLong(0);
                RoleEnums loginType = (RoleEnums)base.AdminUserType;
                List<long> cateIdArray = new List<long>();
                if (loginType == RoleEnums.顾问 || loginType == RoleEnums.客户 || request.UserId > 0)
                {
                    if (request.UserId > 0)
                        userId = request.UserId;

                    var cateIds = this._materialService.GetUserBelongCates(userId);
                    if (string.IsNullOrEmpty(cateIds))
                    {
                        result = new LayuiPageResult<MaterialFileViewModel>() { code = 0, msg = "No data", count = 0, data = new List<MaterialFileViewModel>() };
                        return Json(result, JsonRequestBehavior.AllowGet);
                    }
                    List<string> idList = cateIds.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries).ToList();
                    cateIdArray = idList.Select<string, long>(x => x.ToLong(0)).ToList();
                }

                var response = this._materialService.GetList(request, userId, loginType, cateIdArray);
                result = new LayuiPageResult<MaterialFileViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }


        public ActionResult Add(int type = (int)MaterialTypeEnums.投资报告)
        {
            var Categories = this._dataContext.Categories.Where(p => p.ParentId == 0 && p.Level == 1).ToList();
            ViewBag.Categories = Categories;
            MaterialViewModel model = new MaterialViewModel() { Id = 0, Type = type };
            return View(model);
        }

        public ActionResult ClientUpload(long clientId = 0)
        {
            var account = _dataContext.Accounts.Find(clientId);
            if (account == null || account.RoleType != (int)RoleEnums.客户)
                return Content("Parameter error");

            List<SelectOption> MaterialTypeList = EnumHelper.EnumToList<MaterialTypeEnums>();
            ViewBag.MaterialTypeList = MaterialTypeList;
            return View(account);
        }

        #region Upload
        [HttpPost]
        public JsonResult Upload()
        {
            HttpFileCollection Files = System.Web.HttpContext.Current.Request.Files;
            if (Files.Count > 0)
            {
                try
                {
                    var file = Files[0];
                    string fileName = file.FileName;
                    string fileExt = System.IO.Path.GetExtension(fileName).ToLower();

                    long fileSize = file.ContentLength;
                    if (fileSize >= 500 * 1024 * 1024)
                        return Json(new BaseResult(1, "The file size should not exceed 500MB"));

                    string saveName = string.Format("acm_{0}{1}", DateTime.Now.ToString("yyyyMMddHHmmssfff"), fileExt);
                    string url = CloudStorageUpload.UploadFileAsBlob(file.InputStream, saveName);
                    if (string.IsNullOrEmpty(url))
                    {
                        return Json(new BaseResult(1, "upload error"));
                    }

                    int mediaType = ConvertHelper.GetMediaType(fileName);

                    MediaFile media = new MediaFile()
                    {
                        MaterialId = 0,
                        MediaType = mediaType,
                        NewFileName = saveName,
                        OriginalFileName = fileName,
                        MediaSize = fileSize,
                        MediaSort = 1,
                        MediaCloudId = url,
                        AddTime = DateTime.Now
                    };
                    this._dataContext.MediaFiles.Add(media);
                    this._dataContext.SaveChanges();

                    return Json(new { code = 0, msg = "OK", Id = media.Id });

                }
                catch (Exception e)
                {
                    return Json(new BaseResult(1, e.Message));
                }
            }
            else
            {
                return Json(new BaseResult(1, "Please select the file to upload"));
            }
        }

        [HttpPost]
        public JsonResult SaveAdd(Material model, List<long> fileIds)
        {
            model.AddTime = DateTime.Now;
            model.Status = (int)MaterialStatusEnums.已上架;
            this._dataContext.Materials.Add(model);
            this._dataContext.SaveChanges();

            if (fileIds != null && fileIds.Count > 0)
            {
                this._materialService.UpdateMediaFile(model.Id, fileIds);
            }
            return Json(new { code = 0, materialId = model.Id });
        }
        #endregion

        #endregion

        #region Statement
        public ActionResult Statement(long userId = 0)
        {
            //if (base.AdminUserType != (int)RoleEnums.超级管理员 && base.AdminUserType != (int)RoleEnums.管理员)
            //    return Content("You do not have access to this page");

            ViewBag.UserId = userId;
            var categoryList = this._dataContext.Categories.Where(p => p.ParentId == 0 && p.Level == 1).ToList();
            ViewBag.CategoryList = categoryList;

            ViewBag.AdminUserType = base.AdminUserType;
            return View();
        }
        #endregion

        public ActionResult View(long? id)
        {
            var categoryList = this._dataContext.Categories.Where(p => p.ParentId == 0 && p.Level == 1).ToList();
            ViewBag.CategoryList = categoryList;

            MaterialViewModel model = new MaterialViewModel() { Id = 0 };
            long Id = id.ToLong(0);
            model = this._materialService.Get(Id);
            if (model == null)
                return Content("Parameters error");

            if (model.Status != (int)MaterialStatusEnums.已上架)
                return View("Add", model);

            return View(model);
        }


        [HttpPost]
        public JsonResult Delete(long id)
        {
            var model = this._dataContext.Materials.Find(id);
            if (model != null)
            {
                if (model.Status == (int)MaterialStatusEnums.已上架)
                    return Json(new BaseResult(1, "It is on line and cannot be deleted"));

                this._dataContext.Materials.Remove(model);
                this._dataContext.SaveChanges();
            }
            return Json(new BaseResult(0, "OK"));
        }

        [HttpPost]
        public JsonResult UpdateMaterialsStatus(long id, int status)
        {
            var model = this._dataContext.Materials.Find(id);
            if (model == null)
                return Json(new BaseResult(1, "The naterial does not exist"));

            if (!Enum.IsDefined(typeof(MaterialStatusEnums), status))
                return Json(new BaseResult(1, "Abnormal state"));

            var nowStatusEnums = status == (int)MaterialStatusEnums.编辑中 || status == (int)MaterialStatusEnums.已下架 ? MaterialStatusEnums.已上架 : MaterialStatusEnums.已下架;
            if (nowStatusEnums == MaterialStatusEnums.已上架)
            {
                int count = this._dataContext.MediaFiles.Count(p => p.MaterialId == id);
                if (count <= 0)
                    return Json(new BaseResult(1, "Please upload videos, pictures or documents first"));
            }

            model.Status = (int)nowStatusEnums;
            this._dataContext.SaveChanges();
            return Json(new BaseResult(0, "OK"));
        }

        [HttpPost]
        public JsonResult GetCateListByParentId(long parentId)
        {
            var categories = this._dataContext.Categories.Where(p => p.ParentId == parentId).ToList();
            if (categories == null)
                return Json(new { code = 2 });

            return Json(new { code = 0, data = categories });
        }

        [HttpPost]
        public JsonResult Submit(long Id, string Name, string Description, long FirstCateId, long SecondCateId, long ThirdCateId)
        {
            if (string.IsNullOrEmpty(Name))
                return Json(new { code = 1, msg = "The naterial name cannot be empty" });

            if (ThirdCateId <= 0)
                return Json(new { code = 1, msg = "The third category cannot be empty" });

            Material model = new Material();
            if (Id > 0)
            {
                var material = this._dataContext.Materials.FirstOrDefault(p => p.Id == Id);
                if (material == null)
                    return Json(new { code = 1, msg = "The naterial does not exist" });

                model.Id = material.Id;

                material.Name = Name.Trim();
                material.Description = Description.Trim();
                material.CategoryId = ThirdCateId;
                this._dataContext.SaveChanges();
            }
            else
            {
                model.Name = Name.Trim();
                model.Description = Description.Trim();
                model.CategoryId = ThirdCateId;
                model.AddTime = DateTime.Now;
                model.Status = (int)MaterialStatusEnums.编辑中;
                this._dataContext.Materials.Add(model);
                this._dataContext.SaveChanges();
            }
            return Json(new { code = 0, materialId = model.Id });
        }

        public PartialViewResult PartialMediaFile(long id)
        {
            var model = this._materialService.Get(id);
            if (model.Status == (int)MaterialStatusEnums.已上架)
                return PartialView("ViewPartialMediaFile", model);

            return PartialView(model);
        }

        public PartialViewResult ViewPartialMediaFile(long id)
        {
            var model = this._materialService.Get(id);
            if (model.Status != (int)MaterialStatusEnums.已上架)
                return PartialView("PartialMediaFile", model);

            return PartialView(model);
        }

        [HttpGet]
        public JsonResult GetFilesList(MediaFileRequest request)
        {
            LayuiPageResult<MediaFileViewModel> result = new LayuiPageResult<MediaFileViewModel>() { code = 1 };
            try
            {
                var response = this._materialService.GetFilesList(request);
                foreach (var item in response.data)
                {
                    //if (item.MediaType == (int)MediaTypeEnums.图片)
                    //{
                    //    item.MediaViewUrl = ConfigHelper.GetImageUrl(item.NewFileName, UploadTypeEnums.管理员);
                    //}
                    //else if (item.MediaType == (int)MediaTypeEnums.视频)
                    //{
                    //    item.MediaViewUrl = ConfigHelper.GetVideoUrl(item.NewFileName, UploadTypeEnums.管理员);
                    //}
                    //item.OriginalFileName = System.IO.Path.GetFileNameWithoutExtension(item.OriginalFileName);
                    item.MediaViewUrl = ConfigHelper.GetFileUrl(item.NewFileName);
                }
                result = new LayuiPageResult<MediaFileViewModel>() { code = response.code, msg = response.msg, count = response.page.count, data = response.data };
            }
            catch (Exception e)
            {
                result.msg = e.Message;
            }
            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult DeleteFile(long id)
        {
            var model = this._dataContext.MediaFiles.Find(id);
            if (model != null)
            {
                int count = MediaFilesCount(model.MaterialId);
                if (count <= 1)
                {
                    DeleteMaterial(model.MaterialId);
                }
                CloudStorageUpload.DeleteFileAsBlob(model.NewFileName);

                this._dataContext.MediaFiles.Remove(model);
                this._dataContext.SaveChanges();

            }
            return Json(new BaseResult(0, "OK"));
        }


        private int MediaFilesCount(long materialId)
        {
            return this._dataContext.MediaFiles.Count(p => p.MaterialId == materialId);
        }

        private void DeleteMaterial(long materialId)
        {
            try
            {
                var model = this._dataContext.Materials.Find(materialId);
                if (model != null)
                {
                    this._dataContext.Materials.Remove(model);
                    this._dataContext.SaveChanges();
                }
            }
            catch (Exception e)
            {
            }
        }


        public ActionResult Play(long id)
        {
            var model = this._dataContext.MediaFiles.Find(id);
            if (model == null || string.IsNullOrEmpty(model.NewFileName))
                return Content("The video does not exist or has been deleted");

            int mediaType = ConvertHelper.GetMediaType(model.NewFileName);
            if (mediaType != (int)MediaTypeEnums.视频)
                return Content("Not a video file");

            ViewBag.VideoUrl = ConfigHelper.GetFileUrl(model.NewFileName);
            model.OriginalFileName = System.IO.Path.GetFileNameWithoutExtension(model.OriginalFileName);
            return View(model);
        }


        #region Download

        public ActionResult DownloadCloud(long id)
        {
            var model = this._dataContext.MediaFiles.Find(id);
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
                    int lengthRead = srm.Read(buffer, 0, Convert.ToInt32(ChunkSize));
                    Response.OutputStream.Write(buffer, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                Response.Close();

                AddDownloadLog(new DownloadLog
                {
                    UserId = base.AdminUserId.ToLong(0),
                    UserName = base.AdminUserName,
                    MaterialId = model.MaterialId,
                    MediaFileId = model.Id,
                    MediaFileName = model.OriginalFileName,
                    AddTime = DateTime.Now
                });

                return new EmptyResult();
            }
            catch (Exception e)
            {
                return Content("下载出错：" + e.Message);
            }
        }

        public ActionResult Download(long id)
        {
            var model = this._dataContext.MediaFiles.Find(id);
            if (model == null || string.IsNullOrEmpty(model.NewFileName))
                return Content("The file does not exist or has been deleted");

            string MediaVideoPath = ConfigHelper.MediaVideoPath;
            string MediaImagePath = ConfigHelper.MediaImagePath;
            string MediaDocumentPath = ConfigHelper.MediaDocumentPath;


            MediaTypeEnums mediaType = (MediaTypeEnums)model.MediaType;
            string savePath = string.Empty;
            if (mediaType == MediaTypeEnums.视频)
            {
                savePath = Path.Combine(ConfigHelper.MediaVideoPath, model.NewFileName);
            }
            else if (mediaType == MediaTypeEnums.图片)
            {
                savePath = Path.Combine(ConfigHelper.MediaImagePath, model.NewFileName);
            }
            else if (mediaType == MediaTypeEnums.文档)
            {
                savePath = Path.Combine(ConfigHelper.MediaDocumentPath, model.NewFileName);
            }

            if (!System.IO.File.Exists(savePath))
            {
                return Content("The file does not exist or has been deleted");
            }

            System.IO.FileInfo fileInfo = new System.IO.FileInfo(savePath);
            if (fileInfo.Exists == true)
            {
                string fileName = model.OriginalFileName;
                const long ChunkSize = 102400;//100K 每次读取文件，只读取100K，这样可以缓解服务器的压力

                byte[] buffer = new byte[ChunkSize];
                Response.Clear();
                System.IO.FileStream iStream = System.IO.File.OpenRead(savePath);
                long dataLengthToRead = iStream.Length;//获取下载的文件总大小
                Response.ContentType = "application/octet-stream";
                Response.AddHeader("Content-Disposition", "attachment; filename=" + fileName);
                while (dataLengthToRead > 0 && Response.IsClientConnected)
                {
                    int lengthRead = iStream.Read(buffer, 0, Convert.ToInt32(ChunkSize));//读取的大小
                    Response.OutputStream.Write(buffer, 0, lengthRead);
                    Response.Flush();
                    dataLengthToRead = dataLengthToRead - lengthRead;
                }
                Response.Close();

                AddDownloadLog(new DownloadLog
                {
                    UserId = base.AdminUserId.ToLong(0),
                    UserName = base.AdminUserName,
                    MaterialId = model.MaterialId,
                    MediaFileId = model.Id,
                    MediaFileName = model.OriginalFileName,
                    AddTime = DateTime.Now
                });
                return new EmptyResult();
            }
            else
            {
                return Content("The file does not exist or has been deleted");
            }

        }

        public void AddDownloadLog(DownloadLog log)
        {
            try
            {
                this._dataContext.DownloadLogs.Add(log);
                this._dataContext.SaveChanges();
            }
            catch (Exception e)
            {
            }

        }
        #endregion


    }
}