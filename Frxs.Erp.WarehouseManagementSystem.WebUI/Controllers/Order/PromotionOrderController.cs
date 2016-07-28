using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Request;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Resp;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Platform.Utility.Map;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    [ValidateInput(false)]
    public class PromotionOrderController : BaseController
    {
        #region 视图
        /// <summary>
        /// 订货平台订单列表视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionOrderList()
        {
            return View();
        }

        /// <summary>
        /// 订货平台订单  视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PromotionOrderAddOrEdit()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 批量取消订单
        /// </summary>
        /// <param name="OrderId"></param>
        /// <param name="Status"></param>
        /// <param name="CloseReason"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520311, 52031101)]
        public ActionResult PromotionOrderCancel(string orderIdList, int Status, string CloseReason)
        {
            string result = string.Empty;
            try
            {
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                var resp = this.ErpPromotionSdkClient.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionOrderShopCancelRequest()
                {
                    OrderIdList = orderIdList.Split(',').ToList(),
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    Status = Status,
                    CloseReason = CloseReason
                });
                if (resp != null && resp.Flag == 0)
                {
                    if (resp.Data)
                    {
                        //写操作日志
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3A,
                                               ConstDefinition.XSOperatorActionCancel, string.Format("{0}订货平台预定单号[{1}]", ConstDefinition.XSOperatorActionCancel, orderIdList));

                        //写订单跟踪
                        var part = new OrderPartsServer();
                        var Orders = orderIdList.Split(',').ToList();
                        foreach (var item in Orders)
                        {
                            var track = new FrxsErpOrderSaleOrderAddTrackRequest()
                            {
                                CreateTime = DateTime.Now,
                                CreateUserID = userid,
                                CreateUserName = username,
                                IsDisplayUser = 1,
                                OrderID = item,
                                OrderStatus = Status,
                                Remark = "客服人员帮您取消了订单，交易已经关闭"
                            };
                            var bFlag = part.InsertOrderTrack(track, warehouseId, userid, username);
                        }
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "OK"
                        }.ToJsonString();
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "False"
                        }.ToJsonString();
                    }
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "False"
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
        /// 获取订货平台订单详情
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public ActionResult GetPromotionOrderInfo(string OrderID)
        {
            string result = string.Empty;
            var orderClient = WorkContext.CreatePromotionSdkClient();
            //从门店订单表取数据
            var resultdto = new FrxsErpPromotionOrderShopGetResp.FrxsErpPromotionOrderShopGetRespData();
            var orderResp = orderClient.Execute(new FrxsErpPromotionOrderShopGetRequest()
            {
                OrderId = OrderID,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            if (orderResp.Flag != 0)
            {
                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = orderResp.Info
                }.ToJsonString();
                return Content(result);
            }
            else
            {
                foreach (var item in orderResp.Data.Details)
                {
                    item.SubAmt = item.SubAmt + item.SubAddAmt;
                }
                resultdto = orderResp.Data;
                result = new { Order = orderResp.Data.Order, Details = orderResp.Data.Details }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 确认订单
        /// </summary>
        /// <param name="orderIdList"></param>
        /// <param name="Status"></param>
        /// <param name="CloseReason"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520311, 52031102)]
        public ActionResult PromotionOrderSure(string OrderId)
        {
            string result = string.Empty;
            try
            {
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                var orderClient = WorkContext.CreatePromotionSdkClient();
                //从门店订单表取数据
                var resultdto = new FrxsErpPromotionOrderShopGetResp.FrxsErpPromotionOrderShopGetRespData();
                var picks = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailsPickRequestDto>();
                var shelfAreas = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreShelfAreaRequestDto>();

                var orderResp = orderClient.Execute(new FrxsErpPromotionOrderShopGetRequest()
                {
                    OrderId = OrderId,
                    WarehouseId = warehouseId
                });
                if (orderResp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = orderResp.Info
                    }.ToJsonString();
                    return Content(result);
                }
                else
                {
                    resultdto = orderResp.Data;
                }

                //状态监测
                if (resultdto.Order.Status != 1)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "状态错误，不能进行确认！"
                    }.ToJsonString();
                    return Content(result);
                }
                int subID = resultdto.Order.SubWID.Value;
                int shopID = resultdto.Order.ShopID;

                var productIdList = (from o in resultdto.Details select o.ProductId).ToList();

                var part = new OrderPartsServer();

                //限购检查
                #region 限购检查
                var list = new List<WarehouseOrderDetails>();
                foreach (var detail in resultdto.Details)
                {
                    list.Add(new WarehouseOrderDetails()
                    {
                        ProductId = detail.ProductId,
                        SaleQty = detail.PreQty,
                        ProductName = detail.ProductName
                    });
                }
                string msg = "";
                List<Frxs.Erp.WarehouseManagementSystem.WebUI.OrderPartsServer.SimpleProdcutQuoteModel> maxList = new List<Frxs.Erp.WarehouseManagementSystem.WebUI.OrderPartsServer.SimpleProdcutQuoteModel>();
                var flag = part.QuotaCheck(shopID, warehouseId, productIdList, list, userid, username, ref msg, maxList);
                if (!flag)//有限购商品超过限制
                {
                    foreach (var detail in resultdto.Details)
                    {
                        var m = maxList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                        if (m != null && detail.SaleQty > m.MaxPreQty && m.MaxPreQty != 0)
                        {
                            detail.PreQty = m.MaxPreQty;
                        }
                    }
                }
                #endregion

                //重处理商品价格和积分运算,新增商品货架表记录、商品分类表记录
                #region 重处理商品价格和积分运算,新增商品货架表记录、商品分类表记录
                var wProudcts = part.GetWProductsInfo(warehouseId, productIdList, shopID, userid, username);
                var promotionProducts = part.GetPromotionRebate(warehouseId, shopID, productIdList, userid, username);
                var productStock = part.GetProductStock(warehouseId, subID, productIdList, userid, username);
                var rProducts = part.GetPromotionRate(warehouseId, shopID, productIdList, userid, username);

                //移除限购商品
                if (wProudcts.ItemList.Count != resultdto.Details.GroupBy(x => x.ProductId).Count())
                {
                    for (int i = resultdto.Details.Count - 1; i >= 0; i--)
                    {
                        var detail = resultdto.Details[i];
                        var tmp = wProudcts.ItemList.FirstOrDefault(x => x.ProductId == detail.ProductId);
                        if (tmp == null)
                        {
                            resultdto.Details.Remove(detail);
                            resultdto.DetailExts.RemoveAt(i);
                        }
                    }
                }

                if (resultdto.Details.Count <= 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "订单所有商品均已被限制购买"
                    }.ToJsonString();
                    return Content(result);
                }


                foreach (var detail in resultdto.Details)
                {
                    var wProduct = wProudcts.ItemList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                    var pProduct = promotionProducts.Where(x => x.ProductID == detail.ProductId).FirstOrDefault();
                    var stock = productStock.StockQtyList.Where(x => x.PID == detail.ProductId).FirstOrDefault();
                    var rProduct = rProducts.Where(x => x.ProductID == detail.ProductId).FirstOrDefault();

                    if (wProduct == null)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("仓库中没有找到商品{0}", detail.ProductName)
                        }.ToJsonString();
                        return Content(result);
                    }

                    if (wProduct.WStatus != 1)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("商品{0}已下架，不能提交订单，请手工删除该商品后再确认订单", wProduct.ProductName)
                        }.ToJsonString();
                        return Content(result);
                    }


                    detail.ProductName = wProduct.ProductName;
                    detail.ProductImageUrl200 = wProduct.ImageUrl200x200;
                    detail.ProductImageUrl400 = wProduct.ImageUrl400x400;
                    detail.Unit = wProduct.Unit;
                    detail.BasePoint = wProduct.BasePoint.HasValue ? wProduct.BasePoint.Value : 0;
                    detail.SalePackingQty = wProduct.BigPackingQty.HasValue ? wProduct.BigPackingQty.Value : 1;
                    detail.SalePrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0) * (wProduct.BigPackingQty.HasValue ? wProduct.BigPackingQty.Value : 1);
                    detail.SaleQty = detail.PreQty;
                    detail.SaleUnit = wProduct.BigUnit;
                    //detail.ShopAddPerc = wProduct.ShopAddPerc.HasValue ? wProduct.ShopAddPerc.Value : 0;
                    if (rProduct == null)
                    {
                        detail.ShopAddPerc = wProduct.ShopAddPerc.HasValue ? wProduct.ShopAddPerc.Value : 0;
                    }
                    else
                    {
                        //detail.PromotionID = rProduct.PromotionID;
                        //detail.PromotionName = rProduct.ProductName;
                        detail.ShopAddPerc = rProduct.Point;
                    }
                    detail.ShopPoint = wProduct.ShopPoint.HasValue ? wProduct.ShopPoint.Value : 0;
                    detail.UnitPrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0);
                    detail.UnitQty = (wProduct.BigPackingQty.HasValue
                                                         ? wProduct.BigPackingQty.Value
                                                         : 1) * detail.PreQty;
                    detail.VendorPerc1 = wProduct.VendorPerc1.HasValue ? wProduct.VendorPerc1.Value : 0;
                    detail.VendorPerc2 = wProduct.VendorPerc2.HasValue ? wProduct.VendorPerc2.Value : 0;

                    detail.IsStockOut = stock == null ? 1 : ((detail.PreQty * wProduct.BigPackingQty) > stock.StockQty ? 1 : 0);

                    if (pProduct != null)
                    {
                        detail.PromotionID = pProduct.PromotionID;
                        detail.PromotionName = pProduct.ProductName;
                        detail.PromotionShopPoint = pProduct.Point;
                        //detail.SubPoint = detail.PromotionShopPoint * detail.UnitQty;
                        if (detail.PromotionShopPoint > 0)
                        {
                            detail.SubPoint = detail.PromotionShopPoint * detail.UnitQty;
                        }
                        else
                        {
                            detail.SubPoint = detail.ShopPoint * detail.UnitQty;
                        }
                    }
                    else
                    {
                        detail.PromotionShopPoint = 0;
                        detail.SubPoint = detail.ShopPoint * detail.UnitQty;
                    }
                    detail.PromotionUnitPrice = detail.UnitPrice;
                    detail.SubAmt = detail.SalePrice * detail.PreQty; //没有促销价格，按原价计算

                    detail.SubAddAmt = detail.SubAmt * detail.ShopAddPerc;
                    detail.SubBasePoint = detail.BasePoint * detail.SubAmt;
                    detail.SubVendor1Amt = detail.VendorPerc1 * detail.SubAmt * 0.01m;
                    detail.SubVendor2Amt = detail.VendorPerc2 * detail.SubAmt * 0.01m;

                    var pick = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailsPickRequestDto()
                    {
                        BarCode = detail.BarCode,
                        CheckTime = null,
                        CheckUserID = null,
                        CheckUserName = null,
                        ID = warehouseId.ToString() + Guid.NewGuid(),
                        IsAppend = 0,
                        IsCheckRight = null,
                        ModifyTime = DateTime.Now,
                        ModifyUserID = userid,
                        ModifyUserName = username,
                        OrderID = detail.OrderID,
                        ProductID = detail.ProductId,
                        PickQty = null,
                        PickTime = null,
                        PickUserID = null,
                        PickUserName = null,
                        ProductImageUrl200 = detail.ProductImageUrl200,
                        ProductImageUrl400 = detail.ProductImageUrl400,
                        ProductName = detail.ProductName,
                        Remark = detail.Remark,
                        SKU = detail.SKU,
                        SalePackingQty = detail.SalePackingQty,
                        SaleQty = detail.SaleQty.Value,
                        SaleUnit = detail.SaleUnit,
                        ShelfAreaID = wProduct.ShelfAreaID,
                        ShelfCode = wProduct.ShelfCode,
                        ShelfID = wProduct.ShelfID,
                        UserId = userid,
                        UserName = username,
                        WCProductID = (int)wProduct.WProductID,
                        WarehouseId = warehouseId
                    };
                    picks.Add(pick);

                    var tmpArea = shelfAreas.FirstOrDefault(x => x.ShelfAreaID == pick.ShelfAreaID);
                    if (tmpArea == null)
                    {
                        var area = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreShelfAreaRequestDto()
                        {
                            BeginTime = null,
                            EndTime = null,
                            ID = warehouseId.ToString() + Guid.NewGuid(),
                            ModifyTime = DateTime.Now,
                            ModifyUserID = userid,
                            ModifyUserName = username,
                            OrderID = detail.OrderID,
                            Package1Qty = null,
                            Package2Qty = null,
                            Package3Qty = null,
                            PickUserID = null,
                            PickUserName = null,
                            Remark = 0,
                            ShelfAreaID = pick.ShelfAreaID.Value,
                            ShelfAreaCode = string.IsNullOrEmpty(wProduct.ShelfAreaCode) ? "" : wProduct.ShelfAreaCode,
                            ShelfAreaName = string.IsNullOrEmpty(wProduct.ShelfAreaName) ? "" : wProduct.ShelfAreaName,
                            UserId = userid,
                            UserName = username,
                            WID = warehouseId,
                            WarehouseId = warehouseId,
                        };
                        shelfAreas.Add(area);
                    }

                }
                #endregion

                //重新计算订单数据
                #region 重新计算订单数据
                resultdto.Order.CouponAmt = 0;
                resultdto.Order.TotalAddAmt = resultdto.Details.Sum(x => x.SubAddAmt);
                resultdto.Order.TotalPoint = resultdto.Details.Where(x => x.SubPoint.HasValue).Sum(x => x.SubPoint.Value);
                resultdto.Order.TotalProductAmt = resultdto.Details.Where(x => x.SubAmt.HasValue).Sum(x => x.SubAmt.Value);
                resultdto.Order.PayAmount = resultdto.Order.TotalAddAmt.Value - resultdto.Order.CouponAmt + resultdto.Order.TotalProductAmt;
                resultdto.Order.TotalBasePoint = resultdto.Details.Sum(x => x.SubBasePoint);
                resultdto.Order.Status = 2;   //改变订单状态为 等待捡货
                resultdto.Order.ConfDate = DateTime.Now;
                #endregion

                //此处将门店订单数据写入仓库订单数据begin
                var orderWriteClient = WorkContext.CreateOrderSdkClient();
                var tmpOrder = AutoMapperHelper.MapTo<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreRequestDto>(resultdto.Order);
                var tmpDetails = AutoMapperHelper.MapToList<FrxsErpPromotionOrderShopGetResp.OrderDetailRequestDto, FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailRequestDto>(resultdto.Details);
                var tmpDetailExts = AutoMapperHelper.MapToList<FrxsErpPromotionOrderShopGetResp.OrderDetailExtsRequestDto, FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailExtsRequestDto>(resultdto.DetailExts);

                var sendNumber = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderSendNumberRequestDto();
                sendNumber.OrderID = tmpOrder.OrderId;
                sendNumber.ModifyTime = DateTime.Now;
                sendNumber.ModifyUserID = userid;
                sendNumber.ModifyUserName = username;
                sendNumber.WID = tmpOrder.WID;
                sendNumber.WarehouseId = warehouseId;
                sendNumber.SendNumber = 999;
                var shop = part.GetShopInfo(tmpOrder.ShopID, userid, username);
                if (shop == null)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "没有找到门店信息"
                    }.ToJsonString();
                    return Content(result);
                }
                if (shop.IsDeleted == 1)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "已删除的门店"
                    }.ToJsonString();
                    return Content(result);
                }
                if (shop.Status == 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "门店已被冻结"
                    }.ToJsonString();
                    return Content(result);
                }

                var line = part.GetWLineInfo(shop.LineID, userid, username);
                if (line == null)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "取线路信息失败"
                    }.ToJsonString();
                    return Content(result);
                }
                sendNumber.ShopSerialNumber = shop.SerialNumber;
                sendNumber.LineSerialNumber = line.SerialNumber;

                var orderWriteReq = new FrxsErpOrderSaleOrderPreAddOrEditRequest()
                {
                    SaleOrderPreDetailList = tmpDetails,
                    SaleOrderPreDetailExtsList = tmpDetailExts,
                    Flag = 0,
                    SaleOrderPre = tmpOrder,
                    SendNumber = sendNumber,
                    Picks = picks,
                    ShelfAreas = shelfAreas,
                    WarehouseId = warehouseId
                };
                var orderWriteResp = orderWriteClient.Execute(orderWriteReq);
                if (orderWriteResp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = orderWriteResp.Info
                    }.ToJsonString();
                    return Content(result);
                }
                //此处写仓库订单数据end

                //此处回写门店订单状态
                var client = WorkContext.CreatePromotionSdkClient();
                var resp = client.Execute(new FrxsErpPromotionOrderShopConfirmRequest()
                {
                    OrderId = OrderId,
                    ShopId = shopID,
                    WarehouseId = warehouseId
                });
                if (resp.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    }.ToJsonString();
                    return Content(result);
                }
                else
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3A,
                      ConstDefinition.XSOperatorActionSure, string.Format("{0}订货平台预定单号[{1}]", ConstDefinition.XSOperatorActionSure, OrderId));

                    var track = new FrxsErpOrderSaleOrderAddTrackRequest()
                    {
                        CreateTime = DateTime.Now,
                        CreateUserID = userid,
                        CreateUserName = username,
                        IsDisplayUser = 1,
                        OrderID = OrderId,
                        OrderStatus = 2,
                        Remark = "您的订单已经确认，等待拣货"
                    };
                    var bFlag = part.InsertOrderTrack(track, warehouseId, userid, username);
                    if (bFlag)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "OK"
                        }.ToJsonString();
                        return Content(result);
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "添加订单跟踪消息失败"
                        }.ToJsonString();
                        return Content(result);
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
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        public ActionResult GetPromotionOrderList(PromotionOrderSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new PromotionOrderModel().GetPromotionOrderPageData(searchModel);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

    }
}
