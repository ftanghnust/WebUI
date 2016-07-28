using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Vendor
{
    public class VerdorProductsController : BaseController
    {
        /// <summary>
        /// 供应商商品列表
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520222, new int[] { 52022202 })]
        public ActionResult VendorProductsList(int vendorId)
        {
            return View();
        }

        /// <summary>
        /// 获取供应商商品列表
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520222, new int[] { 52022202 })]
        [ValidateInput(false)]
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
                    IsPage = 1,
                    IsMaster = model.IsMaster,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    KeyWord = model.KeyWord == null ? null : model.KeyWord.Trim(),
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
        /// 设置主供应商
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520222, new int[] { 52022202 })]
        public ActionResult SetMasterVendor(int vendorID, string prodcutids)
        {
            List<int> productIds = new List<int>();

            string[] pridlist = prodcutids.Split(',');

            for (int j = 0; j < pridlist.Length - 1; j++)
            {
                productIds.Add(int.Parse(pridlist[j]));
            }
            try
            {
                var r = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductProductsVendorSetMasterRequest
                {
                    ProductIds = productIds,
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
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6A,
                                  ConstDefinition.XSOperatorActionEdit, string.Format("批量设置主供应商[{0}]商品编号列表为:{1}", vendorID, prodcutids));

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

        /// <summary>
        /// 删除商品供应商关系
        /// </summary>
        /// <param name="vendorID"></param>
        /// <param name="prodcutids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520222, new int[] { 52022202 })]
        public ActionResult DelVendorProduct(int vendorID, string prodcutids)
        {
            try
            {
                List<int> productIds = new List<int>();

                string[] pridlist = prodcutids.Split(',');

                for (int j = 0; j < pridlist.Length - 1; j++)
                {
                    productIds.Add(int.Parse(pridlist[j]));
                }

                var r = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductProductsVendorDelRequest
                {
                    ProductIds = productIds,
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
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6A,
                                  ConstDefinition.XSOperatorActionDel, string.Format("删除商品编号列表{0}的供应商[{1}]关系", prodcutids, vendorID));

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



        /// <summary>
        /// 添加商品供应商供应关系
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520222, new int[] { 52022202 })]
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
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_6A,
                                  ConstDefinition.XSOperatorActionDel, string.Format("批量修改供应商[{0}]商品关系", vendorID));

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
