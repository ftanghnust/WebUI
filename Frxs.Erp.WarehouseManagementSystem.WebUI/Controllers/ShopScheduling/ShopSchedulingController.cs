using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Request;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Excel;
using System.Collections;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// 
    /// </summary>
    public class ShopSchedulingController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cpm"></param>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeMenuFilter(520315)]
        public ActionResult GetShopSchedulingList(ShopSchedulingQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ShopSchedulingModel().GetShopSchedulingPageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult ShopScheduling()
        {
            return View();
        }




    }
}
