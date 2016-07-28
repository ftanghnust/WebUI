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
    public class WarehouseLineController : BaseController
    {
        //
        // GET: /WarehouseLine/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeMenuFilter(520113)]
        public ActionResult EasyuiWarehouseLineList()
        {
            return View();
        }


        [ValidateInput(false)]
        [AuthorizeButtonFiter(520113, 52011301)]
        public ActionResult WarehouseLineAddOrEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                WarehouseLineModel model = new WarehouseLineModel().GetWarehouseLineData(id);
                model.BindEmpIDList();
                return View(model);
            }
            else
            {
                WarehouseLineModel model = new WarehouseLineModel { PageTitle = "新增送货线路" };
                model.BindEmpIDList();
                return View(model);
            }
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520113, 52011302)]
        public ActionResult WarehouseLineEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                WarehouseLineModel model = new WarehouseLineModel().GetWarehouseLineData(id);
                model.BindEmpIDList();
                return View(model);
            }
            else
            {
                WarehouseLineModel model = new WarehouseLineModel { PageTitle = "新增送货线路" };
                model.BindEmpIDList();
                return View(model);
            }
        }

        [ValidateInput(false)]
        public ActionResult warehouseLineHandle(WarehouseLineModel model)
        {
            string result = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {

                    if (!(model.SendW1 || model.SendW2 || model.SendW3 || model.SendW4 || model.SendW5 || model.SendW6 || model.SendW7))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "配送周期最少需要选择一个！"
                        }.ToJsonString();
                    }
                    else
                    {

                        var serviceCenter = WorkContext.CreateProductSdkClient();
                        var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineSaveRequest()
                        {
                            LineID = model.LineID,
                            LineCode = model.LineCode,
                            LineName = model.LineName,
                            EmpID = model.EmpID,
                            SendW1 = Convert.ToInt32(model.SendW1),
                            SendW2 = Convert.ToInt32(model.SendW2),
                            SendW3 = Convert.ToInt32(model.SendW3),
                            SendW4 = Convert.ToInt32(model.SendW4),
                            SendW5 = Convert.ToInt32(model.SendW5),
                            SendW6 = Convert.ToInt32(model.SendW6),
                            SendW7 = Convert.ToInt32(model.SendW7),
                            OrderEndTime = model.OrderEndTime,
                            Distance = model.Distance == null ? 0 : (int)model.Distance,
                            SendNeedTime = model.SendNeedTime == null ? 0 : (int)model.Distance,
                            SerialNumber = model.SerialNumber,
                            Remark = model.Remark,
                            WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
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
                            Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1C, ConstDefinition.XSOperatorActionAdd, "新增" + model.LineName + "送货线路！");
        
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
         [AuthorizeButtonFiter(520113, 52011303)]
        public ActionResult DeleteWarehouseLine(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids) )
                {

                    result = new WarehouseLineModel().DeleteWarehouseLine(ids).ToJson();

                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1C, ConstDefinition.XSOperatorActionDel, "删除" + ids + "送货线路！");
        
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
        public ActionResult GetWarehouseLineList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WarehouseLineModel().GetWarehouseLinePageData(cpm.page, cpm.rows, cpm.sort);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion
    }
}
