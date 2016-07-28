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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Vendor
{
    public class VendorTypeController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult VendorTypeAddOrEdit()
        {
            return View();
        }

        [HttpPost]
        public ActionResult GetVendorTypeList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new VendorTypeModel().GetVendorTypePageData(cpm.page, cpm.rows, cpm.sort);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }

        public ActionResult GetVendorType(int id)
        {
            if (id != null)
            {
                VendorTypeModel model = new VendorTypeModel().GetVendorType(id);
                model.PageTitle = "修改供应商类型";
                return Json(model, JsonRequestBehavior.AllowGet);
            }
            else
            {
                ShelfAreaModel model = new ShelfAreaModel { PageTitle = "新增供应商类型" };
                return null;
            }
        }

        [ValidateInput(false)]
        public ActionResult SaveVendorType(VendorTypeModel model)
        {
            var result = new VendorTypeModel().SaveVendorType(model);
            return Content(result.ToJsonString());
        }

        [ValidateInput(false)]
        public ActionResult DeleteVendorType(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    int rows = new VendorTypeModel().DeleteVendorType(ids);
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功删除{0}条数据", rows)
                    }.ToJsonString();
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中删除数据"
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("出现异常：{0}", ex.Message)
                }.ToJsonString();
            }
            return Content(result);
        }
    }
}
