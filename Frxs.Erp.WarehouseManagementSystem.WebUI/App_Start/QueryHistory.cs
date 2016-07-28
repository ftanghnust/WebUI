using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Json;
using System.Text;


namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    internal class QueryHistory
    {

        /// <summary>
        /// 获取到当前查询历史COOKIES信息 
        /// </summary>
        /// <returns></returns>
        public static HttpCookie GetQueryHistoryCookies(string key)
        {
            return HttpContext.Current.Request.Cookies[key];
        }

        /// <summary>
        /// 记录查询历史记录
        /// </summary>
        /// <param name="queryHistory"></param>
        public static void SetQueryHistory(string key,string queryHistory, DateTime? expires = null)
        {
            //记录下cookies
            HttpCookie httpCookies = new HttpCookie(key);
            //httpCookies.Domain = "frxs.erp.cn";
            //httpCookies.HttpOnly = true;
            if (expires.HasValue)
            {
                httpCookies.Expires = expires.Value;
            }
            httpCookies.Value = queryHistory;

            //写下cookies
            HttpContext.Current.Response.Cookies.Add(httpCookies);
        }


        /// <summary>
        /// 清除
        /// </summary>
        public static void Clear(string key)
        {
            SetQueryHistory(key,null, DateTime.Now.AddDays(-365));
        }


        public string ID { get; set; }

        public string VendorCode { get; set; }

        public string VendorName { get; set; }
    }
}