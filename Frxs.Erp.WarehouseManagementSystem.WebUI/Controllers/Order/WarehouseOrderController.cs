using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Resp;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Request;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Excel;
using System.Dynamic;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.ServiceCenter.SSO.Client;
using System.Data;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    [ValidateInput(false)]
    public class WarehouseOrderController : BaseController
    {
        #region 视图
        /// <summary>
        /// 仓库订单列表视图
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseOrderList()
        {
            return View();
        }

        /// <summary>
        /// 仓库订单 选择配送商品 视图 带库存、在途、可用数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchSaleProductStock()
        {
            return View();
        }

        /// <summary>
        /// 开始装箱 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PackStart()
        {
            return View();
        }

        /// <summary>
        /// 拣货 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PickFinish()
        {
            return View();
        }

        /// <summary>
        /// 待拣货详细商品 或者 对货商品列表
        /// </summary>
        /// <param name="OrderID"></param>
        /// <returns></returns>
        public ActionResult GetWaitPickDetails(string OrderID)
        {
            string result = string.Empty;
            try
            {
                var orderClient = WorkContext.CreateOrderSdkClient();
                var respWaitPickDetails = orderClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetWaitPickDetailsRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString(),
                    OrderID = OrderID
                });
                if (respWaitPickDetails != null && respWaitPickDetails.Flag == 0)
                {
                    var Products = respWaitPickDetails.Data.WaitPickDetailsData.ProductData.OrderBy(q => q.ShelfCode).ToList();
                    var obj = new { total = Products.Count, rows = Products };
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
        /// 仓库订单新增或编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult WarehouseOrderAddOrEdit()
        {
            return View();
        }

        /// <summary>
        /// 订单详情视图
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderDetails(string OrderID)
        {
            int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
            int userid = WorkContext.UserIdentity.UserId;
            string username = WorkContext.UserIdentity.UserName;

            //1.获取订单信息
            var Order = new FrxsErpOrdervSaleOrderGetResp.SaleOrderPreResponseDto();
            var OrderDetails = new List<FrxsErpOrdervSaleOrderGetResp.SaleOrderPreDetailRequestDto>();
            var respOrder = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrdervSaleOrderGetRequest()
            {
                OrderId = OrderID,
                WarehouseId = warehouseId
            });
            if (respOrder != null && respOrder.Flag == 0)
            {
                Order = respOrder.Data.Order;
                OrderDetails = respOrder.Data.Details.ToList();
            }
            Order.StockOutRate = (Order.StockOutRate == null ? 0m : Order.StockOutRate.Value) * 100;   //更新缺货率
            double sumQty = 0D;  //拣货数量
            double sumPreQty = 0D; //订单数量
            OrderDetails.ForEach(x =>
            {
                sumQty += Convert.ToDouble(x.SaleQty);
                sumPreQty += Convert.ToDouble(x.PreQty);
            });

            //2.获取跟踪表信息
            var Track = new List<FrxsErpOrderSaleOrderGetTrackResp.OrderTrackGetResponseDto>();
            var respTrack = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderGetTrackRequest()
            {
                OrderId = OrderID,
                WarehouseId = warehouseId
            });
            if (respTrack != null && respTrack.Flag == 0)
            {
                Track = respTrack.Data.Tracks.OrderByDescending(o => o.CreateTime).ToList();
            }
            //3.获取结算单信息
            var SaleSettle = new List<FrxsErpOrderSaleSettlePrinteGetModelResp.SaleSettleDetail>();
            var respSaleSettle = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettlePrinteGetModelRequest()
            {
                WID = warehouseId,
                OrderId = OrderID
            });
            if (respSaleSettle != null && respSaleSettle.Flag == 0)
            {
                SaleSettle = respSaleSettle.Data.SaleSettleDetailList.ToList();
            }
            //4.商品详情信息  GetDeliverProductInfoAction
            var DeliverProduct = new List<FrxsErpOrderGetDeliverProductInfoExtResp.ProductDetailExt>();
            var respDeliverProduct = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetDeliverProductInfoExtRequest()
            {
                WID = warehouseId.ToString(),
                OrderId = OrderID
            });
            if (respDeliverProduct != null && respDeliverProduct.Flag == 0)
            {
                DeliverProduct = respDeliverProduct.Data.ProductData.ToList();
            }
            //5.装箱信息   GetDeliverOrderInfo
            var DeliverOrder = new FrxsErpOrderGetDeliverOrderInfoExtResp.FrxsErpOrderGetDeliverOrderInfoExtRespData();
            var respDeliverOrder = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetDeliverOrderInfoExtRequest()
            {
                WID = warehouseId.ToString(),
                OrderId = OrderID
            });
            if (respDeliverOrder != null && respDeliverOrder.Flag == 0)
            {
                DeliverOrder = respDeliverOrder.Data;
            }

            dynamic viewModel = new ExpandoObject();
            viewModel.Order = Order;
            viewModel.Track = Track;
            viewModel.sumQty = sumQty;
            viewModel.sumPreQty = sumPreQty;

            if (SaleSettle != null && SaleSettle.Count > 0)
            {
                viewModel.SettleID = SaleSettle[0].SettleID;
                viewModel.SaleSettle = SaleSettle.ToJsonString();
            }
            else
            {
                viewModel.SettleID = "";
                viewModel.SaleSettle = "";

            }
            viewModel.DeliverProducts = DeliverProduct.OrderBy(o=>o.ShelfCode).ToJsonString();
            viewModel.DeliverOrder = DeliverOrder;
            return View(viewModel);
        }


        /// <summary>
        /// 修改当次线路视图
        /// </summary>
        /// <returns></returns>
        public ActionResult LineChange()
        {
            return View();
        }

        /// <summary>
        /// 装箱操作选择视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PackChoose()
        {
            return View();
        }

        /// <summary>
        /// 对货 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult PickCheck()
        {
            return View();
        }

        /// <summary>
        /// 导入 视图
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportWarehouseOrderDetail()
        {
            return View();
        }


        /// <summary>
        /// 仓库订单新增或编辑视图
        /// </summary>
        /// <returns></returns>
        public ActionResult GetOrderDetailsInfo()
        {
            return View();
        }

        #endregion

        #region 订单状态切换
        /// <summary>
        /// 取消 处理
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031204)]
        public ActionResult OrderCancel(string OrderID, int Status)
        {
            string result = string.Empty;
            try
            {
                List<string> OrderIdList = new List<string>() { OrderID };
                var orderserviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = orderserviceCenter.Execute(new FrxsErpOrderOrderPreCancelRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    OrderIdList = OrderIdList,
                    Status = Status
                });
                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      ConstDefinition.XSOperatorActionCancel, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionCancel, OrderID));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "取消成功"
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
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 确认 处理
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031203)]
        public ActionResult OrderSure(string OrderID)
        {
            string result = string.Empty;
            try
            {
                var orderClient = WorkContext.CreateOrderSdkClient();
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                //从门店订单表取数据
                var resultdto = new FrxsErpOrderSaleOrderPreGetResp.FrxsErpOrderSaleOrderPreGetRespData();
                var picks = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailsPickRequestDto>();
                var shelfAreas = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreShelfAreaRequestDto>();

                var orderResp = orderClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleOrderPreGetRequest()
                {
                    OrderId = OrderID,
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
                var part = new OrderPartsServer();
                var productIdList = (from o in resultdto.Details select o.ProductId).ToList();
                int subID = resultdto.Order.SubWID.Value;
                int shopID = resultdto.Order.ShopID;


                var wProudcts = part.GetWProductsInfo(warehouseId, productIdList, shopID, userid, username);
                //限购检查
                #region 限购检查
                var list = new List<WarehouseOrderDetails>();
                foreach (var detail in resultdto.Details)
                {
                    var wProduct = wProudcts.ItemList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                    decimal SaleQty = 0.0m;
                    if (detail.SalePackingQty == 1)   //库存单位
                    {
                        SaleQty = detail.SaleQty.Value / wProduct.BigPackingQty.Value;
                    }
                    else
                    {
                        SaleQty = detail.SaleQty.Value;
                    }
                    list.Add(new WarehouseOrderDetails()
                    {
                        ProductId = detail.ProductId,
                        SaleQty = SaleQty,
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
                        var wProduct = wProudcts.ItemList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                        decimal SaleQty = 0.0m;
                        if (detail.SalePackingQty == 1)   //库存单位
                        {
                            SaleQty = detail.SaleQty.Value / wProduct.BigPackingQty.Value;
                        }
                        else
                        {
                            SaleQty = detail.SaleQty.Value;
                        }
                        var m = maxList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                        if (m != null && SaleQty > m.MaxPreQty && m.MaxPreQty != 0)
                        {
                            if (detail.SalePackingQty == 1)  //库存单位
                            {
                                detail.PreQty = m.MaxPreQty * wProduct.BigPackingQty.Value;
                            }
                            else
                            {
                                detail.PreQty = m.MaxPreQty;
                            }
                        }
                    }
                }
                #endregion

                //重处理商品价格和积分运算,新增商品货架表记录、商品分类表记录
                #region 重处理商品价格和积分运算,新增商品货架表记录、商品分类表记录

                var promotionProducts = part.GetPromotionRebate(warehouseId, shopID, productIdList, userid, username);
                var productStock = part.GetProductStock(warehouseId, subID, productIdList, userid, username);
                var rProducts = part.GetPromotionRate(warehouseId, shopID, productIdList, userid, username);
                //移除已限购商品
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
                    //detail.Unit = wProduct.Unit;   不要读取，可能切换单位
                    detail.BasePoint = wProduct.BasePoint.HasValue ? wProduct.BasePoint.Value : 0;
                    //detail.SalePackingQty = wProduct.BigPackingQty.HasValue ? wProduct.BigPackingQty.Value : 1;  不要读取，可能切换单位
                    if (detail.SalePackingQty != 1)  //销售单位
                    {
                        detail.SalePrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0) * (wProduct.BigPackingQty.HasValue ? wProduct.BigPackingQty.Value : 1);
                    }
                    else
                    {
                        detail.SalePrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0);
                    }
                    detail.SaleQty = detail.PreQty;
                    //detail.SaleUnit = wProduct.BigUnit;  不要读取，可能切换单位
                    if (rProduct == null)
                    {
                        detail.ShopAddPerc = wProduct.ShopAddPerc.HasValue ? wProduct.ShopAddPerc.Value : 0;
                    }
                    else
                    {
                        detail.ShopAddPerc = rProduct.Point;
                    }
                    detail.ShopPoint = wProduct.ShopPoint.HasValue ? wProduct.ShopPoint.Value : 0;
                    detail.UnitPrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0);
                    detail.UnitQty = (detail.SalePackingQty) * detail.PreQty;  //不要读取，可能切换单位
                    detail.VendorPerc1 = wProduct.VendorPerc1.HasValue ? wProduct.VendorPerc1.Value : 0;
                    detail.VendorPerc2 = wProduct.VendorPerc2.HasValue ? wProduct.VendorPerc2.Value : 0;

                    detail.IsStockOut = stock == null ? 1 : ((detail.UnitQty) > stock.StockQty ? 1 : 0);

                    if (pProduct != null)
                    {
                        detail.PromotionID = pProduct.PromotionID;
                        detail.PromotionName = pProduct.ProductName;
                        detail.PromotionShopPoint = pProduct.Point;
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
                    detail.SubVendor1Amt = detail.VendorPerc1 * detail.SubAmt;
                    detail.SubVendor2Amt = detail.VendorPerc2 * detail.SubAmt;

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


                var tmpOrder = AutoMapperHelper.MapTo<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreRequestDto>(resultdto.Order);
                var tmpDetails = AutoMapperHelper.MapToList<FrxsErpOrderSaleOrderPreGetResp.SaleOrderPreDetailRequestDto, FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailRequestDto>(resultdto.Details);
                var tmpDetailExts = AutoMapperHelper.MapToList<FrxsErpOrderSaleOrderPreGetResp.SaleOrderPreDetailExtsRequestDto, FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailExtsRequestDto>(resultdto.DetailExts);

                var orderserviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = orderserviceCenter.Execute(new FrxsErpOrderSaleOrderPreAddOrEditRequest()
                {
                    WarehouseId = warehouseId,
                    SaleOrderPre = tmpOrder,
                    SaleOrderPreDetailList = tmpDetails,
                    SaleOrderPreDetailExtsList = tmpDetailExts,
                    SendNumber = null,
                    Picks = picks,
                    ShelfAreas = shelfAreas,
                    Flag = 1,
                    UserId = userid,
                    UserName = username
                });

                if (resp.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      ConstDefinition.XSOperatorActionSure, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionSure, OrderID));


                    //跟踪记录
                    var track = new FrxsErpOrderSaleOrderAddTrackRequest()
                    {
                        CreateTime = DateTime.Now,
                        CreateUserID = userid,
                        CreateUserName = username,
                        IsDisplayUser = 1,
                        OrderID = tmpOrder.OrderId,
                        OrderStatus = 2,
                        Remark = "您的订单已经确认，等待拣货"
                    };
                    var bFlag = part.InsertOrderTrack(track, warehouseId, userid, username);
                    if (bFlag)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "Ok",
                            Data = new
                            {
                                UserId = WorkContext.UserIdentity.UserId,
                                UserName = WorkContext.UserIdentity.UserName,
                                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            }
                        }.ToJsonString();
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
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    }.ToJsonString();
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 开始拣货 处理
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031208)]
        public ActionResult OrderPickStart(string OrderID)
        {
            string result = string.Empty;
            try
            {
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                ////1、获取获取空闲待装区
                //var productClient = WorkContext.CreateProductSdkClient();
                //var respStationNumber = productClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductGetFreeStationNumberRequest()
                //{
                //    WID = warehouseId.ToString()
                //});
                //if (respStationNumber.Flag != 0)
                //{
                //    result = new ResultData
                //    {
                //        Flag = ConstDefinition.FLAG_FAIL,
                //        Info = respStationNumber.Info
                //    }.ToJsonString();
                //    return Content(result);
                //}
                //string StationNumber = respStationNumber.Data.StationNumber.ToString();

                var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrdervSaleOrderGetRequest()
                {
                    OrderId = OrderID,
                    WarehouseId = warehouseId
                });
                string ChildWID = string.Empty;
                if (resp != null && resp.Flag == 0)
                {
                    ChildWID = resp.Data.Order.SubWID.ToString();
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    }.ToJsonString();
                    return Content(result);
                }

                //2、开始拣货
                var orderClient = WorkContext.CreateOrderSdkClient();
                var respStartPick = orderClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStartPickUpdateRequest()
                {
                    ParentWID = warehouseId.ToString(),
                    ChildWID = ChildWID,
                    OrderId = OrderID
                    //StationNumber = StationNumber
                });
                if (respStartPick.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = respStartPick.Info
                    }.ToJsonString();
                    return Content(result);
                }
                else
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      ConstDefinition.XSOperatorActionStartPick, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionStartPick, OrderID));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "OK"
                    }.ToJsonString();
                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 拣货完成 处理
        /// </summary>
        /// <param name="ProductData"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031209)]
        public ActionResult PickFinishHandle(string OrderID, string Products)
        {
            string result = string.Empty;
            try
            {
                var pickorderProducts = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<PickOrderProducts>(Products).ToList();
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                foreach (var model in pickorderProducts)
                {
                    model.UnitQty = model.SalePackingQty * model.PickQty;   //库存实发数量
                    //model.PreQty = model.SaleQty;                           //预定数量
                    model.SaleQty = model.PickQty;                          //实际拣货数量
                }

                FrxsErpOrderSubmitPickRequest reqDto = new FrxsErpOrderSubmitPickRequest();
                reqDto.UserId = -1;  //不用传
                reqDto.UserName = username;
                reqDto.WID = warehouseId.ToString();
                reqDto.ProductData = new FrxsErpOrderSubmitPickRequest.SubmitPickOrder();
                reqDto.ProductData.OrderId = OrderID;
                reqDto.ProductData.PickUserID = userid;
                reqDto.ProductData.PickUserName = username;
                reqDto.ProductData.ProductsData = new List<FrxsErpOrderSubmitPickRequest.PickOrderProducts>();
                pickorderProducts.ForEach(x =>
                {
                    FrxsErpOrderSubmitPickRequest.PickOrderProducts model = new FrxsErpOrderSubmitPickRequest.PickOrderProducts();
                    model.ShelfAreaID = x.ShelfAreaID;
                    model.SaleUnit = x.SaleUnit;
                    model.SalePackingQty = x.SalePackingQty.Value;
                    model.PickQty = x.PickQty.Value;
                    model.ProductID = x.ProductID.Value;
                    model.PreQty = x.PreQty.Value;
                    model.IsSet = x.IsSet;
                    reqDto.ProductData.ProductsData.Add(model);
                });

                var orderClient = WorkContext.CreateOrderSdkClient();
                var respSubmitPick = orderClient.Execute(reqDto);
                if (respSubmitPick != null && respSubmitPick.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      ConstDefinition.XSOperatorActionPickFinish, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionPickFinish, OrderID));

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
                        Info = respSubmitPick.Info
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
        /// 对货 处理
        /// </summary>
        /// <param name="ProductData"></param>
        /// <returns></returns>
        public ActionResult PickCheckHandle(string OrderID, string Products)
        {
            string result = string.Empty;
            try
            {
                var pickorderProducts = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<PickCheckProducts>(Products).ToList();
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                foreach (var model in pickorderProducts)
                {
                    if (model.PickQty != model.CheckQty)
                    {
                        model.IsCheckRight = 0;
                    }
                    else
                    {
                        model.IsCheckRight = 1;
                    }
                }
                FrxsErpOrderCheckedGoodsRequest reqDto = new FrxsErpOrderCheckedGoodsRequest();
                reqDto.GoodsInfo = new List<FrxsErpOrderCheckedGoodsRequest.CheckedGoodsNumInfo>();
                pickorderProducts.ForEach(x =>
                {
                    FrxsErpOrderCheckedGoodsRequest.CheckedGoodsNumInfo model = new FrxsErpOrderCheckedGoodsRequest.CheckedGoodsNumInfo();
                    model.ProductId = x.ProductID.Value;
                    model.Number = x.CheckQty.Value;
                    model.IsCheckRight = x.IsCheckRight.Value;
                    reqDto.GoodsInfo.Add(model);
                });
                reqDto.UserId = userid;  //不用传
                reqDto.UserName = username;
                reqDto.WID = warehouseId.ToString();
                reqDto.OrderId = OrderID;
                var orderClient = WorkContext.CreateOrderSdkClient();
                var respPickCheck = orderClient.Execute(reqDto);
                if (respPickCheck != null && respPickCheck.Flag == 0)
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      "对货", string.Format("对货销售订单[{0}]", OrderID));

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
                        Info = respPickCheck.Info
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
        /// 装箱 处理
        /// </summary>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031210)]
        public ActionResult PackStartHandle(string OrderId, int ShopID, int Package1Qty, int Package2Qty, int Package3Qty, string Remark, int PackingEmpID, string PackingEmpName)
        {
            string result = string.Empty;
            try
            {
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                var serviceShop = WorkContext.CreateProductSdkClient();
                var respShop = serviceShop.Execute(new FrxsErpProductShopGetRequest()
                {
                    ShopID = ShopID,
                    UserName = username,
                    UserId = userid
                });
                if (respShop == null)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "没有找到门店信息"
                    }.ToJsonString();
                    return Content(result);
                }

                string SettleID = string.Empty;
                var serviceID = WorkContext.CreateIDSdkClient();
                var respID = serviceID.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
                {
                    WID = warehouseId,
                    Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.SaleSettle,
                    UserName = username,
                    UserId = userid
                });
                if (respID != null && respID.Flag == 0)
                {
                    SettleID = respID.Data;
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "获取结算单ID失败"
                    }.ToJsonString();
                    return Content(result);
                }

                var SettleTypeCenter = WorkContext.CreateProductSdkClient();
                var respSettleType = SettleTypeCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductSysDictDetailGetListRequest()
                {
                    dictCode = "ShopSettleType"
                });

                var serviceSave = WorkContext.CreateOrderSdkClient();
                var saleSettleSave = new ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleAddOrEditRequest();
                saleSettleSave.order = new ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleAddOrEditRequest.SaleSettle();
                saleSettleSave.packingmodel = new ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettleAddOrEditRequest.SaleOrderPrePacking();
                saleSettleSave.WID = warehouseId;

                #region 销售订单装箱数表
                saleSettleSave.PackingEmpID = PackingEmpID;
                saleSettleSave.PackingEmpName = PackingEmpName;
                saleSettleSave.packingmodel.OrderID = OrderId;
                saleSettleSave.packingmodel.Package1Qty = Package1Qty;
                saleSettleSave.packingmodel.Package2Qty = Package2Qty;
                saleSettleSave.packingmodel.Package3Qty = Package3Qty;
                saleSettleSave.packingmodel.WID = warehouseId;
                saleSettleSave.packingmodel.ModifyUserID = userid;
                saleSettleSave.packingmodel.ModifyUserName = username;
                saleSettleSave.packingmodel.ModifyTime = DateTime.Now;
                saleSettleSave.packingmodel.Remark = Remark;
                #endregion

                #region 结算表
                saleSettleSave.order.SettleID = SettleID;
                saleSettleSave.order.WID = warehouseId;
                saleSettleSave.order.Status = 2;
                saleSettleSave.order.OrderID = OrderId;
                saleSettleSave.order.ShopID = ShopID;
                saleSettleSave.order.ShopCode = respShop.Data.ShopCode;
                saleSettleSave.order.ShopType = respShop.Data.ShopType;
                saleSettleSave.order.ShopName = respShop.Data.ShopName;
                saleSettleSave.order.CreditAmt = decimal.Parse(respShop.Data.CreditAmt.ToString());
                saleSettleSave.order.BankAccount = respShop.Data.BankAccount;
                saleSettleSave.order.BankAccountName = respShop.Data.BankAccountName;
                saleSettleSave.order.BankType = respShop.Data.BankType;
                saleSettleSave.order.SettleType = respShop.Data.SettleType;
                var dictLabel = respSettleType.Data.FirstOrDefault(o => o.DictValue.ToString() == respShop.Data.SettleType.ToString());
                if (dictLabel != null)
                {
                    saleSettleSave.order.SettleName = dictLabel.DictLabel;
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "获取结算方式失败"
                    }.ToJsonString();
                    return Content(result);
                }
                //saleSettleSave.order.SettleName = respSettleType.Data.FirstOrDefault(o => o.DictValue.ToString() == respShop.Data.SettleType.ToString()).DictLabel;
                saleSettleSave.order.CreateUserID = userid;
                saleSettleSave.order.CreateUserName = username;
                saleSettleSave.order.CreateTime = DateTime.Now;
                saleSettleSave.order.ModifyUserID = userid;
                saleSettleSave.order.ModifyUserName = username;
                saleSettleSave.order.ModifyTime = DateTime.Now;
                #endregion

                var respSave = serviceSave.Execute(saleSettleSave);
                if (respSave == null || respSave.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = respSave.Info
                    }.ToJsonString();
                    return Content(result);
                }

                #region 更新待装区状态
                var serviceStationNumber = WorkContext.CreateProductSdkClient();

                var respStationNumber = serviceStationNumber.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductUpdateStationNumberIsNullRequest()
                {
                    OrderId = OrderId,
                    Status = "1",
                    OrderStatus = "5",
                    UserId = userid,
                    UserName = username
                });
                #endregion


                //写操作日志
                OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                  ConstDefinition.XSOperatorActionStartPack, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionStartPack, OrderId));

                result = new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "OK"
                }.ToJsonString();
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
        /// 装车 处理
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031211)]
        public ActionResult OrderShipping(string OrderID, int LineID)
        {
            string result = string.Empty;
            try
            {
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                //1.根据LineID获取线路负责人信息
                var productClient = WorkContext.CreateProductSdkClient();
                var respWarehouseLine = productClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineGetRequest()
                {
                    LineID = LineID
                });
                if (respWarehouseLine.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = respWarehouseLine.Info
                    }.ToJsonString();
                    return Content(result);
                }

                int empid = respWarehouseLine.Data.EmpID;
                string empname = respWarehouseLine.Data.EmpName;
                if (string.IsNullOrEmpty(empname))
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = string.Format("当前线路[{0}]负责人信息缺失", respWarehouseLine.Data.LineName)
                    }.ToJsonString();
                    return Content(result);
                }

                //2、开始装车
                var orderClient = WorkContext.CreateOrderSdkClient();
                var respSetDeliverStatus = orderClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSetDeliverStatusRequest()
                {
                    EmpId = empid.ToString(),
                    EmpName = empname,
                    OrderId = OrderID,
                    WID = warehouseId.ToString()
                });

                if (respSetDeliverStatus.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = respSetDeliverStatus.Flag.ToString(),
                        Info = respSetDeliverStatus.Info
                    }.ToJsonString();



                    return Content(result);
                }
                else
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      ConstDefinition.XSOperatorActionShipping, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionShipping, OrderID));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "OK"
                    }.ToJsonString();
                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);

        }

        /// <summary>
        /// 配送完成 处理
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031205)]
        public ActionResult OrderShippingFinish(string OrderID, int LineID)
        {
            string result = string.Empty;
            try
            {
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                //1.根据LineID获取线路负责人信息
                var productClient = WorkContext.CreateProductSdkClient();
                var respWarehouseLine = productClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineGetRequest()
                {
                    LineID = LineID
                });
                if (respWarehouseLine.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = respWarehouseLine.Info
                    }.ToJsonString();
                    return Content(result);
                }

                int empid = respWarehouseLine.Data.EmpID;
                string empname = respWarehouseLine.Data.EmpName;
                if (string.IsNullOrEmpty(empname))
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = string.Format("当前线路[{0}]负责人信息缺失", respWarehouseLine.Data.LineName)
                    }.ToJsonString();
                    return Content(result);
                }

                //2、配送完成
                var orderClient = WorkContext.CreateOrderSdkClient();
                var respSetDeliverStatus = orderClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSetDeliveredStatusRequest()
                {
                    EmpId = empid.ToString(),
                    EmpName = empname,
                    OrderId = OrderID,
                    WID = warehouseId.ToString()
                });

                if (respSetDeliverStatus.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = respSetDeliverStatus.Info
                    }.ToJsonString();
                    return Content(result);
                }
                else
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      ConstDefinition.XSOperatorActionShippingFinish, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionShippingFinish, OrderID));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "OK"
                    }.ToJsonString();
                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 交易完成 处理
        /// </summary>
        /// <param name="orderid"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520312, 52031206)]
        public ActionResult OrderDealFinish(string OrderID)
        {
            string result = string.Empty;
            try
            {
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                var orderClient = WorkContext.CreateOrderSdkClient();
                var respFinished = orderClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderOrderPreFinishedRequest()
                {
                    OrderId = OrderID,
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (respFinished.Flag != 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = respFinished.Info
                    }.ToJsonString();
                    return Content(result);
                }
                else
                {
                    //写操作日志
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                      ConstDefinition.XSOperatorActionDealFinish, string.Format("{0}销售订单[{1}]", ConstDefinition.XSOperatorActionDealFinish, OrderID));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "OK"
                    }.ToJsonString();
                    return Content(result);
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);
        }
        #endregion

        /// <summary>
        /// 获取装箱员信息
        /// </summary>
        /// <returns></returns>
        public ActionResult GetZXEmpList()
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
                    UserType = "3",   //装箱员
                    IsFrozen = "0"   //非冻结
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
        ///  获EXCEL里的数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetImportData(string fileName, string folderPath, int shopid, int subWId)
        {
            string jsonStr = "[]";
            try
            {
                var filePath = Server.MapPath(folderPath) + "\\" + fileName;
                DataTable dt = NpoiExcelhelper.RenderFromExcel(filePath);
                var list = new List<WarehouseOrderDetails>();
                for (int i = 0; i < dt.Rows.Count; i++)
                {
                    if (string.IsNullOrEmpty(dt.Rows[i][0].ToString()))
                    {
                        //continue;
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行商品编码为空"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }
                    var model = new WarehouseOrderDetails();
                    //销售数量
                    decimal SaleQty = 0;
                    if (!decimal.TryParse(dt.Rows[i][2].ToString(), out SaleQty))
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][0].ToString()) + "销售数量格式不正确"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }
                    if (SaleQty > 99999999)
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][0].ToString()) + "销售数量过大，最大数为99999999"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }
                    //单位
                    string unit = dt.Rows[i][1].ToString();
                    if (string.IsNullOrEmpty(unit))
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", dt.Rows[i][0].ToString()) + "单位为空"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }
                    model.SKU = dt.Rows[i][0].ToString();
                    model.SaleUnit = dt.Rows[i][1].ToString();
                    model.SaleQty = SaleQty;      //销售数量
                    model.PreQty = SaleQty;       //预定数量
                    model.Remark = Convert.ToString(dt.Rows[i][3]);      //备注
                    model.IsAppend = 1;   //默认为强配商品

                    model.Index = list.Count + 1;
                    list.Add(model);
                }
                var SKUs = from o in list select o.SKU;
                var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetToB2BRequest()
                {
                    IsPage = 1,
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKUs = SKUs.ToList(),
                    ShopID = shopid
                });
                if (resp != null && resp.Flag == 0 && resp.Data != null)
                {
                    if (resp.Data.TotalRecords == 0)
                    {
                        jsonStr = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "Excel表中所有商品数据不存在"
                        }.ToJsonString();
                        return Content(jsonStr);
                    }

                    for (int i = 0; i < list.Count; i++)
                    {
                        var model = list[i];
                        var tmpmodel = resp.Data.ItemList.FirstOrDefault(o => o.SKU == list[i].SKU);
                        if (tmpmodel == null)
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", model.SKU) + "数据不存在"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        //单位
                        if (model.SaleUnit == tmpmodel.Unit)   //库存单位
                        {
                            model.SaleUnit = tmpmodel.Unit;
                            model.SalePackingQty = 1;
                            model.SalePrice = tmpmodel.SalePrice.Value;
                        }
                        else if (model.SaleUnit == tmpmodel.BigUnit)  //配送单位
                        {
                            model.SaleUnit = tmpmodel.BigUnit;
                            model.SalePackingQty = tmpmodel.BigPackingQty.Value;
                            model.SalePrice = tmpmodel.BigSalePrice;
                        }
                        else
                        {
                            jsonStr = new ResultData
                            {
                                Flag = ConstDefinition.FLAG_FAIL,
                                Info = "Excel第 " + (i + 1) + " 行" + string.Format("商品【{0}】", tmpmodel.SKU) + "单位数据库中不存在"
                            }.ToJsonString();
                            return Content(jsonStr);
                        }
                        model.ProductName = tmpmodel.ProductName;
                        model.BarCode = tmpmodel.BarCode;
                        model.ProductId = tmpmodel.ProductId;
                        model.UnitPrice = tmpmodel.SalePrice.Value;
                        model.Unit = tmpmodel.Unit;
                        model.UnitQty = model.SaleQty * model.SalePackingQty;
                        model.MaxOrderQty = tmpmodel.MaxOrderQty;
                        model.ShopAddPerc = tmpmodel.ShopAddPerc.Value;
                        model.BasePoint = tmpmodel.BasePoint.Value;
                        model.VendorPerc1 = tmpmodel.VendorPerc1.Value;
                        model.VendorPerc2 = tmpmodel.VendorPerc2.Value;

                        model.BigSalePrice = tmpmodel.BigSalePrice;
                    }
                }
                else
                {
                    jsonStr = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    }.ToJsonString();
                    return Content(jsonStr);
                }
                //获取库存列表
                var pdidList = from o in list select o.ProductId;
                var OrderStockQty = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyQueryRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SubWID = subWId,
                    PDIDList = pdidList.ToList()
                });
                var promotionserviceCenter = WorkContext.CreatePromotionSdkClient();
                //门店促销积分接口
                var promotionDetailspoint = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    BeginTime = DateTime.Now,
                    EndTime = DateTime.Now,
                    ShopID = shopid,
                    ProductIDList = string.Join(",", pdidList.ToArray()),
                    PromotionType = 1
                });
                //平台费率促销接口
                var promotionDetailsperc = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ShopID = shopid,
                    ProductIDList = string.Join(",", pdidList.ToArray()),
                    PromotionType = 2
                });
                foreach (var item in list)
                {
                    if (OrderStockQty != null && OrderStockQty.Data != null)
                    {
                        var frxsErpOrderStockQtyListQueryRespData = OrderStockQty.Data.StockQtyList.FirstOrDefault(i => i.PID == item.ProductId);
                        if (frxsErpOrderStockQtyListQueryRespData != null)
                        {
                            item.Stock = frxsErpOrderStockQtyListQueryRespData.StockQty;
                            item.OnTheWay = frxsErpOrderStockQtyListQueryRespData.PreQty;
                            item.Available = frxsErpOrderStockQtyListQueryRespData.EnQty;
                        }
                    }
                    if (promotionDetailspoint.Data != null && promotionDetailspoint.Data.Count > 0)
                    {
                        var temp = promotionDetailspoint.Data.FirstOrDefault(o => o.ProductID == item.ProductId);
                        if (temp != null)
                        {
                            item.MaxOrderQty = temp.MaxOrderQty;
                        }
                    }
                    if (promotionDetailsperc.Data != null && promotionDetailsperc.Data.Count > 0)
                    {
                        var temp = promotionDetailsperc.Data.FirstOrDefault(o => o.ProductID == item.ProductId);

                        if (temp != null)
                        {
                            item.ShopAddPerc = temp.Point;
                        }
                    }
                    if (item.MaxOrderQty != 0 && item.MaxOrderQty < item.SaleQty)
                    {
                        item.SaleQty = item.MaxOrderQty;
                        item.PreQty = item.MaxOrderQty;
                        item.UnitQty = item.SaleQty * item.SalePackingQty;
                    }
                    item.SubAmt = item.SaleQty * item.SalePrice * (1 + item.ShopAddPerc);
                }
                var obj = new { total = list.Count, rows = list };
                jsonStr = obj.ToJsonString();
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { Info = "文件读取发生异常，请按模版重新导入", Flag = "FAIL" }.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 更新线路
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="LineName"></param>
        /// <param name="LineID"></param>
        /// <returns></returns>
        public ActionResult LineChangeHandle(string OrderID, string LineName, int LineID)
        {
            string result = string.Empty;
            try
            {
                var orderClient = WorkContext.CreateOrderSdkClient();
                var respUpdateLine = orderClient.Execute(new FrxsErpOrderSaleOrderPreUpdateLineRequest()
                {
                    OrderId = OrderID,
                    LineId = LineID,
                    LineName = LineName,
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId

                });
                if (respUpdateLine != null && respUpdateLine.Flag == 0)
                {
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
                        Info = respUpdateLine.Info
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
        /// 获取门店订单数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetWarehouseOrdeInfo(string OrderID)
        {
            string result = string.Empty;
            //获取数据，绑定
            var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrdervSaleOrderGetRequest()
            {
                OrderId = OrderID,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            if (resp != null && resp.Flag == 0)
            {
                var PIDList = from o in resp.Data.Details select o.ProductId;
                var OrderServiceCenter = WorkContext.CreateOrderSdkClient();
                var OrderStockQty = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyQueryRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SubWID = resp.Data.Order.SubWID.Value,
                    PDIDList = PIDList.ToList()
                });

                //var promotionserviceCenter = WorkContext.CreatePromotionSdkClient();
                ////门店促销积分接口
                //var promotionDetailspoint = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                //{
                //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                //    BeginTime = DateTime.Now,
                //    EndTime = DateTime.Now,
                //    ShopID = resp.Data.Order.ShopID,
                //    ProductIDList = string.Join(",", PIDList.ToArray()),
                //    PromotionType = 1
                //});
                ////平台费率促销接口
                //var promotionDetailsperc = promotionserviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
                //{
                //    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                //    ShopID = resp.Data.Order.ShopID,
                //    ProductIDList = string.Join(",", PIDList.ToArray()),
                //    PromotionType = 2
                //});

                var respTemp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetToB2BRequest()
                {
                    IsPage = 1,
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    WID = resp.Data.Order.WID,
                    ProductIds = PIDList.ToList(),
                    ShopID = resp.Data.Order.ShopID,
                });

                List<SaleOrderDetailExt> saleProductExts = new List<SaleOrderDetailExt>();
                foreach (var item in resp.Data.Details)
                {
                    var tempproduct = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<SaleOrderDetailExt>(item);
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
                    tempproduct.SubAmt = item.SubAmt + item.SubAddAmt;

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

                    if (respTemp != null && respTemp.Data != null)
                    {
                        var tempDetails = respTemp.Data.ItemList.FirstOrDefault(i => i.ProductId == item.ProductId);
                        if (tempDetails != null)
                        {
                            tempproduct.MaxSalePrice = tempDetails.BigSalePrice;
                        }
                    }
                    saleProductExts.Add(tempproduct);
                }

                result = new { Order = resp.Data.Order, Details = saleProductExts, DetailExts = resp.Data.DetailExts }.ToJsonString();
            }
            return Content(result);
        }

        public class SaleOrderDetailExt : Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderSaleOrderPreGetResp.SaleOrderPreDetailRequestDto
        {
            //库存数量
            public decimal Stock { get; set; }

            //在途数量
            public decimal OnTheWay { get; set; }

            //可用数量
            public decimal Available { get; set; }

            //限购数量
            public decimal MaxOrderQty { get; set; }

            //配送积分
            public decimal BigShopPoint { get; set; }

            public decimal MaxSalePrice { get; set; }

        }

        /// <summary>
        /// 获取分页列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        public ActionResult GetWarehouseOrderList(WarehouseOrderSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new WarehouseOrderModel().GetWarehouseOrderPageData(searchModel);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 获取仓库下线路列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        public ActionResult GetWarehouseLineList(string LineName)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineTableListRequest()
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    LineName = !string.IsNullOrEmpty(LineName) ? LineName : null,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });
                if (resp != null && resp.Flag == 0)
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
        /// 新增订单 检测
        /// </summary>
        /// <param name="OrderID"></param>
        /// <param name="Status"></param>
        /// <returns></returns>
        public ActionResult OrderAddCheck(int ShopID)
        {
            string result = string.Empty;
            try
            {
                //接口
                var ServiceCenter = WorkContext.CreateOrderSdkClient();
                var resp = ServiceCenter.Execute(new FrxsErpOrderSaleOrderPreQueryRequest()
                {
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ShopId = ShopID,
                    PageIndex = 1,
                    PageSize = 10,
                    SortBy = "Status asc",
                });
                if (resp != null && resp.Flag == 0)
                {
                    if (resp.Data.Orders[0].Status == 1)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "1"     //有未确认的订单，不能下单！
                        }.ToJsonString();
                        return Content(result);
                    }
                    else if (resp.Data.Orders[0].Status >= 2 && resp.Data.Orders[0].Status <= 6)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "2"     //有已确认的订单，是否创建新订单？
                        }.ToJsonString();
                        return Content(result);
                    }
                    else
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "OK"
                        }.ToJsonString();
                        return Content(result);
                    }

                    //result = new ResultData
                    //{
                    //    Flag = ConstDefinition.FLAG_SUCCESS,
                    //    Info = "OK"
                    //}.ToJsonString();
                    //return Content(result);
                }
                else
                {
                    if (resp != null && resp.Info == "没有找到数据")
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "OK"
                        }.ToJsonString();
                        return Content(result);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                result = new { info = ex.Message }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 导出数据到Excel
        /// </summary>
        /// <returns>文件流</returns>
        [AuthorizeButtonFiter(520312, new int[] { 52031201, 52031202 })]
        public ActionResult DataExport(string OrderID)
        {
            IList<SaleOrderPreDetailsModel> detailsmodel = new List<SaleOrderPreDetailsModel>();
            var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrdervSaleOrderGetRequest()
            {
                OrderId = OrderID,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });


            if (resp != null && resp.Flag == 0)
            {
                detailsmodel = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrdervSaleOrderGetResp.SaleOrderPreDetailRequestDto, SaleOrderPreDetailsModel>(resp.Data.Details);
            }

            foreach (var item in resp.Data.Details)
            {
                var temp = detailsmodel.FirstOrDefault(o => o.ProductId == item.ProductId);
                var ext = resp.Data.DetailExts.FirstOrDefault(o => o.ID == item.ID);
                if (temp != null)
                {
                    temp.IsPromotion = string.IsNullOrEmpty(item.PromotionID) ? "否" : (item.PromotionID == "0" ? "否" : "是");
                    temp.ShopCategoryName = ext.ShopCategoryId1Name + ">>" + ext.ShopCategoryId2Name + ">>" + ext.ShopCategoryId3Name;
                }
                temp.SubAmt = item.SubAmt + item.SubAddAmt;
            }

            int maxRows = 1000;
            string fileName = "销售订单_" + OrderID + ".xls";  //DateTime.Now.ToString("yyyyMMddHHmmssfff")
            byte[] byteArr = NpoiExcelhelper.ExportExcel
                (
                    detailsmodel,
                    maxRows,
                    Server.MapPath("UploadFile"),
                    fileName
                );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }

        /// <summary>
        /// 仓库订单记录保存
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [AuthorizeButtonFiter(520312, new int[] { 52031201, 52031202 })]
        public ActionResult WarehouseOrderAddOrEditeHandle(string jsonData, string jsonDetails, bool isPD = false)
        {
            int flag = 0;
            string result = string.Empty;
            try
            {
                var order = Frxs.Platform.Utility.Json.JsonHelper.FromJson<WarehouseOrder>(jsonData);
                var orderdetails = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<WarehouseOrderDetails>(jsonDetails);
                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                #region 合并商品数据
                var newdetails = new List<WarehouseOrderDetails>();
                foreach (var item in orderdetails)
                {
                    var temp = newdetails.FirstOrDefault(o => o.ProductId == item.ProductId);
                    if (temp == null)
                    {
                        newdetails.Add(item);
                    }
                    else
                    {
                        temp.SaleQty += item.SaleQty;
                    }
                }
                orderdetails = newdetails;
                #endregion

                #region 调用接口，获取数据
                var part = new OrderPartsServer();
                //1.调接口，获取门店详细信息
                var shop = part.GetShopInfo(order.ShopID, userid, username);
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

                var PIDList = from o in orderdetails select o.ProductId;
                //2.调用接口，获取网仓商品信息
                var wProudcts = part.GetWProductsInfo(warehouseId, PIDList.ToList(), order.ShopID, userid, username);
                //3.调用接口，获取商品信息
                var products = part.GetProductsInfo(PIDList.ToList(), userid, username);
                //4.调用接口，获取品牌信息
                var brandIdList = new List<int>();
                foreach (var product in products.ItemList)
                {
                    if (product.BrandId1 > 0)
                    {
                        brandIdList.Add(product.BrandId1);
                    }
                    if (product.BrandId2 > 0)
                    {
                        brandIdList.Add(product.BrandId2);
                    }
                }
                var brandList = part.GetBrands(brandIdList, userid, username);
                //5.调用接口 取商品促销积分
                var promotionProducts = part.GetPromotionRebate(warehouseId, order.ShopID, PIDList.ToList(), userid, username);
                //6.调用接口 取库存信息
                var productStock = part.GetProductStock(warehouseId, order.SubWID, PIDList.ToList(), userid, username);
                //7.调用接口 取促销平台费率
                var rProducts = part.GetPromotionRate(warehouseId, order.ShopID, PIDList.ToList(), userid, username);
                #endregion

                //添加排序表
                var sendNumber = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderSendNumberRequestDto();
                var picks = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailsPickRequestDto>();
                var shelfAreas = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreShelfAreaRequestDto>();

                //开始组装
                FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreRequestDto orderdto = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreRequestDto();
                if (string.IsNullOrEmpty(order.OrderID))
                {

                    string orderid = new WarehouseOrderModel().GetOrderID();
                    if (string.IsNullOrEmpty(orderid))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "获取销售订单号失败"
                        }.ToJsonString();
                        return Content(result);
                    }

                    orderdto.OrderId = orderid;
                    #region SaleOrderSendNumber表
                    sendNumber.OrderID = orderdto.OrderId;
                    sendNumber.ModifyTime = DateTime.Now;
                    sendNumber.ModifyUserID = userid;
                    sendNumber.ModifyUserName = username;
                    sendNumber.WID = warehouseId;
                    sendNumber.WarehouseId = warehouseId;
                    sendNumber.SendNumber = 999;
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
                    #endregion
                }
                else
                {
                    orderdto.OrderId = order.OrderID;
                    flag = 1;
                }
                #region 主表
                orderdto.WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;
                orderdto.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
                WarehouseIdentity subwareHouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(o => o.WarehouseId == order.SubWID);
                orderdto.SubWCode = subwareHouse.WarehouseCode;
                orderdto.SubWName = subwareHouse.WarehouseName;
                orderdto.Status = order.Status;
                orderdto.WID = warehouseId;
                orderdto.SubWID = order.SubWID;
                orderdto.OrderDate = order.OrderDate;
                orderdto.OrderType = 1;   //客服代客
                orderdto.ShopID = order.ShopID;
                orderdto.ShopType = shop.ShopType;
                orderdto.ShopCode = order.ShopCode;
                orderdto.ShopName = order.ShopName;
                orderdto.ProvinceID = shop.ProvinceID;
                orderdto.CityID = shop.CityID;
                orderdto.RegionID = shop.RegionID;
                var regionListIds = new List<int>();
                regionListIds.Add(shop.ProvinceID);
                regionListIds.Add(shop.CityID);
                regionListIds.Add(shop.RegionID);
                var region = part.GetRegions(regionListIds);
                if (region.Count > 0)
                {
                    orderdto.ProvinceName = region.Where(x => x.AreaID == shop.ProvinceID).First().AreaName;
                    orderdto.CityName = region.Where(x => x.AreaID == shop.CityID).First().AreaName;
                    orderdto.RegionName = region.Where(x => x.AreaID == shop.RegionID).First().AreaName;
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "门店区域信息为空"
                    }.ToJsonString();
                    return Content(result);
                }

                orderdto.Address = shop.Address;
                orderdto.FullAddress = shop.FullAddress;
                orderdto.RevLinkMan = shop.LinkMan;
                orderdto.RevTelephone = shop.Telephone;
                string msg = string.Empty;
                if (shop.LineID == 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "门店线路信息为空"
                    }.ToJsonString();
                    return Content(result);
                }
                var sendDate = part.GetSendDate(shop.LineID, userid, username, orderdto.OrderDate, ref msg, isPD);
                if (!sendDate.HasValue || sendDate == null)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = msg
                    }.ToJsonString();
                    return Content(result);
                }
                orderdto.SendDate = sendDate.Value;
                orderdto.LineID = shop.LineID;
                orderdto.LineName = shop.LineName;
                orderdto.IsPrinted = 0;   //未打印
                orderdto.Remark = order.Remark;
                orderdto.CouponAmt = 0;       //订单优惠金额(预留,固定为0)
                orderdto.UserShowFlag = 1;    //用户删除订单标识(0:不显示;1:显示)
                orderdto.ClientType = 2;      //PC
                orderdto.CreateTime = DateTime.Now;
                orderdto.CreateUserID = userid;
                orderdto.CreateUserName = username;
                orderdto.ModifyTime = DateTime.Now;
                orderdto.ModifyUserID = userid;
                orderdto.ModifyUserName = username;
                #endregion

                IList<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailRequestDto> orderdetailsdto
                    = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailRequestDto>();
                IList<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailExtsRequestDto> orderdetailsextdto
                    = new List<FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailExtsRequestDto>();

                decimal TotalProductAmt = 0.00m;    //总金额
                decimal TotalBasePoint = 0.00m;     //总积分
                decimal TotalAddAmt = 0.00m;        //门店合计提点金额
                decimal TotalPoint = 0.00m;         //门店合计总积分

                //移除已限购商品
                if (wProudcts.ItemList.Count != orderdetails.GroupBy(x => x.ProductId).Count())
                {
                    for (int i = orderdetails.Count - 1; i >= 0; i--)
                    {
                        var detail = orderdetails[i];
                        var tmp = wProudcts.ItemList.FirstOrDefault(x => x.ProductId == detail.ProductId);
                        if (tmp == null)
                        {
                            orderdetails.Remove(detail);
                        }
                    }
                }
                if (orderdetails.Count <= 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "订单所有商品均已被限制购买"
                    }.ToJsonString();
                    return Content(result);
                }

                //限购检查
                #region 限购检查
                var list = new List<WarehouseOrderDetails>();
                foreach (var detail in orderdetails)
                {
                    var wProduct = wProudcts.ItemList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                    decimal SaleQty = 0.0m;
                    if (detail.SalePackingQty == 1)   //库存单位
                    {
                        SaleQty = detail.SaleQty / wProduct.BigPackingQty.Value;
                    }
                    else
                    {
                        SaleQty = detail.SaleQty;
                    }
                    list.Add(new WarehouseOrderDetails()
                    {
                        ProductId = detail.ProductId,
                        SaleQty = SaleQty,
                        ProductName = detail.ProductName
                    });
                }
                msg = "";
                List<Frxs.Erp.WarehouseManagementSystem.WebUI.OrderPartsServer.SimpleProdcutQuoteModel> maxList = new List<Frxs.Erp.WarehouseManagementSystem.WebUI.OrderPartsServer.SimpleProdcutQuoteModel>();
                var flag1 = part.QuotaCheck(order.ShopID, warehouseId, PIDList.ToList(), list, userid, username, ref msg, maxList);
                if (!flag1)//有限购商品超过限制
                {
                    foreach (var detail in orderdetails)
                    {
                        var wProduct = wProudcts.ItemList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                        decimal SaleQty = 0.0m;
                        if (detail.SalePackingQty == 1)   //库存单位
                        {
                            SaleQty = detail.SaleQty / wProduct.BigPackingQty.Value;
                        }
                        else
                        {
                            SaleQty = detail.SaleQty;
                        }

                        var m = maxList.Where(x => x.ProductId == detail.ProductId).FirstOrDefault();
                        if (m != null && SaleQty > m.MaxPreQty && m.MaxPreQty != 0)
                        {
                            if (detail.SalePackingQty == 1)  //库存单位
                            {
                                detail.SaleQty = m.MaxPreQty * wProduct.BigPackingQty.Value;
                            }
                            else
                            {
                                detail.SaleQty = m.MaxPreQty;
                            }
                        }
                    }
                }
                #endregion

                foreach (var model in orderdetails)
                {
                    var wProduct = wProudcts.ItemList.Where(x => x.ProductId == model.ProductId).FirstOrDefault();
                    var product = products.ItemList.Where(x => x.ProductId == model.ProductId).FirstOrDefault();
                    var pProduct = promotionProducts.Where(x => x.ProductID == model.ProductId).FirstOrDefault();
                    var stock = productStock.StockQtyList.Where(x => x.PID == model.ProductId).FirstOrDefault();
                    var rProduct = rProducts.Where(x => x.ProductID == model.ProductId).FirstOrDefault();

                    if (product == null)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("没有找到商品{0}", model.SKU)
                        }.ToJsonString();
                        return Content(result);
                    }
                    if (wProduct == null)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = string.Format("仓库中没有找到商品{0}", model.SKU)
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

                    #region 商品信息
                    FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailRequestDto temp = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailRequestDto();
                    temp.WID = warehouseId;
                    temp.OrderID = orderdto.OrderId;
                    temp.ProductId = model.ProductId;
                    temp.SKU = model.SKU;
                    temp.ProductName = model.ProductName;
                    temp.BarCode = model.BarCode;
                    temp.ProductImageUrl200 = wProduct.ImageUrl200x200;          //从网仓商品中取得
                    temp.ProductImageUrl400 = wProduct.ImageUrl400x400;          //从网仓商品中取得
                    temp.WCProductID = (int)wProduct.WProductID;
                    temp.PreQty = model.SaleQty;      //PreQty   配送(销售)预定数量
                    temp.SaleUnit = model.SaleUnit;
                    temp.SalePackingQty = model.SalePackingQty;
                    if (model.SalePackingQty != 1)  //销售单位
                    {
                        temp.SalePrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0) * (wProduct.BigPackingQty.HasValue ? wProduct.BigPackingQty.Value : 1);
                    }
                    else
                    {
                        temp.SalePrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0);
                    }
                    //temp.SalePrice = model.SalePrice;
                    temp.SaleQty = model.SaleQty;
                    temp.Unit = model.Unit;
                    temp.UnitQty = (model.SalePackingQty * model.SaleQty);
                    temp.UnitPrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0);
                    //temp.UnitPrice = model.UnitPrice;
                    temp.SubAmt = (model.SalePrice * model.SaleQty);
                    if (rProduct == null)
                    {
                        temp.ShopAddPerc = wProduct.ShopAddPerc.HasValue ? wProduct.ShopAddPerc.Value : 0;
                    }
                    else
                    {
                        temp.PromotionID = rProduct.PromotionID;
                        temp.PromotionName = rProduct.ProductName;
                        temp.ShopAddPerc = rProduct.Point;
                    }
                    temp.ShopPoint = wProduct.ShopPoint.HasValue ? wProduct.ShopPoint.Value : 0;
                    temp.BasePoint = wProduct.BasePoint.HasValue ? wProduct.BasePoint.Value : 0;
                    //temp.BasePoint = model.BasePoint;
                    temp.VendorPerc1 = wProduct.VendorPerc1.HasValue ? wProduct.VendorPerc1.Value : 0;
                    temp.VendorPerc2 = wProduct.VendorPerc2.HasValue ? wProduct.VendorPerc2.Value : 0;
                    //temp.VendorPerc1 = model.VendorPerc1;
                    //temp.VendorPerc2 = model.VendorPerc2;
                    temp.SubAddAmt = temp.ShopAddPerc * temp.SubAmt;
                    temp.SubBasePoint = temp.SubAmt * temp.BasePoint;
                    temp.SubVendor1Amt = temp.VendorPerc1 * temp.SubAmt;
                    temp.SubVendor2Amt = temp.VendorPerc2 * temp.SubAmt;
                    if (pProduct != null)
                    {
                        temp.PromotionID = pProduct.PromotionID;
                        temp.PromotionName = pProduct.ProductName;
                        temp.PromotionShopPoint = pProduct.Point;
                        //temp.SubPoint = temp.PromotionShopPoint * temp.UnitQty;
                        if (temp.PromotionShopPoint > 0)
                        {
                            temp.SubPoint = temp.PromotionShopPoint * temp.UnitQty;
                        }
                        else
                        {
                            temp.SubPoint = temp.ShopPoint * temp.UnitQty;
                        }
                    }
                    else
                    {
                        temp.PromotionShopPoint = 0;
                        temp.SubPoint = temp.ShopPoint * temp.UnitQty;
                    }
                    //PromotionUnitPrice   库存单位促销价格(WProductPromotionDetails.PromotionPrice)  先不管
                    temp.IsAppend = model.IsAppend;    //是否为后来录单员添加商品(0:不是;1:是的)  @@@@Todo 需要判断是否为0 不是0则赋值为1
                    //EditID     追加的ID(SaleEdit.Edit)  可以为空
                    temp.IsStockOut = (stock == null ? 1 : ((temp.UnitQty) > stock.StockQty ? 1 : 0));
                    temp.Remark = model.Remark;
                    temp.ModifyTime = DateTime.Now;
                    temp.ModifyUserID = userid;
                    temp.ModifyUserName = username;

                    //仓库ID+ProductId+GUID- ProductId用于排序防止每次修改顺序都会被打乱
                    temp.ID = warehouseId.ToString() + model.ProductId + Guid.NewGuid();
                    temp.SerialNumber = orderdetailsdto.Count + 1;

                    orderdetailsdto.Add(temp);
                    #endregion

                    #region 商品扩展
                    FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailExtsRequestDto detailExt = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailExtsRequestDto();
                    detailExt.OrderID = orderdto.OrderId;
                    var shopCategoryNames = product.ShopCategoryName.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                    detailExt.ShopCategoryId1 = product.ShopCategoryId1;
                    detailExt.ShopCategoryId1Name = shopCategoryNames[0];
                    detailExt.ShopCategoryId2 = product.ShopCategoryId2;
                    detailExt.ShopCategoryId2Name = shopCategoryNames[1];
                    detailExt.ShopCategoryId3 = product.CategoryId3;
                    detailExt.ShopCategoryId3Name = shopCategoryNames[2];
                    detailExt.BrandId1 = product.BrandId1;
                    var brand1 = brandList.Where(x => x.BrandId == detailExt.BrandId1).FirstOrDefault();
                    if (brand1 != null)
                    {
                        detailExt.BrandId1Name = brand1.BrandName;
                    }
                    detailExt.BrandId2 = product.BrandId2;
                    var brand2 = brandList.Where(x => x.BrandId == detailExt.BrandId2).FirstOrDefault();
                    if (brand2 != null)
                    {
                        detailExt.BrandId2Name = brand2.BrandName;
                    }
                    detailExt.CategoryId1 = product.CategoryId1;
                    detailExt.CategoryId2 = product.CategoryId2;
                    detailExt.CategoryId3 = product.CategoryId3;
                    var categoryNames = product.CategoryName.Split(new string[] { ">>" }, StringSplitOptions.RemoveEmptyEntries);
                    detailExt.CategoryId1Name = categoryNames[0];
                    detailExt.CategoryId2Name = categoryNames[1];
                    detailExt.CategoryId3Name = categoryNames[2];
                    detailExt.ModifyTime = DateTime.Now;
                    detailExt.ModifyUserID = userid;
                    detailExt.ModifyUserName = username;

                    detailExt.OrderID = temp.OrderID; // 和SaleOrderPreDetail保持相同
                    detailExt.ID = temp.ID;// 和SaleOrderPreDetail保持相同
                    
                    orderdetailsextdto.Add(detailExt);
                    #endregion

                    TotalBasePoint = TotalBasePoint + temp.SubBasePoint.Value;
                    TotalProductAmt = TotalProductAmt + temp.SubAmt.Value;
                    TotalAddAmt = TotalAddAmt + temp.SubAddAmt.Value;
                    TotalPoint = TotalPoint + temp.SubPoint.Value;

                    if (flag == 1 && order.Status == 2)   //等待拣货状态编辑 需要更新商品货架表记录、商品分类表记录
                    {
                        #region 更新商品货架表记录、商品分类表记录
                        var pick = new FrxsErpOrderSaleOrderPreAddOrEditRequest.SaleOrderPreDetailsPickRequestDto()
                        {
                            BarCode = temp.BarCode,
                            CheckTime = null,
                            CheckUserID = null,
                            CheckUserName = null,
                            ID = warehouseId.ToString() + Guid.NewGuid(),
                            IsAppend = 0,
                            IsCheckRight = null,
                            ModifyTime = DateTime.Now,
                            ModifyUserID = userid,
                            ModifyUserName = username,
                            OrderID = temp.OrderID,
                            ProductID = temp.ProductId,
                            PickQty = null,
                            PickTime = null,
                            PickUserID = null,
                            PickUserName = null,
                            ProductImageUrl200 = temp.ProductImageUrl200,
                            ProductImageUrl400 = temp.ProductImageUrl400,
                            ProductName = temp.ProductName,
                            Remark = temp.Remark,
                            SKU = temp.SKU,
                            SalePackingQty = temp.SalePackingQty,
                            SaleQty = temp.SaleQty.Value,
                            SaleUnit = temp.SaleUnit,
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
                                OrderID = temp.OrderID,
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
                        #endregion
                    }
                }

                orderdto.TotalPoint = TotalPoint;              //门店合计总积分 sum(saleOrderDetails.SubPoint)
                orderdto.TotalProductAmt = TotalProductAmt;    //sum(SaleOrderDetails.SubAmt)
                orderdto.TotalBasePoint = TotalBasePoint;      //合计绩效积分 sum(saleOrderDetails.SubBasePoint)
                orderdto.TotalAddAmt = TotalAddAmt;            //门店合计提点金额 sum(SaleOrderDettail.SubAddAmt)
                orderdto.PayAmount = TotalProductAmt - orderdto.CouponAmt + TotalAddAmt;   //最后计算金额/应付金额 TotalProductAmt-CouponAmt+TotalAddAmt

                var orderserviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = orderserviceCenter.Execute(new FrxsErpOrderSaleOrderPreAddOrEditRequest()
                {
                    WarehouseId = warehouseId,
                    SaleOrderPre = orderdto,
                    SaleOrderPreDetailList = orderdetailsdto,
                    SaleOrderPreDetailExtsList = orderdetailsextdto,
                    SendNumber = sendNumber,
                    Picks = picks,
                    ShelfAreas = shelfAreas,
                    Flag = flag,
                    UserId = userid,
                    UserName = username
                });

                if (resp.Flag == 0)
                {
                    //写操作日志
                    string action = string.Empty;
                    if (flag == 1)
                    {
                        action = ConstDefinition.XSOperatorActionEdit;
                    }
                    else
                    {
                        action = ConstDefinition.XSOperatorActionAdd;
                    }
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3B,
                          action, string.Format("{0}销售订单[{1}]", action, orderdto.OrderId));

                    //写订单跟踪记录
                    string Remark = "";
                    int IsDisplayUser = 0;
                    if (flag == 0)
                    {
                        Remark = "客服人员帮您提交了订单，请等待系统确认";
                        IsDisplayUser = 1;
                    }
                    else
                    {
                        Remark = "客服人员进行了改单";
                        IsDisplayUser = 0;
                    }
                    var track = new FrxsErpOrderSaleOrderAddTrackRequest()
                    {
                        CreateTime = DateTime.Now,
                        CreateUserID = userid,
                        CreateUserName = username,
                        IsDisplayUser = IsDisplayUser,
                        OrderID = orderdto.OrderId,
                        OrderStatus = 1,
                        Remark = Remark
                    };
                    var bFlag = part.InsertOrderTrack(track, warehouseId, userid, username);
                    if (bFlag)
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "操作成功",
                            Data = new
                            {
                                OrderID = orderdto.OrderId,
                                UserId = WorkContext.UserIdentity.UserId,
                                UserName = WorkContext.UserIdentity.UserName,
                                Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm")
                            }
                        }.ToJsonString();
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
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
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
    }

    /// <summary>
    /// 仓库订单 主表模型
    /// </summary>
    public class WarehouseOrder
    {
        public string OrderID { get; set; }
        public DateTime OrderDate { get; set; }
        public int SubWID { get; set; }
        public int ShopID { get; set; }
        public int Status { get; set; }
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
        public string Remark { get; set; }
    }

    /// <summary>
    /// 仓库订单 详情模型
    /// </summary>
    public class WarehouseOrderDetails
    {
        public int Index { get; set; }
        public decimal SubAmt { get; set; }
        public decimal UnitQty { get; set; }
        //库存数量
        public decimal Stock { get; set; }
        //在途数量
        public decimal OnTheWay { get; set; }
        //可用数量
        public decimal Available { get; set; }
        //限购数量
        public decimal MaxOrderQty { get; set; }
        //预定数量
        public decimal PreQty { get; set; }


        public int ProductId { get; set; }
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string SaleUnit { get; set; }
        public decimal SaleQty { get; set; }
        public decimal SalePackingQty { get; set; }
        public decimal SalePrice { get; set; }

        public int IsAppend { get; set; }
        public decimal UnitPrice { get; set; }
        public string Unit { get; set; }
        public decimal ShopAddPerc { get; set; }
        public decimal BasePoint { get; set; }
        public decimal VendorPerc1 { get; set; }
        public decimal VendorPerc2 { get; set; }

        public decimal BigSalePrice { get; set; }

        public string Remark { get; set; }
        public string BarCode { get; set; }
    }

    /// <summary>
    /// 拣货提交传入参数
    /// </summary>
    public class SubmitPickOrder
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 拣货人编号
        /// </summary>
        public int PickUserID { get; set; }

        /// <summary>
        /// 拣货人名称
        /// </summary>
        public string PickUserName { get; set; }

        /// <summary>
        /// 商品信息
        /// </summary>
        public List<PickOrderProducts> ProductsData { get; set; }
    }

    /// <summary>
    /// 拣货商品详信息
    /// </summary>
    public class PickOrderProducts
    {
        /// <summary>
        /// 货区编号
        /// </summary>
        public int? ShelfAreaID { get; set; }

        /// <summary>
        /// 购买单位/更换后的单位
        /// </summary>
        public string SaleUnit { get; set; }

        /// <summary>
        /// 单位包装数
        /// </summary>
        public decimal? SalePackingQty { get; set; }

        /// <summary>
        /// 拣货数量/实际购买数量
        /// </summary>
        public decimal? PickQty { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductID { get; set; }

        /// <summary>
        /// 预定数量
        /// </summary>
        public decimal? PreQty { get; set; }

        /// <summary>
        /// 是否修改单位
        /// </summary>
        public int IsSet { get; set; }




        /// <summary>
        /// 购买数量/预定数量
        /// </summary>
        public decimal? SaleQty { get; set; }

        /// <summary>
        /// 配送价格(库存价格*单位包装数)
        /// </summary>
        public decimal? SalePrice { get; set; }

        /// <summary>
        /// 库存实发数量(单位包装数*实际购买数量)
        /// </summary>
        public decimal? UnitQty { get; set; }

        /// <summary>
        /// 提点金额(库存金额*库存实发数量)
        /// </summary>
        public decimal? SubAmt { get; set; }
    }

    /// <summary>
    /// 对货商品详信息
    /// </summary>
    public class PickCheckProducts
    {
        /// <summary>
        /// 拣货数量 
        /// </summary>
        public decimal? PickQty { get; set; }

        /// <summary>
        /// 对货 数量
        /// </summary>
        public decimal? CheckQty { get; set; }

        /// <summary>
        /// 商品编号
        /// </summary>
        public int? ProductID { get; set; }

        /// <summary>
        /// 对货是否正确
        /// </summary>
        public int? IsCheckRight { get; set; }
    }
}
