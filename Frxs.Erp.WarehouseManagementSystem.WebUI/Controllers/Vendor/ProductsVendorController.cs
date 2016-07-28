using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;

using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{

    /// <summary>
    /// 供应商商品控制器
    /// </summary>
    public class ProductsVendorController : BaseController
    {

        /// <summary>
        /// 供应商商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult VendorProductsList(int vendorId)
        {
            return View();
        }


        /// <summary>
        /// 获取供应商商品列表
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVendorProductsList(VendorProductsListSearchModel model)
        {
            string jsonStr = "[]";
            try
            {
                string sortBy = "Sku  asc";
                if (!model.sort.IsNullOrEmpty() && !model.order.IsNullOrEmpty())
                {
                    sortBy = model.sort + "  " + model.order;
                }

                var r = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductProductsVendorGetRequest
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    VendorID = model.VendorID,
                    PageIndex = model.page,
                    PageSize = model.rows,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    SortBy = sortBy
                };

                //获取列表
                var resp = WorkContext.CreateProductSdkClient().Execute(r);
                if (resp != null && resp.Data != null)
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


        /// <summary>
        /// 添加商品供应商供应关系
        /// </summary>
        /// <returns></returns>
        public ActionResult AddProductsVendorListHandle(string jsonObject, int vendorID)
        {
            try
            {
                var productsVendorList = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<FrxsErpProductProductsVendorAddOrEditRequest.ProductsVendorRequestDto>(jsonObject);

                var r = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductProductsVendorAddOrEditRequest
                {
                    productsVendorList = productsVendorList,
                    VendorID = vendorID,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                };

                //获取列表
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
                                Action = OperatorAction.Add,
                                Remark = "批量修改供应商商品关系:" + r.VendorID,
                                OperatorID = this.UserIdentity.UserId,
                                OperatorName = this.UserIdentity.UserName,
                                CreateTime = DateTime.Now
                            }
                        );
                    return this.SuccessResult(resp.Data.ToString("D"));
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
