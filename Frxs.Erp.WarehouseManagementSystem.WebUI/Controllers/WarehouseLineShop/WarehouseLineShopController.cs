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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.WarehouseLineShop
{
    /// <summary>
    /// 
    /// </summary>
    public class WarehouseLineShopController : BaseController
    {
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public ActionResult Index()
        {
            return View();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520113, 52011304)]
        public ActionResult WarehouseLineShopAddOrEdit(int id)
        {
            WarehouseLineShopModel model = new WarehouseLineShopModel();
            model.LineID = id;
            return View(model);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ValidateInput(false)]
        public ActionResult warehouseLineShopHandle(WarehouseLineShopModel model)
        {
            string result = string.Empty;
            try
            {
                if (ModelState.IsValid)
                {
                    if (model.idList != null)
                    {
                        var serviceCenter = WorkContext.CreateProductSdkClient();
                        var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineShopSaveRequest()
                        {
                            LineID = model.LineID,
                            UserId = WorkContext.UserIdentity.UserId,
                            UserName = WorkContext.UserIdentity.UserName,
                            idList = model.idList.Substring(0, model.idList.Length - 1)
                        });

                        if (resp.Flag == 0)
                        {
                            result = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_SUCCESS,
                                Info = "操作成功"
                            }.ToJsonString();
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
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功"
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


        /// <summary>
        /// 获取门店
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWarehouseLineShopSelectList(int id)
        {
            string jsonStr = "[]";
            //获取列表
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineShopTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                LineID = id
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
