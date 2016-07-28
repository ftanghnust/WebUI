using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Comm;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Common
{
    public class CommonController : Controller
    {
        //
        // GET: /Common/

        public ActionResult Index()
        {
            return View();
        }


        /// <summary>
        /// 获取采购员信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetBuyEmpInfo(string EmpName, int rows, int page)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpTableListRequest()
                {
                    PageIndex = page,
                    PageSize = rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    UserType = "4",   //采购员
                    IsFrozen = "0",   //非冻结
                    EmpName = (string.IsNullOrEmpty(EmpName) ? null : EmpName)
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
        /// 获取采购商品单位集合
        /// </summary>
        /// <param name="ProductID">商品ID</param>
        /// <returns></returns>
        public ActionResult GetBuyProductUnit(int ProductID)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ProductId = ProductID
                });

                if (resp != null && resp.Flag == 0)
                {
                    Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductsGetResp.WProductsBuy wProductBuy = resp.Data.WProductsBuy;
                    Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductsGetResp.WProducts wProducts = resp.Data.WProduct;
                    //默认只取两个单位：一个是采购单位，一个是库存单位
                    if (wProductBuy.BigUnit == wProducts.Unit)
                    {
                        result = "[";
                        result += "{";
                        result += "\"UnitName\":\"" + wProducts.Unit + "\",";
                        result += "\"PackingQty\":\"1\"";
                        result += "}";
                        result += "]";
                    }
                    else
                    {

                        result = "[";
                        result += "{";
                        result += "\"UnitName\":\"" + wProductBuy.BigUnit + "\",";
                        result += "\"PackingQty\":\"" + wProductBuy.BigPackingQty + "\"";
                        result += "},";
                        result += "{";
                        result += "\"UnitName\":\"" + wProducts.Unit + "\",";
                        result += "\"PackingQty\":\"1\"";
                        result += "}";
                        result += "]";
                    }
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
        /// 获取配送商品单位集合
        /// </summary>
        /// <param name="ProductID">商品ID</param>
        /// <returns></returns>
        public ActionResult GetSaleProductUnit(int ProductID)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ProductId = ProductID
                });

                if (resp != null && resp.Flag == 0)
                {
                    //Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductsGetResp.WProductsBuy wProductBuy = resp.Data.WProductsBuy;
                    Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductsGetResp.WProducts wProducts = resp.Data.WProduct;
                    //默认只取两个单位：一个是配送单位，一个是库存单位
                    if (wProducts.BigUnit == wProducts.Unit)
                    {
                        result = "[";
                        result += "{";
                        result += "\"UnitName\":\"" + wProducts.Unit + "\",";
                        result += "\"PackingQty\":\"1\"";
                        result += "}";
                        result += "]";
                    }
                    else
                    {
                        result = "[";
                        result += "{";
                        result += "\"UnitName\":\"" + wProducts.BigUnit + "\",";
                        result += "\"PackingQty\":\"" + wProducts.BigPackingQty + "\"";
                        result += "},";
                        result += "{";
                        result += "\"UnitName\":\"" + wProducts.Unit + "\",";
                        result += "\"PackingQty\":\"1\"";
                        result += "}";
                        result += "]";
                    }
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
        /// 获取仓库采购商品信息 需传递供应商ID  包含库存数量
        /// </summary>
        /// <param name="Value">查询内容</param>
        /// <param name="vendorid">供应商ID</param>
        /// <param name="type">查询条件</param>
        /// <param name="SKULikeSearch">是否按SKU模糊搜索</param>
        /// <returns></returns>
        public ActionResult GetBuyProductInfo(string Value, int vendorid, int SubWID, string type, int rows = 30, int page = 1, bool? SKULikeSearch = null)
        {
            string result = string.Empty;
            try
            {
                //根据供应商判断取值
                if (vendorid <= 0)
                {
                    result = GetBuyProductInfoAdd(Value, SubWID, type, rows, page);
                    return Content(result);
                }
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsBuyListGetRequest()
                {
                    PageIndex = page,
                    PageSize = rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    VendorID = vendorid,
                    SKU = (type == "SKU") ? Value : null,
                    ProductName = (type == "ProductName") ? Value : null,
                    SKULikeSearch = SKULikeSearch
                });
                if (resp != null && resp.Flag == 0)
                {
                    //获取库存列表
                    var pdidList = from o in resp.Data.ItemList select o.ProductId;
                    var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    {
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        PDIDList = pdidList.ToList(),
                        WIDList = widList
                    };
                    var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    List<BuyProductExt> products = new List<BuyProductExt>();
                    foreach (var item in resp.Data.ItemList)
                    {
                        var tempproduct = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<BuyProductExt>(item);
                        tempproduct.OldBuyPrice = item.BuyPrice;
                        if (respstock != null && respstock.Data != null)
                        {
                            var frxsErpOrderStockQtyListQueryRespData = respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == SubWID);
                            if (frxsErpOrderStockQtyListQueryRespData != null)
                            {
                                tempproduct.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            }
                        }
                        products.Add(tempproduct);
                    }
                    var obj = new { total = resp.Data.TotalRecords, rows = products };
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
        /// 补货调用
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="SubWID"></param>
        /// <param name="type"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public string GetBuyProductInfoAdd(string Value, int SubWID, string type, int rows = 30, int page = 1)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsAddedListGetRequest()
                {
                    PageIndex = page,
                    PageSize = rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKU = (type == "SKU") ? Value : null,
                    ProductName = (type == "ProductName") ? Value : null,
                    WStatus = 1
                });
                if (resp != null && resp.Flag == 0)
                {
                    //获取库存列表
                    var pdidList = from o in resp.Data.ItemList select o.ProductId;
                    var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    {
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        PDIDList = pdidList.ToList(),
                        WIDList = widList
                    };
                    var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    List<BuyProductExt> products = new List<BuyProductExt>();
                    foreach (var item in resp.Data.ItemList)
                    {
                        var tempproduct = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<BuyProductExt>(item);
                        tempproduct.OldBuyPrice = item.BuyPrice;
                        if (respstock != null && respstock.Data != null)
                        {
                            var frxsErpOrderStockQtyListQueryRespData = respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == SubWID);
                            if (frxsErpOrderStockQtyListQueryRespData != null)
                            {
                                tempproduct.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            }
                        }
                        tempproduct.BuyUnit = item.BigUnit;
                        tempproduct.PackingQty = item.BuyBigPackingQty;
                        tempproduct.Unit = item.Unit;
                        tempproduct.UnitPrice = item.UnitBuyPrice;
                        tempproduct.SaleUnitPrice = item.UnitSalePrice;
                        products.Add(tempproduct);
                    }
                    var obj = new { total = resp.Data.TotalRecords, rows = products };
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
            return result;
        }




        public class BuyProductExt : Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductsBuyListGetResp.WProductsQueryModel
        {
            //购买价格备份
            public decimal OldBuyPrice { get; set; }

            //库存数
            public decimal WStock { get; set; }

        }

        /// <summary>
        /// 获取仓库商品信息 公共方法
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="type"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="SKULikeSearch"></param>
        /// <returns></returns>
        public ActionResult GetSaleProductInfo(string Value, string type, int? ShopID, int rows = 30, int page = 1, bool? SKULikeSearch = null, int subWId = 0)
        {
            return GetSaleProductInfoSaleAll(Value, type, ShopID, null, rows, page, SKULikeSearch, subWId);
        }

        /// <summary>
        /// 获取仓库商品信息 公共方法
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="type"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="SKULikeSearch"></param>
        /// <returns></returns>
        public ActionResult GetSaleProductInfoSaleAll(string Value, string type, int? ShopID, bool? saleBackFlag, int rows = 30, int page = 1, bool? SKULikeSearch = null, int subWId = 0)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsSaleListGetRequest()
                {
                    PageIndex = page,
                    PageSize = rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKU = (type == "SKU") ? Value : null,
                    ProductName = (type == "ProductName") ? Value : null,
                    BarCode = (type == "BarCode") ? Value : null,
                    SKULikeSearch = SKULikeSearch,
                    SaleBackFlag = saleBackFlag
                });
                if (resp != null && resp.Flag == 0)
                {
                    if (ShopID.HasValue)
                    {
                        var PIDList = from o in resp.Data.ItemList select o.ProductId;
                        var promotionserviceCenter = WorkContext.CreatePromotionSdkClient();
                        //如果有shopid值，则更新平台费率
                        var promotionDetailsperc = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                        {
                            WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                            ShopID = ShopID,
                            ProductIDList = string.Join(",", PIDList.ToArray()),
                            PromotionType = 2
                        });
                        foreach (var item in resp.Data.ItemList)
                        {
                            if (promotionDetailsperc.Data != null)
                            {
                                var temp = promotionDetailsperc.Data.FirstOrDefault(o => o.ProductID == item.ProductId);
                                if (temp != null)
                                {
                                    item.ShopAddPerc = temp.Point;
                                }
                            }
                        }
                    }


                    var pdidList = from o in resp.Data.ItemList select o.ProductId;

                    var widList = new List<int> { WorkContext.CurrentWarehouse.Parent.WarehouseId };
                    var stockQuery = new FrxsErpOrderStockQtyListQueryRequest
                    {
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName,
                        PDIDList = pdidList.ToList(),
                        WIDList = widList
                    };

                    //获取库存列表
                    var respstock = WorkContext.CreateOrderSdkClient().Execute(stockQuery);
                    if (respstock != null && respstock.Data != null)
                    {
                        foreach (var item in resp.Data.ItemList)
                        {
                            ServiceCenter.Order.SDK.Resp.FrxsErpOrderStockQtyListQueryResp.FrxsErpOrderStockQtyListQueryRespData frxsErpOrderStockQtyListQueryRespData = null;

                            if (subWId > 0)
                            {
                                frxsErpOrderStockQtyListQueryRespData = respstock.Data.FirstOrDefault(i => i.PID == item.ProductId && i.SubWID == subWId);
                            }
                            else
                            {
                                frxsErpOrderStockQtyListQueryRespData = respstock.Data.FirstOrDefault(i => i.PID == item.ProductId);
                            }

                            if (frxsErpOrderStockQtyListQueryRespData != null)
                            {
                                item.WStock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            }
                        }
                    }



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
        /// 获取仓库配送商品信息 去掉门店限购商品 (带库存、在途、可用数量) 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="type"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="SKULikeSearch"></param>
        /// <returns></returns>
        public ActionResult GetSaleProductStockInfo(string Value, string type, int SubWID, int ShopID, int rows = 30, int page = 1, bool? SKULikeSearch = null)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetToB2BRequest()
                {
                    IsPage = 1,
                    PageIndex = page,
                    PageSize = rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKU = (type == "SKU") ? Value : null,
                    ProductName = (type == "ProductName") ? Value : null,
                    ShopID = ShopID
                });
                if (resp != null && resp.Flag == 0)
                {
                    List<SaleProductExt> saleProductExts = new List<SaleProductExt>();
                    var PIDList = from o in resp.Data.ItemList select o.ProductId;

                    var OrderServiceCenter = WorkContext.CreateOrderSdkClient();
                    var OrderStockQty = OrderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyQueryRequest()
                    {
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        SubWID = SubWID,
                        PDIDList = PIDList.ToList()
                    });

                    var promotionserviceCenter = WorkContext.CreatePromotionSdkClient();
                    //门店促销积分接口
                    var promotionDetailspoint = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                    {
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        BeginTime = DateTime.Now,
                        EndTime = DateTime.Now,
                        ShopID = ShopID,
                        ProductIDList = string.Join(",", PIDList.ToArray()),
                        PromotionType = 1
                    });
                    //平台费率促销接口
                    var promotionDetailsperc = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                    {
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        ShopID = ShopID,
                        ProductIDList = string.Join(",", PIDList.ToArray()),
                        PromotionType = 2
                    });

                    foreach (var item in resp.Data.ItemList)
                    {
                        var tempproduct = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<SaleProductExt>(item);
                        if (OrderStockQty.Data != null)
                        {
                            var temp = OrderStockQty.Data.StockQtyList.FirstOrDefault(o => o.PID == item.ProductId);
                            if (temp != null)
                            {
                                tempproduct.Stock = temp.StockQty;
                                tempproduct.OnTheWay = temp.PreQty;
                                tempproduct.Available = temp.EnQty;
                            }
                        }
                        if (promotionDetailspoint.Data != null && promotionDetailspoint.Data.Count > 0)
                        {
                            var temp = promotionDetailspoint.Data.FirstOrDefault(o => o.ProductID == item.ProductId);
                            if (temp != null)
                            {
                                //int packingQty = int.Parse(temp.PackingQty.ToString());
                                tempproduct.BigShopPoint = temp.Point * temp.PackingQty.Value;
                                tempproduct.MaxOrderQty = temp.MaxOrderQty;
                            }
                        }
                        if (promotionDetailsperc.Data != null && promotionDetailsperc.Data.Count > 0)
                        {
                            var temp = promotionDetailsperc.Data.FirstOrDefault(o => o.ProductID == item.ProductId);

                            if (temp != null)
                            {
                                tempproduct.ShopAddPerc = temp.Point;
                            }
                        }
                        saleProductExts.Add(tempproduct);
                    }
                    var obj = new { total = resp.Data.TotalRecords, rows = saleProductExts };
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


        public class SaleProductExt : Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductsGetToB2BResp.WProductExt
        {
            //库存数量
            public decimal Stock { get; set; }

            //在途数量
            public decimal OnTheWay { get; set; }

            //可用数量
            public decimal Available { get; set; }

        }

        /// <summary>
        /// 获取仓库配送商品信息 包括限购商品 (带库存、在途、可用数量) 
        /// </summary>
        /// <param name="Value"></param>
        /// <param name="type"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <param name="SKULikeSearch"></param>
        /// <returns></returns>
        public ActionResult GetSaleProductStockXGInfo(string Value, string type, int SubWID, int rows = 30, int page = 1, bool? SKULikeSearch = null)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetToB2BExtRequest()
                {
                    IsPage = 1,
                    PageIndex = page,
                    PageSize = rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKU = (type == "SKU") ? Value : null,
                    ProductName = (type == "ProductName") ? Value : null
                });
                if (resp != null && resp.Flag == 0)
                {
                    List<SaleProductExt> saleProductExts = new List<SaleProductExt>();
                    var PIDList = from o in resp.Data.ItemList select o.ProductId;

                    var OrderServiceCenter = WorkContext.CreateOrderSdkClient();
                    var OrderStockQty = OrderServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyQueryRequest()
                    {
                        WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        SubWID = SubWID,
                        PDIDList = PIDList.ToList()
                    });

                    //var promotionserviceCenter = WorkContext.CreatePromotionSdkClient();
                    ////门店促销积分接口
                    //var promotionDetailspoint = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                    //{
                    //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    //    BeginTime = DateTime.Now,
                    //    EndTime = DateTime.Now,
                    //    ShopID = ShopID,
                    //    ProductIDList = string.Join(",", PIDList.ToArray()),
                    //    PromotionType = 1
                    //});
                    ////平台费率促销接口
                    //var promotionDetailsperc = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                    //{
                    //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    //    ShopID = ShopID,
                    //    ProductIDList = string.Join(",", PIDList.ToArray()),
                    //    PromotionType = 2
                    //});

                    foreach (var item in resp.Data.ItemList)
                    {
                        var tempproduct = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<SaleProductExt>(item);
                        if (OrderStockQty.Data != null)
                        {
                            var temp = OrderStockQty.Data.StockQtyList.FirstOrDefault(o => o.PID == item.ProductId);
                            if (temp != null)
                            {
                                tempproduct.Stock = temp.StockQty;
                                tempproduct.OnTheWay = temp.PreQty;
                                tempproduct.Available = temp.EnQty;
                            }
                        }
                        //if (promotionDetailspoint.Data != null && promotionDetailspoint.Data.Count > 0)
                        //{
                        //    var temp = promotionDetailspoint.Data.FirstOrDefault(o => o.ProductID == item.ProductId);
                        //    if (temp != null)
                        //    {
                        //        int packingQty = int.Parse(temp.PackingQty.ToString());
                        //        tempproduct.BigShopPoint = temp.Point * packingQty;
                        //        tempproduct.MaxOrderQty = temp.MaxOrderQty;
                        //    }
                        //}
                        //if (promotionDetailsperc.Data != null && promotionDetailsperc.Data.Count > 0)
                        //{
                        //    var temp = promotionDetailsperc.Data.FirstOrDefault(o => o.ProductID == item.ProductId);

                        //    if (temp != null)
                        //    {
                        //        tempproduct.ShopAddPerc = temp.Point;
                        //    }
                        //}
                        saleProductExts.Add(tempproduct);
                    }
                    var obj = new { total = resp.Data.TotalRecords, rows = saleProductExts };
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
        /// 获取供应商信息
        /// </summary>
        /// <param name="VendorCode"></param>
        /// <param name="VendorName"></param>
        /// <param name="rows"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        public ActionResult GetVendorInfo(string VendorCode, string VendorName, int rows, int page)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductVendorTableListRequest()
                {
                    PageIndex = page,
                    PageSize = rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    VendorCode = (string.IsNullOrEmpty(VendorCode) ? null : VendorCode),
                    VendorName = (string.IsNullOrEmpty(VendorName) ? null : VendorName),
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
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
        /// 获取仓库后台门店信息（非冻结）
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public ActionResult GetShopInfo(string shopCode, string shopName, int rows, int page)
        {
            string result = string.Empty;
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopWarehouseTableListRequest()
                {
                    PageIndex = page,
                    PageSize = rows,
                    Status = "1",
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString(),
                    ShopCode = (string.IsNullOrEmpty(shopCode) ? null : shopCode),
                    ShopName = (string.IsNullOrEmpty(shopName) ? null : shopName),
                    SortBy = "shopCode asc"
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
        /// 获取仓库信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWCList()
        {
            string result = string.Empty;
            try
            {
                result = "[";
                if (WorkContext.CurrentWarehouse.Parent.WarehouseId == WorkContext.CurrentWarehouse.WarehouseId)
                {
                    //仓库结点
                    var subWarehouses = WorkContext.CurrentWarehouse.ParentSubWarehouses;
                    for (int i = 0; i < subWarehouses.Count; i++)
                    {
                        result += "{";
                        result += "\"WName\":\"" + string.Format("【{0}】{1}_{2}", subWarehouses[i].WarehouseCode, WorkContext.CurrentWarehouse.Parent.WarehouseName, subWarehouses[i].WarehouseName) + "\",";
                        result += "\"WID\":\"" + subWarehouses[i].WarehouseId + "\"";
                        result += "},";
                    }
                }
                else
                {
                    result += "{";
                    result += "\"WName\":\"" + string.Format("【{0}】{1}_{2}", WorkContext.CurrentWarehouse.WarehouseCode, WorkContext.CurrentWarehouse.Parent.WarehouseName, WorkContext.CurrentWarehouse.WarehouseName) + "\",";
                    result += "\"WID\":\"" + WorkContext.CurrentWarehouse.WarehouseId + "\"";
                    result += "},";
                }
                result = result.Substring(0, result.Length - 1);
                result += "]";
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
        /// 获取供应商类型
        /// </summary>
        /// <returns></returns>
        public ActionResult GetVendorTypes()
        {
            string result = string.Empty;
            try
            {
                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new FrxsErpProductVendorTypeListGetRequest()
                {
                    PageIndex = 1,
                    PageSize = 100,
                    SortBy = "VendorTypeID"
                });
                if (resp != null && resp.Flag == 0)
                {
                    result = resp.Data.ItemList.ToJsonString();
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
        /// 获取门店下拉数据（信用等级、结算方式）
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public ActionResult GetVendorDllInfo(string dictType)
        {
            string result = string.Empty;
            try
            {
                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new FrxsErpProductSysDictDetailGetListRequest()
                {
                    dictCode = dictType
                });
                if (resp != null && resp.Flag == 0)
                {
                    result = resp.Data.ToJsonString();
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
        /// 通过父节点编号得到子节点的基本分类列表数据
        /// </summary>
        /// <param name="pcategoryId">父基本分类编号,取第一级基本分类编号传递null值</param>
        /// <returns></returns>
        public ActionResult GetChirldCategories(int? pcategoryId)
        {
            string jsonStr = "[]";
            FrxsErpProductCategoriesChildsGetRequest r = new FrxsErpProductCategoriesChildsGetRequest
            {
                CategoryId = pcategoryId.HasValue ? pcategoryId.Value : (int?)null,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            };

            //获取列表
            var resp = WorkContext.CreateProductSdkClient().Execute(r);
            //获取分类List解析的对象
            if (resp != null && resp.Data != null && resp.Data.Count > 0)
            {
                jsonStr = resp.Data.ToJsonString();
            }
            return Content(jsonStr);
        }




        /// <summary>
        /// 通过父节点编号得到子节点的运营分类列表数据
        /// </summary>    
        /// <param name="pshopcategoryId">父运营分类编号,取第一级运营分类编号传递null值</param>
        /// <returns></returns>
        public ActionResult GetChirldShopCategories(int? pshopcategoryId)
        {
            string jsonStr = "[]";
            FrxsErpProductShopCategoriesChildsGetRequest r = new FrxsErpProductShopCategoriesChildsGetRequest
            {
                CategoryId = pshopcategoryId.HasValue ? pshopcategoryId.Value : (int?)null,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            };

            //获取列表
            var resp = WorkContext.CreateProductSdkClient().Execute(r);
            //获取分类List解析的对象
            if (resp != null && resp.Data != null && resp.Data.Count > 0)
            {
                jsonStr = resp.Data.ToJsonString();
            }
            return Content(jsonStr);
        }
        /// <summary>
        /// 通过参数编号和网仓编号 获取网仓参数信息
        ///
        /// </summary>
        /// <param name="paramCode">参数编码</param>
        /// <returns></returns>
        public ActionResult GetWarehouseSysParams(string paramCode)
        {
            string jsonStr = "[]";
            var resp = CommModel.GetWarehouseSysParams(paramCode);
            if (resp != null)
            {
                jsonStr = resp.ToJsonString();
            }
            return Content(jsonStr);
        }


        /// <summary>
        /// 消息类型
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public ActionResult GetMessageType()
        {
            string result = string.Empty;
            try
            {
                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new FrxsErpProductSysDictDetailGetListRequest()
                {
                    dictCode = "MessageType"
                });
                if (resp != null && resp.Flag == 0)
                {
                    result = resp.Data.ToJsonString();
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
        /// 费用项
        /// </summary>
        /// <param name="dictType"></param>
        /// <returns></returns>
        public ActionResult GetSaleFee()
        {
            string result = string.Empty;
            try
            {
                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new FrxsErpProductSysDictDetailGetListRequest()
                {
                    dictCode = "SaleFeeCode"
                });
                if (resp != null && resp.Flag == 0)
                {
                    result = resp.Data.ToJsonString();
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
        /// 获取送货线路
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWarehouseLineList()
        {
            string jsonStr = "[]";
            //获取列表
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            //获取分类List解析的对象

            if (resp != null && resp.Data != null && resp.Data.ItemList.Count > 0)
            {
                jsonStr = resp.Data.ItemList.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 获取系统地区(省市县)信息
        /// 根据入参 分3种情况查询 1.按照ID查确定的记录 2.按照父级ID查某个地区的子区域 3.按照 区域级别查(1:省;2:市;3:区)
        /// 未按照规则传入参则返回所有信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetSysArea(string searchType, int searchValue)
        {
            string jsonStr = "[]";
            //创建客户端SDK访问对象
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var requestDto = new FrxsErpProductSysAreaGetRequest();
            switch (searchType)
            {
                case "AreaID":
                    requestDto.AreaID = searchValue;
                    break;
                case "ParentID":
                    requestDto.ParentID = searchValue;
                    break;
                case "Level":
                    requestDto.Level = searchValue;
                    break;
                default:
                    break;
            }
            //调用接口查询地区信息
            var resp = serviceCenter.Execute(requestDto);
            //获取地区信息对象
            if (resp != null && resp.Data != null && resp.Data.ToList().Count > 0)
            {
                jsonStr = resp.Data.ToList().ToJsonString();
            }
            return Content(jsonStr);
        }
    }
}
