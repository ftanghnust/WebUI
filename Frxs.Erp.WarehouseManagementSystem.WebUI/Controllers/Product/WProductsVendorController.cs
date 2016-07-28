using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Product;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Product
{
    /// <summary>
    /// 网仓商品供应商设置控制器
    /// </summary>
    public class WProductsVendorController : BaseController
    {
        /// <summary> 
        /// 网仓商品供应商列表
        /// </summary>
        /// <param name="productid">商品编号</param>
        /// <returns></returns>
        public ActionResult ProductsVendorList(int productid)
        {
            return View();
        }


        /// <summary> 
        /// 获取网仓商品供应商列表
        /// </summary>
        /// <param name="productid">商品编号</param>
        /// <returns></returns>
        public ActionResult GetProductsVendorList(int productid)
        {
            string jsonStr = "[]";
            try
            {
                FrxsErpProductWProductsVendorsGetRequest r = new FrxsErpProductWProductsVendorsGetRequest()
                                                                 {
                                                                     ProductId = productid,
                                                                     UserId = WorkContext.UserIdentity.UserId,
                                                                     UserName = WorkContext.UserIdentity.UserName,
                                                                     WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                                                                 };

                //获取列表
                var resp = WorkContext.CreateProductSdkClient().Execute(r);
                if (resp != null && resp.Data != null)
                {
                    jsonStr = resp.Data.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }


        /// <summary>
        /// 设置主供应商
        /// </summary>
        /// <returns></returns>
        public ActionResult SetMasterVendor(WProductMasterVendor model)
        {
            try
            {
                FrxsErpProductWProductsMasterVendorSetRequest r = new FrxsErpProductWProductsMasterVendorSetRequest
                                                               {
                                                                   ProductId = model.ProductId,
                                                                   UserId = WorkContext.UserIdentity.UserId,
                                                                   UserName = WorkContext.UserIdentity.UserName,
                                                                   VendorID = model.VendorID,
                                                                   WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                                                               };
                var resp = WorkContext.CreateProductSdkClient().Execute(r);
                if (resp == null)
                {
                    return this.ErrorResult("数据不正确");
                }
                if (resp.Flag == 0)
                {
                    //记录操作日志
                    Logger.GetInstance().XSOperatorLog
                        (
                            new XSOperatorLog
                            {
                                SysID = SysIDCategory.Erp,
                                FirstMenuID = -1,
                                SecondMenuID = -1,
                                Action = OperatorAction.Modify,
                                Remark = "设置主供应商",
                                OperatorID = this.UserIdentity.UserId,
                                OperatorName = this.UserIdentity.UserName,
                                CreateTime = DateTime.Now
                            }
                        );
                    return this.SuccessResult();
                }
                return this.FailResult(resp.Info);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                return this.ExceptionResult(string.Format("出现异常：{0}", ex.Message));
            }
        }


    }
}
