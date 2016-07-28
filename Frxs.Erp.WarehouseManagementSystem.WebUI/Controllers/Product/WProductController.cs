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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Product
{
    /// <summary>
    /// 仓库商品添加编辑控制器
    /// </summary>
    public class WProductController : BaseController
    {

        /// <summary>
        /// 查看仓库商品列表
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520223)]
        public ActionResult WProductList()
        {
            return View();
        }


        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="model">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [AuthorizeMenuFilter(520223)]
        [ValidateInput(false)]
        public ActionResult GetWProductList(WProductListSearchModel model)
        {
            string jsonStr = "[]";
            try
            {
                string sortBy = "SKU ASC";
                if (!model.sort.IsNullOrEmpty() && !model.order.IsNullOrEmpty())
                {
                    sortBy = model.sort + " " + model.order;
                }
                var r = new FrxsErpProductWProductsAddedListGetRequest
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
                                WStatus = model.WStatus
                            };
                //获取列表

                var productWorkContext = WorkContext.CreateProductSdkClient();
                var resp = productWorkContext.Execute(r);
                if (resp != null && resp.Data != null)
                {
                    //读取库存信息：
                    List<int> pdidList = resp.Data.ItemList.Select(item => item.ProductId).ToList();
                    List<int> widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    FrxsErpOrderStockQtyListQueryRequest stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    {
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        PDIDList = pdidList,
                        WIDList = widList
                    };

                    //获取库存列表
                    var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    if (respstock != null && respstock.Data != null)
                    {
                        foreach (var item in resp.Data.ItemList)
                        {
                            var sumStockQty = (from i in respstock.Data
                                               where i.WID == WorkContext.CurrentWarehouse.Parent.WarehouseId && i.PID == item.ProductId
                                               select i.StockQty).Sum();
                            item.StockQty = sumStockQty;
                        }
                    }

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
        /// 仓库商品添加编辑页面
        /// </summary>
        /// <param name="productId">商品编号</param>
        /// <param name="addOrEdit">添加编辑操作(0表示添加,1表示编辑)</param>
        /// <returns></returns>
        public ActionResult WProductAdd(int productId, int addOrEdit)
        {
            string jsonStr = "[]";
            try
            {
                WProductAddModel model = new WProductAddModel
                {
                    AddOrEdit =
                        addOrEdit == 0
                            ? FrxsErpProductWProductsAddOrUpdateRequest.Flag.Add
                            : FrxsErpProductWProductsAddOrUpdateRequest.Flag.Edit,
                    PageTitle = addOrEdit == 0 ? "新增仓库商品" : "编辑仓库商品"
                };

                model.BindData();
                FrxsErpProductProductGetV20Request r = new FrxsErpProductProductGetV20Request
                {
                    ProductId = productId,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                };
                var resp = WorkContext.CreateProductSdkClient().Execute(r);
                if (resp != null && resp.Data != null)
                {

                    model.BrandName = (resp.Data.BrandCategories != null && resp.Data.BrandCategories.Count > 0)
                                          ? string.Join(";",
                                                        (from item in resp.Data.BrandCategories select item.BrandName))
                                          : "";
                    if (resp.Data.Product == null)
                    {
                        return ErrorResult("商品数据不能为空");
                    }

                    model.Sku = resp.Data.Product.SKU;
                    model.Mnemonic = resp.Data.Product.Mnemonic;
                    model.ProductId = resp.Data.Product.ProductId;
                    model.ProductName = resp.Data.Product.ProductName;
                    model.ProductName2 = resp.Data.Product.ProductName2;
                    model.CategoriesName = (resp.Data.Categories != null && resp.Data.Categories.Count > 0)
                                               ? string.Join(">>", (from item in resp.Data.Categories select item.Name))
                                               : "";
                    model.ShopCategoriesName = (resp.Data.ShopCategories != null && resp.Data.ShopCategories.Count > 0)
                                                   ? string.Join(">>",
                                                                 (from item in resp.Data.ShopCategories
                                                                  select item.CategoryName))
                                                   : "";
                    model.WProductUnitList = new List<WProductAddModel.WProductUnit>();
                    model.SaleBackFlag = "1"; //硬编码,表示可退

                    FrxsErpProductWProductsGetRequest wr = new FrxsErpProductWProductsGetRequest
                    {
                        ProductId = productId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    };
                    var wresp = WorkContext.CreateProductSdkClient().Execute(wr);
                    if (wresp != null && wresp.Data != null)
                    {
                        model.AddOrEdit = FrxsErpProductWProductsAddOrUpdateRequest.Flag.Edit;
                        if (wresp.Data.WProduct == null)
                        {
                            return ErrorResult("仓库商品不能为空");
                        }

                        model.BasePoint = wresp.Data.WProduct.BasePoint;
                        model.MarketPrice = wresp.Data.WProduct.MarketPrice;
                        model.MarketUnit = wresp.Data.WProduct.MarketUnit;
                        model.ProductName2 = wresp.Data.WProduct.ProductName2;
                        model.SaleBackFlag = wresp.Data.WProduct.SaleBackFlag;
                        model.SalePrice = wresp.Data.WProduct.SalePrice;
                        model.ShelfCode = wresp.Data.Shelf == null ? "" : wresp.Data.Shelf.ShelfCode;
                        model.ShopAddPerc = wresp.Data.WProduct.ShopAddPerc;
                        model.ShopPoint = wresp.Data.WProduct.ShopPoint;
                        model.VendorPerc1 = wresp.Data.WProduct.VendorPerc1;
                        model.VendorPerc2 = wresp.Data.WProduct.VendorPerc2;
                        model.WStatus = wresp.Data.WProduct.WStatus;

                        if (wresp.Data.WProductsBuy != null)
                        {
                            model.BuyPrice = wresp.Data.WProductsBuy.BuyPrice;
                            model.VendorID = wresp.Data.WProductsBuy.VendorID;
                        }

                        FrxsErpProductVendorGetByIDRequest vr = new FrxsErpProductVendorGetByIDRequest
                                                                    {
                                                                        UserId = WorkContext.UserIdentity.UserId,
                                                                        UserName = WorkContext.UserIdentity.UserName,
                                                                        VendorID = model.VendorID
                                                                    };
                        var vendoreq = WorkContext.CreateProductSdkClient().Execute(vr);
                        if (vendoreq != null && vendoreq.Data != null)
                        {
                            model.VendorName = vendoreq.Data.VendorName;
                            model.VendorCode = vendoreq.Data.VendorCode;
                            model.hidVendorCode = vendoreq.Data.VendorCode;
                        }

                        model.DeliveryUnitID = wresp.Data.WProduct.BigProductsUnitID.HasValue
                                                   ? wresp.Data.WProduct.BigProductsUnitID.Value
                                                   : 0;
                    }

                    if (resp.Data.Product != null && resp.Data.ProductsUnit != null
                        && resp.Data.ProductsUnit.Any())
                    {
                        //默认选择默认配送单位
                        int defalutDeliveryUnitID = 0;

                        IList<SelectListItem> items = new List<SelectListItem>();
                        foreach (var item in resp.Data.ProductsUnit)
                        {
                            var unitTypeName = item.IsUnit == 1 ? "库存单位" : "配送单位";
                            decimal? unitShopAddPercMoney = model.SalePrice * item.PackingQty * model.ShopAddPerc;
                            decimal? unitVendorPerc1Money = model.SalePrice * item.PackingQty * model.VendorPerc1;
                            decimal? unitVendorPerc2Money = model.SalePrice * item.PackingQty * model.VendorPerc2;
                            decimal unitVolume = resp.Data.Product.Volume * item.PackingQty;
                            decimal unitWeight = resp.Data.Product.Weight * item.PackingQty;
                            decimal? unitMarketPrice = model.MarketPrice * item.PackingQty;

                            if (item.IsSaleUnit == 1) //IsSaleUnit==1表示默认配送单位,0表示普通配送单位。IsUnit==1表示库存单位，其他表示配送单位
                            {
                                defalutDeliveryUnitID = item.ProductsUnitID;
                            }
                            var wPunit = new WProductAddModel.WProductUnit
                            {
                                IsSaleUnit = item.IsSaleUnit,
                                IsUnit = item.IsUnit,
                                PackingQty = item.PackingQty,
                                Spec = item.Spec,
                                Unit = item.Unit,
                                UnitBuyPrice = model.BuyPrice * item.PackingQty,
                                UnitMarketPrice = unitMarketPrice,
                                UnitSalePrice = model.SalePrice * item.PackingQty,
                                UnitShopPoint = model.ShopPoint * item.PackingQty,
                                UnitShopAddPerc = model.ShopAddPerc,
                                UnitShopAddPercMoney = unitShopAddPercMoney,
                                UnitTypeName = unitTypeName,
                                UnitVendorPerc1 = model.VendorPerc1,
                                UnitVendorPerc1Money = unitVendorPerc1Money,
                                UnitVendorPerc2 = model.VendorPerc2,
                                UnitVendorPerc2Money = unitVendorPerc2Money,
                                UnitVolume = unitVolume,
                                UnitWeight = unitWeight,
                                ProductsUnitId = item.ProductsUnitID
                            };
                            model.WProductUnitList.Add(wPunit);
                            items.Add(new SelectListItem()
                            {
                                Text = item.Unit,
                                Value = item.Unit
                            });
                        }
                        model.WProductMarketUnitList = new SelectList(items, "Value", "Text", true);

                        if (model.DeliveryUnitID == 0)
                        {
                            model.DeliveryUnitID = defalutDeliveryUnitID;
                        }
                    }
                }

                jsonStr = model.ToJsonString();
            }
            catch (Exception ex)
            {
                //Logger.GetInstance().ExceptionLog(new NormalLog
                //                                      {
                //                                          LogTime = DateTime.Now,
                //                                          LogContent = ex.Message + ex.StackTrace,
                //                                          LogSource = "Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Product.WProductController",
                //                                          LogOperation = "WProductAdd",
                //                                          LogIp = Request.UserHostAddress
                //                                      });
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.StackTrace }.ToJsonString();
            }
            return Content(jsonStr);
        }


        /// <summary>
        /// 仓库商品添加编辑页面
        /// </summary>
        /// <returns></returns>
        public ActionResult WProductAddNew()
        {
            return View();
        }


        /// <summary>
        ///仓库商品编辑页面
        /// </summary>
        /// <param name="productId">商品编号</param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520223, 52022301)]
        public ActionResult WProductEdit(int productId)
        {
            WProductViewModel model = new WProductViewModel { ProductId = productId };
            return View(model);
        }

        /// <summary>
        /// 保存
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public ActionResult WProductAddHandle(WProductModeNew model)
        {
            try
            {
                if (model.BuyPrice != model.SalePrice)
                {
                    return ErrorResult("进价要等于配送价");
                }

                //校验
                if (!this.ModelState.IsValid)
                {
                    //服务端错误信息
                    return this.ErrorResult(string.Format("校验失败，失败原因为：{0}", base.GetValidateErrorMsg()));
                }

                var r = new FrxsErpProductWProductsAddOrUpdateRequest
                {
                    AddOrEdit = model.AddOrEdit,
                    BasePoint = model.BasePoint,
                    BuyPrice = model.BuyPrice,
                    DeliveryUnitID = model.DeliveryUnitID,
                    MarketPrice = model.MarketPrice,
                    MarketUnit = model.MarketUnit,
                    ProductId = model.ProductId,
                    ProductName2 = model.ProductName2,
                    SaleBackFlag = model.SaleBackFlag,
                    SalePrice = model.SalePrice,
                    ShelfCode = model.ShelfCode,
                    ShopAddPerc = model.ShopAddPerc,
                    ShopPoint = model.ShopPoint,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    VendorPerc1 = model.VendorPerc1,
                    VendorPerc2 = model.VendorPerc2,
                    VendorID = model.VendorID,

                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    WStatus = model.WStatus
                };

                var wresp = WorkContext.CreateProductSdkClient().Execute(r);
                if (wresp == null)
                {
                    return this.ErrorResult("返回数据不正确,保存失败");
                }
                if (wresp.Flag == 0 && wresp.Data.HasValue)
                {
                    //记录操作日志
                    var action = model.AddOrEdit == FrxsErpProductWProductsAddOrUpdateRequest.Flag.Add
                                     ? ConstDefinition.XSOperatorActionAdd
                                     : ConstDefinition.XSOperatorActionEdit;
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2C,
                                  action, string.Format("{0}仓库商品[{1}]", action, wresp.Data.Value));

                    return this.SuccessResult(wresp.Data.Value.ToString("D"));
                }
                return this.FailResult(wresp.Info);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                return this.ExceptionResult(string.Format("出现异常：{0}", ex.StackTrace));
            }
        }


        /// <summary>
        /// 批量移除仓库商品
        /// </summary>
        /// <param name="prodcutids">商品编号串</param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520223, new int[] { 52022302 })]
        public ActionResult DeletWProduct(string prodcutids)
        {
            string result;
            try
            {
                IList<int> productIds = new List<int>();

                string[] pridlist = prodcutids.Split(',');

                for (int j = 0; j < pridlist.Length - 1; j++)
                {
                    productIds.Add(int.Parse(pridlist[j]));
                }

                //传入参数
                var r = new FrxsErpProductWProductsStatusUpdateRequest
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    Status = (int)WarehouseEnum.WProductStatus.已移除,
                    //请求参数 
                    ProductIds = productIds.ToList(),
                    UserId = this.UserIdentity.UserId,
                    UserName = this.UserIdentity.UserName
                };

                //获取列表
                var resp = WorkContext.CreateProductSdkClient().Execute(r);
                if (resp == null)
                {
                    return ErrorResult("返回值为空,可能是网络异常");
                }

                if (resp.Flag != 0)
                {
                    return ErrorResult(resp.Info);
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功"
                }.ToJsonString();

                //记录操作日志
                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2C,
                               ConstDefinition.XSOperatorActionDel,
                               string.Format("批量{0}仓库商品[{1}]", ConstDefinition.XSOperatorActionDel, prodcutids));

            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.StackTrace }.ToJsonString();
            }
            return Content(result);
        }



        /// <summary>
        /// 批量添加仓库商品
        /// </summary>
        /// <param name="prodcutids">商品编号串 </param>
        /// <returns></returns>
        public ActionResult WProductStatus(string prodcutids)
        {
            string result;
            try
            {
                IList<int> productIds = new List<int>();

                string[] pridlist = prodcutids.Split(',');

                for (int j = 0; j < pridlist.Length - 1; j++)
                {
                    productIds.Add(int.Parse(pridlist[j]));
                }

                //传入参数
                var r = new FrxsErpProductWProductsStatusUpdateRequest
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    Status = (int)WarehouseEnum.WProductStatus.正常,
                    //请求参数 
                    ProductIds = productIds.ToList(),
                    UserId = this.UserIdentity.UserId,
                    UserName = this.UserIdentity.UserName
                };

                //获取列表
                var resp = WorkContext.CreateProductSdkClient().Execute(r);
                if (resp == null)
                {
                    return ErrorResult("返回值为空,可能是网络异常");
                }

                if (resp.Flag != 0)
                {
                    return ErrorResult(resp.Info);
                }

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功"
                }.ToJsonString();
                //记录操作日志
                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2C,
                            ConstDefinition.XSOperatorActionDel,
                            string.Format("批量添加{0}仓库商品[{1}]", ConstDefinition.XSOperatorActionAdd, prodcutids));
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);
        }


        /// <summary>
        /// 仓库库存查看窗口
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520223, 52022301)]
        public ActionResult WareHouseStockList()
        {
            return View();
        }

        /// <summary>
        /// 查看库存数据窗口
        /// </summary>
        /// <param name="productId"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520223, 52022301)]
        public ActionResult GetWareHouseStockList(int productId)
        {
            var stockList = new List<WareHouseStockListShowModel>();
            string jsonStr = "[]";
            try
            {
                //读取库存信息：
                List<int> pdidList = new List<int> { productId };
                List<int> widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                FrxsErpOrderStockQtyListQueryRequest stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                {
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName,
                    PDIDList = pdidList,
                    WIDList = widList
                };

                //获取库存列表
                var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                if (respstock != null && respstock.Data != null)
                {
                    stockList.AddRange(respstock.Data.Select(item =>
                                                                 {
                                                                     var subWarehouseName = "";
                                                                     var subWarehouse =
                                                                         WorkContext.CurrentWarehouse.
                                                                             ParentSubWarehouses.FirstOrDefault(
                                                                                 i => i.WarehouseId == item.SubWID);
                                                                     if (subWarehouse != null)
                                                                         subWarehouseName = subWarehouse.WarehouseName;
                                                                     return new WareHouseStockListShowModel
                                                                                {
                                                                                    ProductId = item.PID,
                                                                                    StockQty = item.StockQty,
                                                                                    SubWarehouseName = subWarehouseName,
                                                                                    SubWId = item.SubWID,
                                                                                    WId = item.WID
                                                                                };
                                                                 }));

                    var obj = new { total = stockList.Count, rows = stockList };
                    jsonStr = obj.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.StackTrace }.ToJsonString();
            }
            return Content(jsonStr);
        }

    }
}
