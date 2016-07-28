using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Erp.ServiceCenter.Product.SDK;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Platform.Utility.Map;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    public class VendorController : BaseController
    {
        /// <summary>
        /// 供应商列表页面
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520222)]
        public ActionResult EasyuiVendorList()
        {
            return View();
        }

        /// <summary>
        /// 供应商查看
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520222, new int[] { 52022201 })]
        public ActionResult AddOrEditNew()
        {
            return View();
        }

        #region 获取分页列表
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [AuthorizeMenuFilter(520222)]
        public ActionResult VendorList(VendorQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new VendorModel().GetVendorPageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }
        #endregion

        /// <summary>
        /// 获取供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVendor(int id)
        {
            VendorModel model = new VendorModel();
            FrxsErpProductVendorSaveRequest.Vendor vmod = model.GetModelByID((int)id);
            return Json(vmod, JsonRequestBehavior.AllowGet);
        }


    }
}