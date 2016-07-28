using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility.Log;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility;
using Frxs.Erp.ServiceCenter.Order.SDK.Request;
using Frxs.Platform.Utility.Map;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;
using Frxs.ServiceCenter.SSO.Client;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Order
{
    [ValidateInput(false)]
    public class BatchRecommendController : BaseController
    {
        #region 视图
        public ActionResult BatchRecommendList()
        {
            return View();
        }

        public ActionResult BatchRecommendAddOrEdit()
        {
            return View();
        }

        public ActionResult OrderSelect()
        {
            return View();
        }

        /// <summary>
        /// 仓库订单 选择配送商品 包括限购 视图 带库存、在途、可用数据
        /// </summary>
        /// <returns></returns>
        public ActionResult SearchSaleProductStockXG()
        {
            return View();
        }
        #endregion

        /// <summary>
        /// 获取批量改单数据
        /// </summary>
        /// <param name="EditID"></param>
        /// <returns></returns>
        public ActionResult GetBatchRecommendInfo(string EditID)
        {
            string result = string.Empty;
            //获取数据，绑定
            var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleEditGetRequest()
            {
                EditId = EditID,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            if (resp != null && resp.Flag == 0)
            {
                var PIDList = from o in resp.Data.Details select o.ProductId;
                var OrderServiceCenter = WorkContext.CreateOrderSdkClient();
                var OrderStockQty = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockQtyQueryRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SubWID = resp.Data.SaleEdit.SubWID.Value,
                    PDIDList = PIDList.ToList()
                });
                List<SaleOrderDetailExt> saleProductExts = new List<SaleOrderDetailExt>();

                var newList = new List<FrxsErpOrderSaleEditGetResp.SaleEditDetailResponseDto>();
                foreach (var item in resp.Data.Details)
                {
                    var tmp = newList.FirstOrDefault(x => x.SKU == item.SKU);
                    if (tmp == null)
                    {
                        newList.Add(item);
                    }
                }

                var respTemp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsAddedListGetRequest()
                {
                    PageIndex = 1,
                    PageSize = int.MaxValue,
                    WID = resp.Data.SaleEdit.WID,
                    WStatus = 1,
                    ProductIds = PIDList.ToList()
                });

               
                foreach (var item in newList)
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

                    if (respTemp != null && respTemp.Data != null)
                    {
                        var tempDetails = respTemp.Data.ItemList.FirstOrDefault(i => i.ProductId == item.ProductId);
                        if (tempDetails != null)
                        {
                            tempproduct.MaxSalePrice = tempDetails.SalePrice;
                        }
                    }

                    saleProductExts.Add(tempproduct);
                }

                var data = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderSaleEditGetResp.SaleEditOrderResponseDto, SaleEditOrderExt>(resp.Data.Order);
                result = new { SaleEdit = resp.Data.SaleEdit, Order = data, Details = saleProductExts }.ToJsonString();
            }
            return Content(result);
        }

        /// <summary>
        /// 扩展get方法返回模型
        /// </summary>
        public class SaleEditOrderExt
        {
            #region 模型
            /// <summary>
            /// 主键ID(WID+GUID)
            /// </summary>
            public string ID { get; set; }

            /// <summary>
            /// 改单ID（GUID)
            /// </summary>
            public string EditID { get; set; }

            /// <summary>
            /// 仓库ID(Warehouse.WID)
            /// </summary>
            public int WID { get; set; }

            /// <summary>
            /// 订单编号
            /// </summary>
            public string OrderId { get; set; }

            /// <summary>
            /// 下单时间
            /// </summary>
            public DateTime OrderDate { get; set; }

            /// <summary>
            /// 门店ID
            /// </summary>
            public int ShopID { get; set; }

            /// <summary>
            /// 门店编号
            /// </summary>
            public string ShopCode { get; set; }

            /// <summary>
            /// 门店名称
            /// </summary>
            public string ShopName { get; set; }

            /// <summary>
            /// 最后修改时间
            /// </summary>
            public DateTime CreateTime { get; set; }

            /// <summary>
            /// 最后修改用户ID
            /// </summary>
            public int CreateUserID { get; set; }

            /// <summary>
            /// 最后修改用户名称
            /// </summary>
            public string CreateUserName { get; set; }

            public DateTime? SendDate { get; set; }

            public decimal? PayAmount { get; set; }

            public string LineName { get; set; }

            public int? LineID { get; set; }

            public int? Status { get; set; }

            public string StatusName { get; set; }

            public string ShopTypeName { get; set; }

            #endregion
        }

        public class SaleOrderDetailExt : Frxs.Erp.ServiceCenter.Order.SDK.Resp.FrxsErpOrderSaleEditGetResp.SaleEditDetailResponseDto
        {
            //库存数量
            public decimal Stock { get; set; }

            //在途数量
            public decimal OnTheWay { get; set; }

            //可用数量
            public decimal Available { get; set; }

            //大单位价格
            public decimal MaxSalePrice { get; set; }
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="editids"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520313, 52031303)]
        public ActionResult DeleteBatchRecommend(string editids)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(editids))
                {
                    var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleEditDeleteListRequest()
                    {
                        EditId = editids,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });
                    if (resp != null && resp.Flag == 0)
                    {
                        //写操作日志
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3C,
                                               ConstDefinition.XSOperatorActionDel, string.Format("{0}商品批量推荐单号[{1}]", ConstDefinition.XSOperatorActionDel, editids));

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
                            Info = resp.Info
                        }.ToJsonString();
                    }
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中删除数据"
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
        /// 批量更改状态
        /// </summary>
        /// <param name="editids"></param>
        /// <param name="statuss"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520313, new int[] { 52031304, 52031305, 52031306 })]
        public ActionResult BatchRecommendChangeStatus(string editids, int status)
        {
            string result = string.Empty;
            try
            {
                if (!string.IsNullOrEmpty(editids))
                {
                    var resp = this.ErpOrderSdkClient.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleEditUpdateStatusListRequest()
                    {
                        EditId = editids,
                        Status = status,
                        WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    });
                    if (resp != null && resp.Flag == 0)
                    {
                        //写操作日志
                        string action = string.Empty;
                        if (status == 1)       //确认
                        {
                            action = ConstDefinition.XSOperatorActionSure;
                        }
                        else if (status == 2)  //过账
                        {
                            action = ConstDefinition.XSOperatorActionPasting;
                        }
                        else if (status == 0)   //反确认
                        {
                            action = ConstDefinition.XSOperatorActionNoSure;
                        }
                        OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3C,
                                              action, string.Format("{0}商品批量推荐单号[{1}]", action, editids));

                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_SUCCESS,
                            Info = "OK",
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
                            Info = resp.Info
                        }.ToJsonString();
                    }
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "未选中确认数据"
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
        /// 批量改单新增或编辑
        /// </summary>
        /// <param name="jsonEdit"></param>
        /// <param name="jsonProducts"></param>
        /// <param name="jsonOrders"></param>
        /// <returns></returns>
        [AuthorizeButtonFiter(520313, new int[] { 52031301, 52031302 })]
        public ActionResult BatchRecommendAddOrEditeHandle(string jsonEdit, string jsonProducts, string jsonOrders)
        {
            int flag = 0;
            string result = string.Empty;
            try
            {
                var edit = Frxs.Platform.Utility.Json.JsonHelper.FromJson<EditModel>(jsonEdit);
                var eproducts = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<EditProduct>(jsonProducts);
                var editOrders = Frxs.Platform.Utility.Json.JsonHelper.GetObjectIList<EditOrder>(jsonOrders);

                #region 合并商品数据
                var newdetails = new List<EditProduct>();
                foreach (var item in eproducts)
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
                eproducts = newdetails;
                #endregion

                int warehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                int userid = WorkContext.UserIdentity.UserId;
                string username = WorkContext.UserIdentity.UserName;

                var part = new OrderPartsServer();
                var PIDList = from o in eproducts select o.ProductId;
                //1.调用接口，获取网仓商品信息
                var wProudcts = part.GetWProductsExtInfo(warehouseId, PIDList.ToList(), userid, username);
                //2.调用接口，获取商品信息
                var products = part.GetProductsInfo(PIDList.ToList(), userid, username);
                //3.调用接口，获取品牌信息
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
                //4.调用接口 取库存信息
                var productStock = part.GetProductStock(warehouseId, edit.SubWID, PIDList.ToList(), userid, username);

                //开始组装数据
                #region 取主表数据
                //1.主表数据
                FrxsErpOrderSaleEditAddOrEditRequest.SaleEditModelRequestDto editdto = new FrxsErpOrderSaleEditAddOrEditRequest.SaleEditModelRequestDto();
                if (string.IsNullOrEmpty(edit.EditID))
                {
                    string editid = new BatchRecommendModel().GetEditID();
                    if (string.IsNullOrEmpty(editid))
                    {
                        result = new ResultData
                        {
                            Flag = ConstDefinition.FLAG_FAIL,
                            Info = "获取批量改单号失败"
                        }.ToJsonString();
                        return Content(result);
                    }
                    editdto.EditID = editid;
                    editdto.CreateTime = DateTime.Now;
                    editdto.CreateUserID = userid;
                    editdto.CreateUserName = username;
                }
                else
                {
                    editdto.EditID = edit.EditID;
                    flag = 1;

                }
                editdto.WID = warehouseId;
                editdto.WCode = WorkContext.CurrentWarehouse.Parent.WarehouseCode;
                editdto.WName = WorkContext.CurrentWarehouse.Parent.WarehouseName;
                WarehouseIdentity subwareHouse = WorkContext.CurrentWarehouse.ParentSubWarehouses.FirstOrDefault(o => o.WarehouseId == edit.SubWID);
                editdto.SubWID = edit.SubWID;
                editdto.SubWCode = subwareHouse.WarehouseCode;
                editdto.SubWName = subwareHouse.WarehouseName;
                editdto.EditDate = edit.EditDate;
                editdto.Remark = edit.Remark;
                editdto.Status = edit.Status;
                editdto.ModifyTime = DateTime.Now;
                editdto.ModifyUserID = userid;
                editdto.ModifyUserName = username;
                #endregion

                IList<FrxsErpOrderSaleEditAddOrEditRequest.SaleEditOrderRequestDto> Order = new List<FrxsErpOrderSaleEditAddOrEditRequest.SaleEditOrderRequestDto>();
                IList<FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailsRequestDto> Details = new List<FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailsRequestDto>();
                IList<FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailExtsRequestDto> DetailExts = new List<FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailExtsRequestDto>();

                foreach (var order in editOrders)
                {
                    //5.调用接口 取商品促销积分
                    var promotionProducts = part.GetPromotionRebate(warehouseId, order.ShopID, PIDList.ToList(), userid, username);

                    //6.调用获取限购商品接口
                    var wProudctXG = part.GetWProductsInfo(warehouseId, PIDList.ToList(), order.ShopID, userid, username);

                    //7.调用接口 取促销平台费率
                    var rProducts = part.GetPromotionRate(warehouseId, order.ShopID, PIDList.ToList(), userid, username);

                    var editProducts = AutoMapperHelper.MapToList<EditProduct, EditProduct>(eproducts);

                    //移除限购商品
                    if (wProudctXG.ItemList.Count != editProducts.GroupBy(x => x.ProductId).Count())
                    {
                        for (int i = editProducts.Count - 1; i >= 0; i--)
                        {
                            var detail = editProducts[i];
                            var tmp = wProudctXG.ItemList.FirstOrDefault(x => x.ProductId == detail.ProductId);
                            if (tmp == null)
                            {
                                editProducts.Remove(detail);
                            }
                        }
                    }
                    if (editProducts.Count <= 0)
                    {
                        continue;
                    }

                    //限购检查
                    #region 限购检查
                    var list = new List<WarehouseOrderDetails>();
                    foreach (var detail in eproducts)
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
                    string msg = "";
                    List<Frxs.Erp.WarehouseManagementSystem.WebUI.OrderPartsServer.SimpleProdcutQuoteModel> maxList = new List<Frxs.Erp.WarehouseManagementSystem.WebUI.OrderPartsServer.SimpleProdcutQuoteModel>();
                    var flag1 = part.QuotaCheck(order.ShopID, warehouseId, PIDList.ToList(), list, userid, username, ref msg, maxList);
                    if (!flag1)//有限购商品超过限制
                    {
                        foreach (var detail in eproducts)
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


                    #region 取订单信息
                    FrxsErpOrderSaleEditAddOrEditRequest.SaleEditOrderRequestDto editOrder = new FrxsErpOrderSaleEditAddOrEditRequest.SaleEditOrderRequestDto();
                    editOrder.ID = warehouseId + Guid.NewGuid().ToString();
                    editOrder.EditID = editdto.EditID;
                    editOrder.WID = warehouseId;
                    editOrder.OrderID = order.OrderID;
                    editOrder.OrderDate = order.OrderDate;
                    editOrder.SendDate = order.SendDate;
                    editOrder.ShopID = order.ShopID;
                    editOrder.ShopCode = order.ShopCode;
                    editOrder.ShopName = order.ShopName;
                    //editOrder.ProcFlag = 0;
                    editOrder.CreateTime = DateTime.Now;
                    editOrder.CreateUserID = userid;
                    editOrder.CreateUserName = username;
                    Order.Add(editOrder);
                    #endregion

                    foreach (var model in editProducts)
                    {
                        string id = warehouseId + Guid.NewGuid().ToString();

                        #region 取商品信息
                        FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailsRequestDto editProduct = new FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailsRequestDto();
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
                        editProduct.ID = id;
                        editProduct.EditID = editdto.EditID;
                        editProduct.OrderID = order.OrderID;
                        editProduct.WID = warehouseId;
                        editProduct.ProductId = model.ProductId;
                        editProduct.SKU = model.SKU;
                        editProduct.ProductName = model.ProductName;
                        editProduct.BarCode = model.BarCode;
                        editProduct.ProductImageUrl200 = wProduct.ImageUrl200x200;          //从网仓商品中取得
                        editProduct.ProductImageUrl400 = wProduct.ImageUrl400x400;          //从网仓商品中取得
                        editProduct.WCProductID = (int)wProduct.WProductID;
                        editProduct.SaleUnit = model.SaleUnit;
                        editProduct.SalePackingQty = model.SalePackingQty;
                        //editProduct.SalePrice = model.SalePrice;
                        if (model.SalePackingQty != 1)  //销售单位
                        {
                            editProduct.SalePrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0) * (wProduct.BigPackingQty.HasValue ? wProduct.BigPackingQty.Value : 1);
                        }
                        else
                        {
                            editProduct.SalePrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0);
                        }
                        editProduct.SaleQty = model.SaleQty;
                        editProduct.Unit = model.Unit;
                        editProduct.UnitQty = (model.SalePackingQty * model.SaleQty);
                        //editProduct.UnitPrice = model.UnitPrice;
                        editProduct.UnitPrice = (wProduct.SalePrice.HasValue ? wProduct.SalePrice.Value : 0);
                        editProduct.PromotionUnitPrice = editProduct.UnitPrice;
                        editProduct.SubAmt = (model.SalePrice * model.SaleQty);
                        if (rProduct == null)
                        {
                            editProduct.ShopAddPerc = wProduct.ShopAddPerc.HasValue ? wProduct.ShopAddPerc.Value : 0;
                        }
                        else
                        {
                            editProduct.ShopAddPerc = rProduct.Point;
                        }
                        editProduct.ShopPoint = wProduct.ShopPoint.HasValue ? wProduct.ShopPoint.Value : 0;
                        editProduct.BasePoint = wProduct.BasePoint.HasValue ? wProduct.BasePoint.Value : 0;
                        editProduct.VendorPerc1 = wProduct.VendorPerc1.HasValue ? wProduct.VendorPerc1.Value : 0;
                        editProduct.VendorPerc2 = wProduct.VendorPerc2.HasValue ? wProduct.VendorPerc2.Value : 0;
                        editProduct.SubAddAmt = editProduct.ShopAddPerc * editProduct.SubAmt;
                        editProduct.SubBasePoint = editProduct.SubAmt * editProduct.BasePoint;
                        editProduct.SubVendor1Amt = editProduct.VendorPerc1 * editProduct.SubAmt;
                        editProduct.SubVendor2Amt = editProduct.VendorPerc2 * editProduct.SubAmt;
                        if (pProduct != null)
                        {
                            editProduct.PromotionShopPoint = pProduct.Point;
                            if (editProduct.PromotionShopPoint > 0)
                            {
                                editProduct.SubPoint = editProduct.PromotionShopPoint * editProduct.UnitQty;
                            }
                            else
                            {
                                editProduct.SubPoint = editProduct.ShopPoint * editProduct.UnitQty;
                            }
                        }
                        else
                        {
                            editProduct.PromotionShopPoint = 0;
                            editProduct.SubPoint = editProduct.ShopPoint * editProduct.UnitQty;
                        }
                        editProduct.ShelfAreaID = wProduct.ShelfAreaID;
                        editProduct.ShelfAreaCode = wProduct.ShelfAreaCode;
                        editProduct.ShelfAreaName = wProduct.ShelfAreaName;
                        editProduct.ShelfID = wProduct.ShelfID;
                        editProduct.ShelfCode = wProduct.ShelfCode;
                        if (stock != null)
                        {
                            editProduct.StockQty = stock.StockQty;
                        }
                        editProduct.Remark = model.Remark;
                        editProduct.ModifyTime = DateTime.Now;
                        editProduct.ModifyUserID = userid;
                        editProduct.ModifyUserName = username;
                        Details.Add(editProduct);
                        #endregion

                        #region 取商品扩展信息
                        FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailExtsRequestDto detailExt = new FrxsErpOrderSaleEditAddOrEditRequest.SaleEditDetailExtsRequestDto();
                        detailExt.ID = id;
                        detailExt.EditID = editdto.EditID;
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
                        DetailExts.Add(detailExt);
                        #endregion
                    }
                }

                if (Order.Count == 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "所选订单所有商品均已被限购，不能改单"
                    }.ToJsonString();
                    return Content(result);
                }
                var orderserviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = orderserviceCenter.Execute(new FrxsErpOrderSaleEditAddOrEditRequest()
                {
                    WarehouseId = warehouseId,
                    SaleEdit = editdto,
                    Order = Order,
                    Details = Details,
                    DetailExts = DetailExts,
                    Type = flag,
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
                    OperatorLogHelp.Write(ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_3C,
                          action, string.Format("{0}商品批量推荐单号[{1}]", action, editdto.EditID));

                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功",
                        Data = new
                        {
                            EditID = editdto.EditID,
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

        /// <summary>
        /// 批量推荐列表
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public ActionResult GetBatchRecommendList(BatchRecommendSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new BatchRecommendModel().GetBatchRecommendPageData(searchModel);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// 批量改单 主表模型
        /// </summary>
        public class EditModel
        {
            public string EditID { get; set; }
            public DateTime EditDate { get; set; }
            public int SubWID { get; set; }
            public int Status { get; set; }
            public string Remark { get; set; }
        }

        /// <summary>
        /// 批量改单 商品模型
        /// </summary>
        public class EditProduct
        {
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

            public string Remark { get; set; }
            public string BarCode { get; set; }
        }

        /// <summary>
        /// 批量改单 订单模型
        /// </summary>
        public class EditOrder
        {
            public string OrderID { get; set; }
            public DateTime OrderDate { get; set; }
            public DateTime SendDate { get; set; }
            public int ShopID { get; set; }
            public string ShopCode { get; set; }
            public string ShopName { get; set; }
        }
    }
}
