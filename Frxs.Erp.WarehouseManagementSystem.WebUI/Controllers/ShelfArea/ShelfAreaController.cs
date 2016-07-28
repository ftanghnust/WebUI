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
    public class ShelfAreaController : BaseController
    {
        //
        // GET: /ShelfArea/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeMenuFilter(520115)]
        public ActionResult EasyuiShelfAreaList()
        {
            return View();
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520115, 52011501)]
        public ActionResult ShelfAreaAddOrEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShelfAreaModel model = new ShelfAreaModel().GetShelfAreaData(id);
                model.PageTitle = "修改货区";                
                return View(model);
            }
            else
            {
                ShelfAreaModel model = new ShelfAreaModel { PageTitle = "新增货区" };               
                return View(model);
            }
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520115, 52011502)]
        public ActionResult ShelfAreaEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShelfAreaModel model = new ShelfAreaModel().GetShelfAreaData(id);
                model.PageTitle = "修改货区";
                return View(model);
            }
            else
            {
                ShelfAreaModel model = new ShelfAreaModel { PageTitle = "新增货区" };
                return View(model);
            }
        }

        [ValidateInput(false)]
        public ActionResult ShelfAreaHandle(ShelfAreaModel model)
        {
            string result = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceCenter = WorkContext.CreateProductSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfAreaSaveRequest()
                    {
                        ShelfAreaID = model.ShelfAreaID,
                        ShelfAreaCode = model.ShelfAreaCode,
                        ShelfAreaName = model.ShelfAreaName,
                        PickingMaxRecord = model.PickingMaxRecord,
                        SerialNumber = model.SerialNumber,
                        Remark = model.Remark,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    });

                    if (resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功"
                        }.ToJsonString();

                        Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1E, ConstDefinition.XSOperatorActionAdd, "保存" + model.ShelfAreaName + "货区！");
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = resp.Info
                        }.ToJsonString();
                    }
                }
                else
                {
                    //服务端错误信息
                    string errorMsg = base.GetValidateErrorMsg();
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = string.Format("校验失败，失败原因为：{0}", errorMsg)
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


        #region 批量删除
        [AuthorizeButtonFiter(520115, 52011503)]
        public ActionResult DeleteShelfArea(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) )
                {                   
                  result= new ShelfAreaModel().DeleteShelfArea(ids).ToJson();

                  Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1E, ConstDefinition.XSOperatorActionDel, "删除" + ids + "货区！");
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
        #endregion

        #region 获取分页列表
        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetShelfAreaList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ShelfAreaModel().GetShelfAreaPageData(cpm.page, cpm.rows, cpm.sort);
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
        /// 获取货区
        /// </summary>
        /// <returns></returns>
        public ActionResult GetShelfAreaSelectList()
        {
            string jsonStr = "[]";
            //获取列表
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfAreaTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            //获取分类List解析的对象

            if (resp != null && resp.Data != null && resp.Data.ItemList.Count > 0)
            {
                jsonStr = resp.Data.ItemList.ToJsonString();
            }
            return Content(jsonStr);
        }
    }
}
