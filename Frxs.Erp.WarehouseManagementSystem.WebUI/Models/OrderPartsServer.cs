using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Request;
using Frxs.Erp.ServiceCenter.Promotion.SDK.Resp;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers;
using Frxs.Platform.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    public class OrderPartsServer
    {
        /// <summary>
        /// 限购检查方法
        /// </summary>
        /// <param name="shopId">门店ID</param>
        /// <param name="Wid">仓库ID</param>
        /// <param name="productIds">商品ID列表</param>
        /// <param name="list">被检查的购物车或订单列表</param>
        /// <param name="userId">用户ID</param>
        /// <param name="userName">用户姓名</param>
        /// <param name="msg">返回的出错消息</param>
        /// <param name="maxList">返回的限购数量列表，如不需要返回该列表，传入null</param>
        /// <returns>true or false</returns>
        //public bool QuotaCheck(int shopId, int Wid, IList<int> productIds, List<FrxsErpPromotionSaleCartAddOrEditRequest.SaleCartAddOrEditRequestDto> list, int userId, string userName, ref string msg, List<SimpleProdcutQuoteModel> maxList = null)
        //{
        //    #region 限购判断
        //    //获取限购信息
        //    var order = GetUnConfirmOrderShop(Wid, shopId, userId, userName);
        //    var quota = GetPromotionQuota(Wid, shopId, productIds, userId, userName);
        //    //有购物车商品处于限购状态
        //    if (quota != null && quota.Count > 0)
        //    {
        //        if (maxList != null)
        //        {
        //            maxList = new List<SimpleProdcutQuoteModel>();
        //            foreach (var product in quota)
        //            {
        //                maxList.Add(new SimpleProdcutQuoteModel()
        //                {
        //                    ProductId = product.ProductID,
        //                    MaxPreQty = product.MaxOrderQty
        //                });
        //            }
        //        }
        //        foreach (var product in quota)
        //        {
        //            decimal count = 0;
        //            var cartProdcut = list.Where(x => x.ProductID == product.ProductID).FirstOrDefault();
        //            if (cartProdcut != null)
        //            {
        //                count += cartProdcut.PreQty;
        //            }
        //            //判断购物车中是否超过限制数量
        //            if (count > product.MaxOrderQty)
        //            {
        //                var tmpList = new List<int>();
        //                tmpList.Add(product.ProductID);
        //                var productInfo = GetProductsInfo(tmpList, userId, userName);
        //                if (productInfo == null || productInfo.TotalRecords <= 0)
        //                {
        //                    msg = string.Format("没有找到ID为{0}的商品", product.ProductID);
        //                    return false;
        //                }
        //                msg = string.Format("购物车中商品{0}数量为{1}件，限购{2}件", productInfo.ItemList[0].ProductName, count,
        //                                    product.MaxOrderQty);
        //                return false;
        //            }
        //            //判断未确认订单中的该商品与购物车中是否超过限制数量
        //            if (order != null)
        //            {
        //                var orderProdcut =
        //                    order.Details.Where(x => x.ProductId == product.ProductID).FirstOrDefault();
        //                if (orderProdcut != null)
        //                {
        //                    if (orderProdcut.PreQty > product.MaxOrderQty)
        //                    {
        //                        var tmpList = new List<int>();
        //                        tmpList.Add(product.ProductID);
        //                        var productInfo = GetProductsInfo(tmpList, userId, userName);
        //                        if (productInfo == null || productInfo.TotalRecords <= 0)
        //                        {
        //                            msg = string.Format("没有找到ID为{0}的商品", product.ProductID);
        //                            return false;
        //                        }
        //                        msg = string.Format("已有订单中商品{0}数量有{1}件，限购{2}件", productInfo.ItemList[0].ProductName,
        //                                            orderProdcut.PreQty, product.MaxOrderQty);
        //                        return false;
        //                    }
        //                    count += orderProdcut.PreQty;
        //                    if (count > product.MaxOrderQty)
        //                    {
        //                        var tmpList = new List<int>();
        //                        tmpList.Add(product.ProductID);
        //                        var productInfo = GetProductsInfo(tmpList, userId, userName);
        //                        if (productInfo == null || productInfo.TotalRecords <= 0)
        //                        {
        //                            msg = string.Format("没有找到ID为{0}的商品", product.ProductID);
        //                            return false;
        //                        }
        //                        msg = string.Format("购物车中商品{0}数量为{1}件，订单中为{2}件，限购{3}件",
        //                                            productInfo.ItemList[0].ProductName, cartProdcut.PreQty,
        //                                            orderProdcut.PreQty, product.MaxOrderQty);
        //                        return false;
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    #endregion

        //    return true;
        //}

        /// <summary>
        /// 获取未确认订单数据
        /// </summary>
        /// <param name="wid">仓库ID</param>
        /// <param name="shopId">门店ID</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Frxs.Erp.ServiceCenter.Promotion.SDK.Resp.FrxsErpPromotionOrderShopUnConfirmResp.FrxsErpPromotionOrderShopUnConfirmRespData GetUnConfirmOrderShop(int wid, int shopId, int userId, string userName)
        {
            var client = WorkContext.CreatePromotionSdkClient();
            var req = new FrxsErpPromotionOrderShopUnConfirmRequest()
            {
                UserId = userId,
                UserName = userName,
                ShopId = shopId,
                WarehouseId = wid,
                WID = wid
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                throw new Exception(resp.Info);
            }
            else
            {
                return resp.Data;
            }
        }

        /// <summary>
        /// 获取门店限购列表情况
        /// </summary>
        /// <param name="wid">仓库ID</param>
        /// <param name="shopId">门店ID</param>
        /// <param name="productIds">商品ID串</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IList<FrxsErpPromotionWProductPromotionDetailsListModelGetResp.FrxsErpPromotionWProductPromotionDetailsListModelGetRespData> GetPromotionQuota(int wid, int shopId, IList<int> productIds, int userId, string userName)
        {
            var productStr = "";
            foreach (var productId in productIds)
            {
                productStr += productId + ",";
            }
            productStr = productStr.Substring(0, productStr.Length - 1);
            var client = WorkContext.CreatePromotionSdkClient();
            var promotionDetails = client.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
            {
                WID = wid,
                BeginTime = DateTime.Now,
                EndTime = DateTime.Now,
                ShopID = shopId,
                ProductIDList = productStr,
                PromotionType = 1,
                UserId = userId,
                UserName = userName
            });
            if (promotionDetails.Flag != 0)
            {
                return null;
            }
            else
            {
                return promotionDetails.Data;
            }
        }

        /// <summary>
        /// 获取商品信息
        /// </summary>
        /// <param name="productIds">商品ID</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FrxsErpProductProductListGetResp.FrxsErpProductProductListGetRespData GetProductsInfo(IList<int> productIds, int userId, string userName)
        {
            var client = WorkContext.CreateProductSdkClient();
            var req = new FrxsErpProductProductListGetRequest()
            {
                ProductIds = productIds,
                PageIndex = 1,
                PageSize = int.MaxValue
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                return null;
            }
            else
            {
                return resp.Data;
            }
        }

        /// <summary>
        /// 获取仓库商品列表(排除限购)
        /// </summary>
        /// <param name="Wid">仓库ID</param>
        /// <param name="productIds">商品List</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FrxsErpProductWProductsGetToB2BResp.FrxsErpProductWProductsGetToB2BRespData GetWProductsInfo(int Wid, IList<int> productIds, int shopid, int userId, string userName)
        {
            var client = WorkContext.CreateProductSdkClient();
            var req = new FrxsErpProductWProductsGetToB2BRequest()
            {
                ShopID = shopid,
                ProductIds = productIds,
                UserId = userId,
                UserName = userName,
                WID = Wid
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                throw new Exception(resp.Info);
            }
            else
            {
                return resp.Data;
            }
        }

        /// <summary>
        /// 获取仓库商品列表(包含限购)
        /// </summary>
        /// <param name="Wid">仓库ID</param>
        /// <param name="productIds">商品List</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FrxsErpProductWProductsGetToB2BExtResp.FrxsErpProductWProductsGetToB2BExtRespData GetWProductsExtInfo(int Wid, IList<int> productIds, int userId, string userName)
        {
            var client = WorkContext.CreateProductSdkClient();
            var req = new FrxsErpProductWProductsGetToB2BExtRequest()
            {
                ProductIds = productIds,
                UserId = userId,
                UserName = userName,
                WID = Wid
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                throw new Exception(resp.Info);
            }
            else
            {
                return resp.Data;
            }
        }

        /// <summary>
        /// 获取门店返点列表情况
        /// </summary>
        /// <param name="wid">仓库ID</param>
        /// <param name="shopId">门店ID</param>
        /// <param name="productIds">商品ID串</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IList<FrxsErpPromotionWProductPromotionDetailsListModelGetResp.FrxsErpPromotionWProductPromotionDetailsListModelGetRespData> GetPromotionRebate(int wid, int shopId, IList<int> productIds, int userId, string userName)
        {
            var productStr = "";
            foreach (var productId in productIds)
            {
                productStr += productId + ",";
            }
            productStr = productStr.Substring(0, productStr.Length - 1);
            var client = WorkContext.CreatePromotionSdkClient();
            var promotionDetails = client.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
            {
                WID = wid,
                ShopID = shopId,
                ProductIDList = productStr,
                BeginTime = DateTime.Now,
                EndTime = DateTime.Now,
                PromotionType = 1,
                UserId = userId,
                UserName = userName
            });
            if (promotionDetails.Flag != 0)
            {
                return null;
            }
            else
            {
                return promotionDetails.Data;
            }
        }

        /// <summary>
        /// 获取仓库商品库存
        /// </summary>
        /// <param name="Wid">仓库ID</param>
        /// <param name="subWid">子仓库ID </param>
        /// <param name="productIds">商品List</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderStockQtyQueryResp.FrxsErpOrderStockQtyQueryRespData GetProductStock(int Wid, int subWid, IList<int> productIds, int userId, string userName)
        {
            var client = WorkContext.CreateOrderSdkClient();
            var req = new FrxsErpOrderStockQtyQueryRequest()
            {
                PDIDList = productIds.ToList(),
                WID = Wid,
                SubWID = subWid,
                UserId = userId,
                UserName = userName
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                throw new Exception(resp.Info);
            }
            else
            {
                return resp.Data;
            }
        }

        /// <summary>
        /// 获取门店促销平台费率情况
        /// </summary>
        /// <param name="wid">仓库ID</param>
        /// <param name="shopId">门店ID</param>
        /// <param name="productIds">商品ID串</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public IList<FrxsErpPromotionWProductPromotionDetailsListModelGetResp.FrxsErpPromotionWProductPromotionDetailsListModelGetRespData> GetPromotionRate(int wid, int shopId, IList<int> productIds, int userId, string userName)
        {
            var productStr = "";
            foreach (var productId in productIds)
            {
                productStr += productId + ",";
            }
            productStr = productStr.Substring(0, productStr.Length - 1);
            var client = WorkContext.CreatePromotionSdkClient();
            var promotionDetails = client.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDetailsListModelGetRequest()
            {
                WID = wid,
                ShopID = shopId,
                ProductIDList = productStr,
                PromotionType = 2,
                UserId = userId,
                UserName = userName
            });
            if (promotionDetails == null)
            {
                throw new Exception("查询平台费率信息失败");
            }
            if (promotionDetails.Flag != 0)
            {
                return null;
            }
            else
            {
                return promotionDetails.Data;
            }
        }


        /// <summary>
        /// 取线路信息
        /// </summary>
        /// <param name="lineId">线路ID</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FrxsErpProductWarehouseLineGetResp.FrxsErpProductWarehouseLineGetRespData GetWLineInfo(int lineId, int userId, string userName)
        {
            var client = WorkContext.CreateProductSdkClient();
            var req = new FrxsErpProductWarehouseLineGetRequest()
            {
                LineID = lineId,
                UserName = userName,
                UserId = userId
            };
            var resp = client.Execute(req);
            if (resp == null)
            {
                throw new Exception("查询线路信息失败");
            }
            if (resp.Flag != 0)
            {
                return null;
            }
            else
            {
                return resp.Data;
            }
        }



        /// <summary>
        /// 获取门店信息
        /// </summary>
        /// <param name="shopId">门店ID</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public FrxsErpProductShopGetResp.FrxsErpProductShopGetRespData GetShopInfo(int shopId, int userId, string userName)
        {
            var client = WorkContext.CreateProductSdkClient();
            var req = new FrxsErpProductShopGetRequest()
            {
                ShopID = shopId,
                UserName = userName,
                UserId = userId
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                return null;
            }
            else
            {
                return resp.Data;
            }
        }


        /// <summary>
        /// 添加订单跟踪消息
        /// </summary>
        /// <param name="track">跟踪消息内容</param>
        /// <param name="wid">仓库ID</param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public bool InsertOrderTrack(FrxsErpOrderSaleOrderAddTrackRequest track, int wid, int userId, string userName)
        {
            var client = WorkContext.CreateOrderSdkClient();
            var req = track;
            req.UserId = userId;
            req.UserName = userName;
            req.WarehouseId = wid;
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// 计算预计送货时间
        /// </summary>
        /// <param name="lineId"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <param name="isPD">是否排单订单</param>
        /// <returns></returns>
        public DateTime? GetSendDate(int lineId, int userId, string userName, DateTime orderDate, ref string msg, bool isPD)
        {
            if (isPD)
            {
                return DateTime.Now.Date.AddDays(1).AddSeconds(-1);
            }
            else
            {
                var client = WorkContext.CreateProductSdkClient();
                var req = new FrxsErpProductWarehouseLineGetRequest()
                {
                    LineID = lineId,
                    UserId = userId,
                    UserName = userName
                };
                var resp = client.Execute(req);
                if (resp.Flag != 0)
                {
                    msg = resp.Info;
                    return null;
                }
                else
                {
                    var line = resp.Data;
                    var endTime = resp.Data.OrderEndTime;
                    int[] sendW = new int[]
                    {
                        resp.Data.SendW1,
                        resp.Data.SendW2, 
                        resp.Data.SendW3,
                        resp.Data.SendW4, 
                        resp.Data.SendW5,
                        resp.Data.SendW6, 
                        resp.Data.SendW7
                    };
                    int wOrderDate = (int)orderDate.DayOfWeek;
                    if (wOrderDate <= 0)
                    {
                        wOrderDate += 7;
                    }
                    DateTime beginDate = orderDate.Date;
                    for (int i = 0; i < 7; i++)
                    {
                        var send = sendW[(wOrderDate + i - 1) % 7];
                        if (send == 1)
                        {
                            var date = beginDate.AddDays(i).Add(endTime);
                            if ((i == 0 && orderDate < date) || i > 0)
                            {
                                return date.Date.AddDays(1).AddSeconds(-1);
                            }
                        }
                    }
                    msg = "该线路没有配送时间设置";
                    return null;
                }
            }
        }


        /// <summary>
        /// 获取品牌列表
        /// </summary>
        public IList<FrxsErpProductBrandCategorieGetResp.BrandCategories> GetBrands(IList<int> brandList, int userId, string userName)
        {
            var client = WorkContext.CreateProductSdkClient();
            var req = new FrxsErpProductBrandCategorieGetRequest()
            {
                BrandIds = brandList.ToList(),
                UserId = userId,
                UserName = userName,
                PageSize = int.MaxValue
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                throw new Exception(resp.Info);
            }
            else
            {
                return resp.Data.ItemList;
            }
        }

        /// <summary>
        /// 获取区域信息列表
        /// </summary>
        /// <param name="regionIds"></param>
        /// <returns></returns>
        public IList<FrxsErpProductSysAreaGetByIDsResp.FrxsErpProductSysAreaGetByIDsRespData> GetRegions(IList<int> regionIds)
        {
            var client = WorkContext.CreateProductSdkClient();
            var req = new FrxsErpProductSysAreaGetByIDsRequest()
            {
                Ids = regionIds.ToList()
            };
            var resp = client.Execute(req);
            if (resp.Flag != 0)
            {
                throw new Exception(resp.Info);
            }
            else
            {
                return resp.Data;
            }
        }

        /// <summary>
        /// 限购检查方法
        /// </summary>
        ///<param name="type">提交类型，0从购物车提交（需检查未确认订单） 1从订单修改提交，不需检查未确认订单</param>
        /// <param name="shopId">门店ID</param>
        /// <param name="Wid">仓库ID</param>
        /// <param name="productIds">商品ID列表</param>
        /// <param name="list">被检查的购物车或订单列表</param>
        /// <param name="userId">用户ID</param>
        /// <param name="userName">用户姓名</param>
        /// <param name="msg">返回的出错消息</param>
        /// <param name="maxList">返回的限购数量列表，如不需要返回该列表，传入null</param>
        /// <returns>true or false</returns>
        public bool QuotaCheck(int shopId, int Wid, IList<int> productIds, List<WarehouseOrderDetails> list, int userId, string userName, ref string msg, List<SimpleProdcutQuoteModel> maxList = null)
        {
            #region 限购判断
            var quota = GetPromotionQuota(Wid, shopId, productIds, userId, userName);
            //有购物车商品处于限购状态
            if (quota != null && quota.Count > 0)
            {
                if (maxList != null)
                {
                    maxList = new List<SimpleProdcutQuoteModel>();
                    foreach (var product in quota)
                    {
                        maxList.Add(new SimpleProdcutQuoteModel()
                        {
                            ProductId = product.ProductID,
                            MaxPreQty = product.MaxOrderQty
                        });
                    }
                }
                foreach (var item in list)
                {
                    var quotaItem = quota.FirstOrDefault(x => x.ProductID == item.ProductId);
                    if (quotaItem != null)
                    {
                        if (item.SaleQty > quotaItem.MaxOrderQty && quotaItem.MaxOrderQty != 0)
                        {
                            msg = string.Format("商品{0}数量限购{1}件", item.ProductName, quotaItem.MaxOrderQty);
                            return false;
                        }
                    }

                }
            }
            #endregion

            return true;
        }

        /// <summary>
        /// 简单的限购数据类，用于返回一个限购最大数量的列表
        /// </summary>
        public class SimpleProdcutQuoteModel
        {
            /// <summary>
            /// 商品ID
            /// </summary>
            public int ProductId { get; set; }

            /// <summary>
            /// 最大限购数量
            /// </summary>
            public decimal MaxPreQty { get; set; }
        }
    }
}