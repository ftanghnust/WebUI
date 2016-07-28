using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.ShopGroup
{
    public class ShopGroupController : BaseController
    {
        //
        // GET: /ShopGroup/

        public ActionResult Index()
        {
            return View();
        }
        [AuthorizeMenuFilter(520114)]
        public ActionResult EasyuiShopGroupList()
        {
            return View();
        }

      

        [ValidateInput(false)]
        public ActionResult ShopGroupShow(string id)
        {
            ShopGroupModel model = new ShopGroupModel().GetShopGroupData(id);
            return View(model);
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520114, 52011401)]
        public ActionResult ShopGroupNewAddOrEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShopGroupModel model = new ShopGroupModel().GetShopGroupData(id);              
                return View(model);
            }
            else
            {
                ShopGroupModel model = new ShopGroupModel();                
                return View(model);
            }
        }

        [ValidateInput(false)]
        [AuthorizeButtonFiter(520114, 52011402)]
        public ActionResult ShopGroupNewEdit(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShopGroupModel model = new ShopGroupModel().GetShopGroupData(id);
                return View(model);
            }
            else
            {
                ShopGroupModel model = new ShopGroupModel();
                return View(model);
            }
        }
      

         /// <summary>
        /// 记录保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ShopGroupAddSave(string jsonData, string jsonDetails)
        {
            string result = string.Empty;
            try
            {
                ShopGroupModel model = Frxs.Platform.Utility.Json.JsonHelper.FromJson<ShopGroupModel>(jsonData);
                 model.List = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<ShopGroupDetails>(jsonDetails);

                result = new ShopGroupModel().SaveShopGroupData(model).ToJson();

                Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1D, ConstDefinition.XSOperatorActionAdd, "添加" + model.GroupName + "门店分组！");
              
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
        [AuthorizeButtonFiter(520114, 52011403)]
        public ActionResult DeleteShopGroup(string ids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(ids))
                {
                    
                    int rows = new ShopGroupModel().DeleteShopGroup(ids); 

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = string.Format("成功删除{0}条数据", rows)
                    }.ToJsonString();
                    Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure.OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_1D, ConstDefinition.XSOperatorActionDel, "删除" + ids + "门店分组！");
        
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
        public ActionResult GetShopGroupList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ShopGroupModel().GetShopGroupModelPageData(cpm.page, cpm.rows, cpm.sort);
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
        /// 获取数据
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult ShopGroupInfo(string id)
        {

            if (!string.IsNullOrEmpty(id))
            {
                ShopGroupModel model = new ShopGroupModel().GetShopGroupData(id);
                return Content(model.ToJsonString());
            }
            else
            {
                ShopGroupModel model = new ShopGroupModel();
                return Content(model.ToJsonString());
            }
        }


        public ActionResult EasyuiSearchShop()
        {
            return View();
        }

    }
}
