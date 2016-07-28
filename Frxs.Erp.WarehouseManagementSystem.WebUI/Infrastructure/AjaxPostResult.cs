/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com 2016/3/16 15:22:09
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Json;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// 返回系统定义的错误JSON串ActionResult
    /// </summary>
    public class AjaxPostResult : ContentResult
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="info">错误消息</param>
        /// <param name="code"></param>
        /// <param name="data"></param>
        /// <param name="flag"></param>
        public AjaxPostResult(string flag, string info, string code = null, object data = null)
        {
            this.Content = new ResultData() { Flag = flag, Info = info, Code = code, Data = data }.ToJson();
        }
    }
}