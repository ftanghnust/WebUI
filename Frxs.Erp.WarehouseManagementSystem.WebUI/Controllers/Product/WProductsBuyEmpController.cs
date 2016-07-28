using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Enum;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.WProduct;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    public class WProductsBuyEmpController : BaseController
    {
        [AuthorizeMenuFilter(520230)]
        public ActionResult WProductsBuyEmpList()
        {
            return View();
        }

        [AuthorizeMenuFilter(520230)]
        public ActionResult WProductsBuyEmpSet()
        {
            return View();
        }

        /// <summary>
        /// 获取采购员信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBuyEmpInfo()
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpTableListRequest()
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    UserType = "4",   //采购员
                    IsFrozen = "0",   //非冻结
                });
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
                    result = obj.ToJsonString();
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
        /// 获取分页列表
        /// </summary>
        /// <param name="model">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetWProductsBuyEmpList(WProductsBuyEmpListSearchModel model)
        {
            string jsonStr = "[]";
            try
            {
                string sortBy = "SKU ASC";
                if (!model.sort.IsNullOrEmpty() && !model.order.IsNullOrEmpty())
                {
                    sortBy = model.sort + " " + model.order;
                }
                var frxsErpProductWProductsBuyEmpListGetRequest = new FrxsErpProductWProductsBuyEmpListGetRequest
                {
                    AddedVendor = null,
                    BarCode = model.BarCode == null ? null : model.BarCode.Trim(),
                    CategoryId1 = model.CategoriesId1,
                    CategoryId2 = model.CategoriesId2,
                    CategoryId3 = model.CategoriesId3,
                    PageIndex = model.page,
                    PageSize = model.rows,
                    ProductName = model.ProductName == null ? null : model.ProductName.Trim(),
                    SKU = model.Sku == null ? null : model.Sku.Trim(),
                    SortBy = sortBy,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    VendorName = model.VendorName == null ? null : model.VendorName.Trim(),
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    WStatus = model.WStatus,
                    BuyEmpName = model.BuyEmpName,
                    HasBuyEmp = model.HasBuyEmp
                };
                //获取列表

                var productWorkContext = WorkContext.CreateProductSdkClient();
                productWorkContext.SetTimeout(20000);
                var resp = productWorkContext.Execute(frxsErpProductWProductsBuyEmpListGetRequest);
                if (resp != null && resp.Data != null)
                {
                    //读取库存信息：
                    //List<int> pdidList = resp.Data.ItemList.Select(item => item.ProductId).ToList();
                    //List<int> widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    //FrxsErpOrderStockQtyListQueryRequest stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    //{
                    //    UserId = WorkContext.UserIdentity.UserId,
                    //    UserName = WorkContext.UserIdentity.UserName,
                    //    PDIDList = pdidList,
                    //    WIDList = widList
                    //};

                    ////获取库存列表
                    //var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    //if (respstock != null && respstock.Data != null)
                    //{
                    //    foreach (var item in resp.Data.ItemList)
                    //    {
                    //        var sumStockQty = (from i in respstock.Data
                    //                           where i.WID == WorkContext.CurrentWarehouse.Parent.WarehouseId && i.PID == item.ProductId
                    //                           select i.StockQty).Sum();
                    //        item.StockQty = sumStockQty;
                    //    }
                    //}
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

        [HttpPost]
        [ValidateInput(false)]
        [AuthorizeButtonFiter(520230, 52023001)]
        public ActionResult WProductsBuyEmpListSet(string productIds, string ids)
        {
            var result = new ResultData
            {
                Flag = ConstDefinition.FLAG_FAIL,
                Info = "操作失败",
                Data = null
            };
            try
            {
                var wproductIds = new List<int>();
                if (!String.IsNullOrEmpty(productIds))
                {
                    wproductIds = productIds.Split(',').ToList().Select(x => int.Parse(x)).ToList();
                }
                var buyEmpIds = new List<int>();
                if (!String.IsNullOrEmpty(ids))
                {
                    buyEmpIds = ids.Split(',').ToList().Select(x => int.Parse(x)).ToList();
                }
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsBuyEmpListSaveRequest()
                {
                    wid = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    wProductIds = wproductIds,
                    BuyEmpIds = buyEmpIds
                });

                if (resp == null)
                {
                    return ErrorResult("返回数据有误,请重新设置");
                }
                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2J,
                      ConstDefinition.XSOperatorActionEdit, string.Format("{0}商品采购员[商品:{1}][采购员:{2}]", "设置", productIds, ids));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = null
                    };
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info,
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(result.ToJsonString());
        }
    }
}
