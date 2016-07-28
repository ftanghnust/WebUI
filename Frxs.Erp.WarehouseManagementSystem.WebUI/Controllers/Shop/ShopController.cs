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
    public class ShopController : BaseController
    {
        //
        // GET: /Shop/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeMenuFilter(520112)]
        public ActionResult EasyuiShopList()
        {
            return View();
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520112, 52011202)]
        public ActionResult ShopAddOrEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShopModel model = new ShopModel().GetShopData(id);
                
                model.BindSettleTypeList();
                model.BindSettleTimeTypeList();
                model.BindCreditLevelList();
                model.BindShopTypeList();
                model.BindWarehouseLineList();
                return View(model);
            }
            else
            {
                ShopModel model = new ShopModel();
                model.BindSettleTypeList();
                model.BindSettleTimeTypeList();
                model.BindCreditLevelList();
                model.BindShopTypeList();
                model.BindWarehouseLineList();
                return View(model);
            }
        }

        [ValidateInput(false)]
        //[AuthorizeButtonFiter(520112, 52011201)]
        public ActionResult ShopView(string id)
        {
            ShopModel model = new ShopModel().GetShopData(id);
            model.PageTitle = "查看";
            model.BindSettleTypeList();
            model.BindSettleTimeTypeList();
            model.BindCreditLevelList();
            model.BindShopTypeList();
            model.BindWarehouseLineList();
            return View(model);

        }

        [ValidateInput(false)]
        public ActionResult ShopHandle(ShopModel model)
        {
            string result = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    result = new ShopModel().SaveShopData(model).ToJson();
                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1B, ConstDefinition.XSOperatorActionEdit, "修改" + model.ShopName + "门店送货线路！");
                
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

      

        #region 批量冻结
        [AuthorizeButtonFiter(520112, 52011203)]
        public ActionResult IsFrozenShop(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {                    
                    int rows = new ShopModel().FrozenShop(ids, 0); 

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功冻结{0}个门店！", rows)
                    }.ToJsonString();

                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1B, ConstDefinition.XSOperatorActionFREEZE, "冻结" + ids + "门店！");
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
        [AuthorizeButtonFiter(520112, 52011204)]
        public ActionResult FrozenShop(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {                    
                    int rows = new ShopModel().FrozenShop(ids, 1); ;

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功解冻{0}个门店！", rows)
                    }.ToJsonString();

                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1B, ConstDefinition.XSOperatorActionUNFREEZE, "解冻" + ids + "门店！");
               
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
        public ActionResult GetShopList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ShopModel().GetShopModelPageData(cpm.page, cpm.rows, cpm.sort);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion

        #region 密码重置
        [AuthorizeButtonFiter(520112, 52011205)]
        public ActionResult ResetPasswordShop(string id)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(id))
                {

                    var serviceCenter = WorkContext.CreateMemberSdkClient();
                    var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Member.SDK.Request.FrxsErpMemberUserResetPasswordRequest()
                    {
                         UserAccount=id,
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

                        Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1B, ConstDefinition.XSOperatorActionEdit, "重置" + id + "密码！");

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

    }
}
