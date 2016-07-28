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
    public class WarehouseEmpShelfController : BaseController
    {
        //
        // GET: /WarehouseEmpShelf/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeMenuFilter(520118)]
        public ActionResult EasyuiWarehouseEmpShelfList()
        {
            return View();
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520118, 52011801)]
        public ActionResult WarehouseEmpShelfEdit(string id, string name, string userAccount)
        {
            WarehouseEmpShelfModel model = new WarehouseEmpShelfModel();
            model.EmpName = name;
            model.UserAccount = userAccount;
            model.EmpID = int.Parse(id);
            model.GetEmpShelfData(id);
            model.BindShelfAreaList();
            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult WarehouseEmpShelfHandle(WarehouseEmpShelfModel model)
        {
            string result = string.Empty;
            if (model.ShelfIDs != null)
            {
                model.ShelfIDs = model.ShelfIDs.Substring(0, model.ShelfIDs.Length - 1);
            }
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpShelfSaveRequest()
            {
                EmpID = model.EmpID.ToString(),
                ShelfAreaID = model.ShelfAreaID.ToString(),
                ShelfIDs = model.ShelfIDs,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            if (resp.Flag == 0)
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功"
                }.ToJsonString();
                Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1H, ConstDefinition.XSOperatorActionAdd, "新增" + model.EmpID + "拣货货位！");
        
            }
            else
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                }.ToJsonString();
            }
            return Content(result);
        }

        #region 获取分页列表
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetWarehouseEmpShelfList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WarehouseEmpShelfModel().GetWarehouseEmpShelfPageData(cpm.page, cpm.rows, cpm.sort);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion

        /// <summary>
        /// 获取货位
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWarehouseEmpShelfSelectList(int ShelfAreaID, string ShelfCodeStart, string ShelfCodeEnd)
        {
            string jsonStr = "[]";
            //获取列表
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfListRequest()
            {
                ShelfAreaID = ShelfAreaID,
                ShelfCodeStart = ShelfCodeStart,
                ShelfCodeEnd = ShelfCodeEnd,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            //获取分类List解析的对象

            if (resp != null )
            {
                jsonStr = resp.ToJsonString();
            }
            return Content(jsonStr);
        }

    }
}
