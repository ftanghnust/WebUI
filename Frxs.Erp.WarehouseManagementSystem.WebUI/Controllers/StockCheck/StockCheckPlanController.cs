using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    public class StockCheckPlanController : BaseController
    {
        public ActionResult StockCheckPlanList()
        {
            return View();
        }

        public ActionResult StockCheckPlanAddOrEdit()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetStockCheckPlanList(StockCheckPlanQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new StockCheckPlanModel().GetStockCheckPlanPageData(cpm);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        public ActionResult GetStockCheckPlan(string id)
        {
            return Content(String.Empty);
        }

        public ActionResult EasyuiShelfList()
        {
            return View();
        }

        public ActionResult EasyuiShelfAreaList()
        {
            return View();
        }

        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetShelfList()
        {
            return Content(String.Empty);
        }
    }
}
