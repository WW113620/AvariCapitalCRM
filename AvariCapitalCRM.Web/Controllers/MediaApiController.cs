
using AvariCapitalCRM.Models.Data;
using AvariCapitalCRM.Models.Utils;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Web.Http;

namespace AvariCapitalCRM.Web.Controllers
{

    public class MediaApiController : ApiController
    {
        private static string MediaVideoPath = ConfigHelper.MediaVideoPath;
        private static string MediaImagePath = ConfigHelper.MediaImagePath;
        private static string MediaDistributorDocumentPath = ConfigHelper.MediaDistributorDocumentPath;

        private readonly DataContextEntities _dataContext;

        public MediaApiController(DataContextEntities dataContext)
        {
            this._dataContext = dataContext;
        }

        #region 图片

        [Route("api/media/image/{uploadType}/{fileName}")]
        [HttpGet]
        public HttpResponseMessage GetImage(int uploadType, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("Parameter error") };

            string filePath = MediaImagePath;
            if (uploadType == (int)UploadTypeEnums.经销商)
            {
                filePath = MediaDistributorDocumentPath;
            }
            string savePath = Path.Combine(filePath, fileName);

            if (!File.Exists(savePath))
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("The image does not exist or has been deleted") };
            byte[] fileBytes = ConvertHelper.FileToBytes(savePath);
            HttpResponseMessage result = new HttpResponseMessage(HttpStatusCode.OK);
            result.Content = new ByteArrayContent(fileBytes);
            result.Content.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");
            ContentDispositionHeaderValue cdh = new ContentDispositionHeaderValue("Inline");
            cdh.FileName = fileName;
            result.Content.Headers.ContentDisposition = cdh;
            return result;

        }
        #endregion

        #region 播放视频

        [Route("api/media/video/{uploadType}/{fileName}")]
        [HttpGet]
        public HttpResponseMessage GetVideo(int uploadType, string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("Parameter error") };

            string filePath = MediaVideoPath;
            if (uploadType == (int)UploadTypeEnums.经销商)
            {
                filePath = MediaDistributorDocumentPath;
            }
            string savePath = Path.Combine(filePath, fileName);
            if (!File.Exists(savePath))
                return new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent("The video does not exist or has been deleted") };

            string videoFormat = System.IO.Path.GetExtension(fileName);
            if (!string.IsNullOrEmpty(videoFormat))
                videoFormat = videoFormat.Trim('.');

            var mediaType = MediaTypeHeaderValue.Parse($"video/{videoFormat}");
            var stream = new FileStream(savePath, FileMode.Open, FileAccess.Read, FileShare.Read);

            if (Request.Headers.Range != null)
            {
                try
                {
                    var partialResponse = Request.CreateResponse(HttpStatusCode.PartialContent);
                    partialResponse.Content = new ByteRangeStreamContent(stream, Request.Headers.Range, mediaType);

                    return partialResponse;
                }
                catch (InvalidByteRangeException invalidByteRangeException)
                {
                    return Request.CreateErrorResponse(invalidByteRangeException);
                }
            }
            else
            {
                var fullResponse = Request.CreateResponse(HttpStatusCode.OK);

                fullResponse.Content = new StreamContent(stream);
                fullResponse.Content.Headers.ContentType = mediaType;
                return fullResponse;
            }

        }
        #endregion

    }
}
