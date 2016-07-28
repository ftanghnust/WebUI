using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.SessionState;
using Frxs.Platform.Utility;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Data
{
    /// <summary>
    /// 文件上传处理页
    /// </summary>
    public class upload_ajax : IHttpHandler, IRequiresSessionState
    {
        public void ProcessRequest(HttpContext context)
        {
            UpLoadFile(context);
        }

        #region 上传文件处理===================================
        private void UpLoadFile(HttpContext context)
        {
            //HttpPostedFile upfile = context.Request.Files["Filedata"];
            HttpPostedFile upfile = context.Request.Files[0];
            var savemethod = context.Request["savemethod"];

            if (upfile == null)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"请选择要上传文件！\"}");
                return;
            }

            string msg = "";
            try
            {
                var fileSize = upfile.ContentLength; //获得文件大小，以字节为单位
                if (fileSize > 1024 * 1000 * 1000)
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"文件太大上传失败！\"}");
                    return;
                }

                var fileExt = Utils.GetFileExt(upfile.FileName); //文件扩展名，不含“.”

                //如果是运营分类验证格式
                if (savemethod == "SaveCategoryImages" && fileExt.ToLower() != "jpg" && fileExt.ToLower() != "jpeg")
                {
                    context.Response.Write("{\"status\": 0, \"msg\": \"错误：仅限jpg/jpeg格式图片！\"}");
                    return;
                }

                var fileName = Utils.GetRamCode() + "." + fileExt; //随机生成新的文件名


                //图片临时保存地址
                var tempPath = context.Server.MapPath("../upLoad");

                //判断是否存在upload文件夹
                if (!Directory.Exists(tempPath))
                {
                    Directory.CreateDirectory(tempPath);
                }

                upfile.SaveAs(tempPath + "/" + fileName);

                //如果是橱窗推荐图片尺寸
                //if (savemethod == "SaveWadvertisementImages")
                //{
                //    System.Drawing.Image img = System.Drawing.Image.FromFile(tempPath + "/" + fileName);
                //    if (img.Size.Height != 48 || img.Size.Width != 48)
                //    {
                //        context.Response.Write("{\"status\": 0, \"msg\": \"错误：橱窗推荐图片尺寸应为48*48px！\",\"filePath\":\"\"}");
                //        img.Dispose();
                //        //删除临时保存的图片
                //        File.Delete(tempPath + "/" + fileName);
                //        return;
                //    }
                //    img.Dispose();
                //}


                //重构一个URL
                // var url = context.Request.Url.AbsoluteUri.Substring(0, context.Request.Url.AbsoluteUri.IndexOf("/Data/", StringComparison.Ordinal)) + "/upLoad" + "/" + fileName;
                //context.Response.Write("{\"filePath\":\"" + url + "\"}");
                //重构一个URL。因为生产环境站点对内有端口，对外访问则通过域名，所以域名+端口将无法访问，所以去除端口 Modify By CR 2016-5-20
                var url = context.Request.Url.AbsoluteUri.Substring(0, context.Request.Url.AbsoluteUri.IndexOf("/Data/", StringComparison.Ordinal)) + "/upLoad" + "/" + fileName;
                var port = context.Request.Url.Port;
                url = url.Replace(string.Format(":{0}", port), "");
                context.Response.Write("{\"status\": 1, \"msg\": \"上传成功\",\"filePath\":\"" + url + "\"}");

                ////远程保存地址
                //var strUrl = ConfigurationManager.AppSettings["imageSvrUrl"].Trim('/') + "/" + savemethod + "?url=" + url;

                ////模拟发送请求
                //WebRequest request = WebRequest.Create(strUrl);
                //request.Method = "GET";
                //WebResponse response = request.GetResponse();
                //var resp = response.GetResponseStream();
                //if (resp != null)
                //{
                //    //获取字符集编码
                //    string coder = ((HttpWebResponse)response).CharacterSet;
                //    if (coder != null)
                //    {
                //        var reader = new StreamReader(resp, Encoding.GetEncoding(coder));
                //        var callbackmessage = reader.ReadToEnd();

                //        //解析返回结果
                //        var obj = Frxs.Platform.Utility.Json.JsonHelper.FromJson<ImageServer>(callbackmessage);
                //        if (obj.Flag == "SUCCESS")
                //        {
                //            if (savemethod == "SaveProductImages")
                //            {
                //                if (!obj.Data.ImgPath.Contains("http://"))
                //                {
                //                    obj.Data.ImgPath = "http://" + obj.Data.ImgPath;
                //                    obj.Data.ImgPath120 = "http://" + obj.Data.ImgPath120;
                //                    obj.Data.ImgPath200 = "http://" + obj.Data.ImgPath200;
                //                    obj.Data.ImgPath400 = "http://" + obj.Data.ImgPath400;
                //                    obj.Data.ImgPath60 = "http://" + obj.Data.ImgPath60;
                //                    msg = "{\"status\": 1, \"msg\": \"上传文件成功！\", \"ImgPath\": \"" + obj.Data.ImgPath +
                //                          "\", \"ImgPath120\": \"" +
                //                          obj.Data.ImgPath120 + "\", \"ImgPath200\": \"" + obj.Data.ImgPath200 +
                //                          "\", \"ImgPath400\": \"" + obj.Data.ImgPath400
                //                          + "\", \"ImgPath60\": \"" + obj.Data.ImgPath60 + "\"}";
                //                }
                //                else
                //                {
                //                    msg = "{\"status\": 1, \"msg\": \"上传文件成功！\", \"ImgPath\": \"" + obj.Data.ImgPath +
                //                          "\", \"ImgPath120\": \"" +
                //                          obj.Data.ImgPath120 + "\", \"ImgPath200\": \"" + obj.Data.ImgPath200 +
                //                          "\", \"ImgPath400\": \"" + obj.Data.ImgPath400
                //                          + "\", \"ImgPath60\": \"" + obj.Data.ImgPath60 + "\"}";
                //                }
                //            }
                //            else
                //            {
                //                msg = "{\"status\": 1, \"msg\": \"上传文件成功！\", \"path\": \"http://" + obj.Data.ImgPath + "\", \"ServerDomain\": \"" + "http://" + HttpContext.Current.Request.Url.Authority + "\", \"ServerDirectory\": \"\", \"ImageName\": \"\"}";
                //            }



                //            //删除临时保存的图片
                //            File.Delete(tempPath + "/" + fileName);
                //        }
                //        else
                //        {
                //            msg = "{\"status\": 0, \"msg\": \"" + obj.Info + "\"}";
                //        }
                //    }
                //}
            }
            catch (Exception ex)
            {
                context.Response.Write("{\"status\": 0, \"msg\": \"" + ex.Message + "\"}");
                return;
            }

            //返回成功信息
            context.Response.Write(msg);
            context.Response.End();
        }
        #endregion


        public class ImageServer
        {
            public string Flag { get; set; }
            public string Info { get; set; }
            public string Code { get; set; }
            public ImagePath Data { get; set; }
        }

        public class ImagePath
        {
            public string ImgPath { get; set; }
            public string ImgPath400 { get; set; }
            public string ImgPath200 { get; set; }
            public string ImgPath120 { get; set; }
            public string ImgPath60 { get; set; }
        }




        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}