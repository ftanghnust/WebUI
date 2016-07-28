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
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    public class OperatorLogController : BaseController
    {
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [ValidateInput(false)]
        [HttpPost]
        public ActionResult GetOperatorLogList(OperatorLogQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var erpIDSdkClient = WorkContext.CreateIDSdkClient();
                var resp = erpIDSdkClient.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogGetListRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort + " " + cpm.order,
                    OperatorName = cpm.OperatorName,
                    //Action = cpm.Action
                    MenuID = cpm.MenuID,
                    BeginTime = cpm.BeginTime,
                    EndTime = cpm.EndTime,
                });
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }

        [ValidateInput(false)]
        public ActionResult GetOperatorLogMenu()
        {
            string jsonStr = "[]";
            try
            {
                var erpIDSdkClient = WorkContext.CreateIDSdkClient();
                var resp = erpIDSdkClient.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogGetMenuRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                });
                if (resp != null && resp.Flag == 0)
                {
                    var obj = resp.Data.menus.ToList();
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }
    }
}
