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
    public class WStationNumberController : BaseController
    {
        //
        // GET: /WStationNumber/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeMenuFilter(520314)]
        public ActionResult EasyuiWStationNumberList()
        {
            return View();
        }
        [AuthorizeButtonFiter(520314, 52031401)]
        public ActionResult AddWStationNumberList()
        {
            return View();
        }

        [ValidateInput(false)]
        public ActionResult WStationNumberHandle(WStationNumberModel model)
        {
            string result = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceCenter = WorkContext.CreateProductSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWStationNumberSaveRequest()
                    {
                        StartID = model.StartID,
                        EndID = model.EndID,
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

                        Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3D, ConstDefinition.XSOperatorActionDel, "新增" + model.StartID + "至" + model.EndID + "待装区！");
        
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
        [AuthorizeButtonFiter(520314, 52031403)]
        public ActionResult DeleteWStationNumber(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    int rows = new WStationNumberModel().DeleteWStationNumber(ids); 

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功删除{0}条数据", rows)
                    }.ToJsonString();
                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3D, ConstDefinition.XSOperatorActionDel, "删除" + ids + "待装区！");
        
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

        #region 数据清空
        [AuthorizeButtonFiter(520314, 52031402)]
        public ActionResult ResetWStationNumber(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {

                    var serviceCenter = WorkContext.CreateProductSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWStationNumberResetRequest()
                    {
                        ID=StringExtension.ToIntArray(ids, ',').ToList(),
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });

                    if (resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "数据清空成功！"
                        }.ToJsonString();

                        Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3D, ConstDefinition.XSOperatorActionReset, "清空" + ids + "待装区！");
        
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
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中数据"
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

        #region 批量冻结
        [AuthorizeButtonFiter(520314, 52031404)]
        public ActionResult IsFrozenWStationNumber(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {

                    int rows = new WStationNumberModel().FrozenWStationNumber(ids, 2);

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功冻结{0}个待装区！", rows)
                    }.ToJsonString();
                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3D, ConstDefinition.XSOperatorActionFREEZE, "冻结" + ids + "待装区！");
        
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中冻结数据"
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

        #region 批量解冻
        [AuthorizeButtonFiter(520314, 52031405)]
        public ActionResult FrozenWStationNumber(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {

                    int rows = new WStationNumberModel().FrozenWStationNumber(ids, 0);

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功解冻{0}个待装区！", rows)
                    }.ToJsonString();

                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3D, ConstDefinition.XSOperatorActionUNFREEZE, "解冻" + ids + "待装区！");
        
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中解冻数据"
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
        public ActionResult GetWStationNumberList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WStationNumberModel().GetWStationNumberPageData(cpm.page, cpm.rows, cpm.sort);
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
