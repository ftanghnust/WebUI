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
    /// <summary>
    /// 
    /// </summary>
    public class WarehouseEmpController : BaseController
    {
        //
        // GET: /WarehouseEmp/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeMenuFilter(520117)]
        public ActionResult EasyuiWarehouseEmpList()
        {
            return View();
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520117, 52011701)]
        public ActionResult WarehouseEmpAddOrEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                WarehouseEmpModel model = new WarehouseEmpModel().GetWarehouseEmpData(id);
                model.PageTitle = "修改账户";
                model.BindWCList();
                model.BindUserTypeList();
                model.BindIsMasterList();
                return View(model);
            }
            else
            {
                WarehouseEmpModel model = new WarehouseEmpModel { PageTitle = "新增账户" };
                model.BindWCList();
                model.BindUserTypeList();
                model.BindIsMasterList();
                return View(model);
            }
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520117, 52011702)]
        public ActionResult WarehouseEmpEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                WarehouseEmpModel model = new WarehouseEmpModel().GetWarehouseEmpData(id);
                model.PageTitle = "修改账户";
                model.BindWCList();
                model.BindUserTypeList();
                model.BindIsMasterList();
                return View(model);
            }
            else
            {
                WarehouseEmpModel model = new WarehouseEmpModel { PageTitle = "新增账户" };
                model.BindWCList();
                model.BindUserTypeList();
                model.BindIsMasterList();
                return View(model);
            }
        }
        [ValidateInput(false)]
        public ActionResult WarehouseEmpHandle(WarehouseEmpModel model)
        {
            string result = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    var serviceCenter = WorkContext.CreateProductSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpSaveRequest()
                    {
                        EmpID = model.EmpID,
                        EmpName = model.EmpName,
                        UserAccount = model.UserAccount,
                        UserType = model.UserType,
                        IsMaster = model.IsMaster,
                        WID = model.WID,
                        UserMobile = model.UserMobile,
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

                        Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1G, ConstDefinition.XSOperatorActionAdd, "新增" + model.EmpName + "账户！");
        
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
        [AuthorizeButtonFiter(520117, 52011703)]
        public ActionResult DeleteWarehouseEmp(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    
                    result = new WarehouseEmpModel().DeleteWarehouseEmp(ids).ToJson();                   

                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1G, ConstDefinition.XSOperatorActionDel, "删除" + ids + "账户！");
        
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

        #region 密码重置
        [AuthorizeButtonFiter(520117, 52011702)]
        public ActionResult ResetPasswordWarehouseEmp(string id)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {

                    var serviceCenter = WorkContext.CreateProductSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpResetRequest()
                    {
                        EmpID = int.Parse(id),                       
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });

                    if (resp.Flag == 0)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "密码重置成功！"
                        }.ToJsonString();

                        Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1G, ConstDefinition.XSOperatorActionEdit, "重置" + id + "密码！");
        
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
        [AuthorizeButtonFiter(520117, 52011704)]
        public ActionResult IsFrozenWarehouseEmp(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))                {
                    
                    int rows = new WarehouseEmpModel().FrozenWarehouseEmp(ids,1); 

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功冻结{0}个用户！", rows)
                    }.ToJsonString();
                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1G, ConstDefinition.XSOperatorActionFREEZE, "冻结" + ids + "用户！");
        
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
        [AuthorizeButtonFiter(520117, 52011705)]
        public ActionResult FrozenWarehouseEmp(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    
                    int rows = new WarehouseEmpModel().FrozenWarehouseEmp(ids, 0); 

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功解冻{0}个用户！", rows)
                    }.ToJsonString();

                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1G, ConstDefinition.XSOperatorActionUNFREEZE, "解冻" + ids + "用户！");
        
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
        public ActionResult GetWarehouseEmpList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WarehouseEmpModel().GetWarehouseEmpModelPageData(cpm.page, cpm.rows, cpm.sort);
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
