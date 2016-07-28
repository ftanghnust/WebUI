<%@ webhandler Language="C#" class="Upload" %>

/**
 * KindEditor ASP.NET
 */

using System;
using System.Collections;
using System.Configuration;
using System.Net;
using System.Text;
using System.Web;
using System.IO;
using System.Globalization;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Data;
using LitJson;

public class Upload : IHttpHandler
{
	private HttpContext context;

    public void ProcessRequest(HttpContext context)
    {
        //String aspxUrl = context.Request.Path.Substring(0, context.Request.Path.LastIndexOf("/") + 1);

        //文件保存目录路径
        String savePath = "/FileUpload/Kindeditor/";

        //文件保存目录URL
        String saveUrl = "/FileUpload/Kindeditor/";

        //定义允许上传的文件扩展名
        Hashtable extTable = new Hashtable();
        extTable.Add("image", "gif,jpg,jpeg,png,bmp");
        extTable.Add("flash", "swf,flv");
        extTable.Add("media", "swf,flv,mp3,wav,wma,wmv,mid,avi,mpg,asf,rm,rmvb");
        extTable.Add("file", "doc,docx,xls,xlsx,ppt,htm,html,txt,zip,rar,gz,bz2");

        //最大文件大小
        int maxSize = 1000000;
        this.context = context;

        HttpPostedFile imgFile = context.Request.Files["imgFile"];
        if (imgFile == null)
        {
            ShowError("请选择文件。");
        }

        String dirPath = context.Server.MapPath(savePath);
        if (!Directory.Exists(dirPath))
        {
            ShowError("上传目录不存在。");
        }

        String dirName = context.Request.QueryString["dir"];
        if (String.IsNullOrEmpty(dirName))
        {
            dirName = "image";
        }
        if (!extTable.ContainsKey(dirName))
        {
            ShowError("目录名不正确。");
        }

        String fileName = imgFile.FileName;
        String fileExt = Path.GetExtension(fileName).ToLower();

        if (imgFile.InputStream == null || imgFile.InputStream.Length > maxSize)
        {
            ShowError("上传文件大小超过限制。");
        }

        if (String.IsNullOrEmpty(fileExt) ||
            Array.IndexOf(((String) extTable[dirName]).Split(','), fileExt.Substring(1).ToLower()) == -1)
        {
            ShowError("上传文件扩展名是不允许的扩展名。\n只允许" + ((String) extTable[dirName]) + "格式。");
        }

        //创建文件夹
        dirPath += dirName + "/";
        saveUrl += dirName + "/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }
        String ymd = DateTime.Now.ToString("yyyyMMdd", DateTimeFormatInfo.InvariantInfo);
        dirPath += ymd + "/";
        saveUrl += ymd + "/";
        if (!Directory.Exists(dirPath))
        {
            Directory.CreateDirectory(dirPath);
        }

        String newFileName = DateTime.Now.ToString("yyyyMMddHHmmss_ffff", DateTimeFormatInfo.InvariantInfo) + fileExt;
        String filePath = dirPath + newFileName;

        imgFile.SaveAs(filePath);

        var fileUrl = saveUrl + newFileName;
        fileUrl = context.Request.Url.Scheme + "://" + context.Request.Url.Host + fileUrl;

        //远程保存地址
        var strUrl = ConfigurationManager.AppSettings["imageSvrUrl"].Trim('/') + "/SaveKindeditorImages" + "?url=" + fileUrl;

        var imgUrl = "";
        //模拟发送请求
        WebRequest request = WebRequest.Create(strUrl);
        request.Method = "GET";
        WebResponse response = request.GetResponse();
        var resp = response.GetResponseStream();
        if (resp != null)
        {
            //获取字符集编码
            string coder = ((HttpWebResponse) response).CharacterSet;
            if (coder != null)
            {
                var reader = new StreamReader(resp, Encoding.GetEncoding(coder));
                var callbackmessage = reader.ReadToEnd();

                //解析返回结果
                var obj = Frxs.Platform.Utility.Json.JsonHelper.FromJson<upload_ajax.ImageServer>(callbackmessage);
                if (obj.Flag == "SUCCESS")
                {
                    imgUrl = "http://" + obj.Data.ImgPath;
                }
            }
            
            //删除临时保存的图片
            File.Delete(filePath);
        }

        var hash = new Hashtable();
        hash["error"] = 0;
        hash["url"] = imgUrl;
        context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
        context.Response.Write(JsonMapper.ToJson(hash));
        context.Response.End();
    }

    private void ShowError(string message)
	{
		var hash = new Hashtable();
		hash["error"] = 1;
		hash["message"] = message;
		context.Response.AddHeader("Content-Type", "text/html; charset=UTF-8");
		context.Response.Write(JsonMapper.ToJson(hash));
		context.Response.End();
	}

	public bool IsReusable
	{
		get
		{
			return true;
		}
	}
}
