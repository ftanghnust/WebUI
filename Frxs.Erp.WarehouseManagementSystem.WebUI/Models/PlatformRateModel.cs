using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using System.Web.Mvc;
using Frxs.Platform.Utility.Map;
using System.Runtime.Serialization;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class PlatformRateQuery : BasePageModel
    {
        /// <summary>
        /// 促销类型(1:门店积分促销;2:平台费率促销)
        /// </summary>
        [DataMember]
        [DisplayName("促销类型")]
        public int PromotionType { get; set; }

        [DataMember]
        [DisplayName("单据号")]
        public string PromotionID { get; set; }

        [DataMember]
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        [DataMember]
        [DisplayName("ERP编码")]
        public string SKU { get; set; }

        [DataMember]
        [DisplayName("商品条码")]
        public string BarCode { get; set; }

        [DataMember]
        [DisplayName("单据状态")]
        public string Status { get; set; }

        [DataMember]
        [DisplayName("活动主题")]
        public string PromotionName { get; set; }

        [DataMember]
        [DisplayName("生效开始时间")]
        public DateTime? BeginTimeFrom { get; set; }

        [DataMember]
        [DisplayName("生效结束时间")]
        public DateTime? BeginTimeEnd { get; set; }
    }

    public class ShopListQuery : BasePageModel
    {
        [DataMember]
        [DisplayName("")]
        public string searchType { get; set; }

        [DataMember]
        [DisplayName("")]
        public string searchKey { get; set; }
    }

    public class PlatformRateModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 主键ID(WID+ID服务表)
        /// </summary>
        [DataMember]
        [DisplayName("主键ID(WID+ID服务表)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string PromotionID { get; set; }

        /// <summary>
        /// 促销类型(1:门店积分促销;2:平台费率促销)
        /// </summary>
        [DataMember]
        [DisplayName("促销类型(1:门店积分促销;2:平台费率促销)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int PromotionType { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WID { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WCode { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        [DisplayName("仓库名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WName { get; set; }

        /// <summary>
        /// 活动主题
        /// </summary>
        [DataMember]
        [DisplayName("活动主题")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string PromotionName { get; set; }

        /// <summary>
        /// 生效开始时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("生效开始时间(yyyy-MM-dd HH:mm:ss)")]

        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 生效结束时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("生效结束时间(yyyy-MM-dd HH:mm:ss)")]

        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 状态(0:录单;1:已确认;2:已生效;3:已停用)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:录单;1:已确认;2:已生效;3:已停用)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Status { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        [DisplayName("提交时间")]

        public DateTime? ConfTime { get; set; }

        /// <summary>
        /// 提交用户ID
        /// </summary>
        [DataMember]
        [DisplayName("提交用户ID")]

        public int? ConfUserID { get; set; }

        /// <summary>
        /// 提交用户名称
        /// </summary>
        [DataMember]
        [DisplayName("提交用户名称")]

        public string ConfUserName { get; set; }

        /// <summary>
        /// 过帐时间
        /// </summary>
        [DataMember]
        [DisplayName("过帐时间")]

        public DateTime? PostingTime { get; set; }

        /// <summary>
        /// 过帐用户ID
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户ID")]

        public int? PostingUserID { get; set; }

        /// <summary>
        /// 过帐用户名称
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户名称")]

        public string PostingUserName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]

        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]

        public string CreateUserName { get; set; }

        /// <summary>
        /// 最后修改时间(停用时也更新该字段)
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间(停用时也更新该字段)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID(停用时也更新该字段)
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID(停用时也更新该字段)")]

        public int? ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称(停用时也更新该字段)
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称(停用时也更新该字段)")]

        public string ModifyUserName { get; set; }

        #endregion

        #region 从表
        /// <summary>
        /// 详情表集合
        /// </summary>
        public IList<WProductPromotionDetails> DetailsList { get; set; }

        /// <summary>
        /// 门店关联表集合
        /// </summary>
        public IList<WProductPromotionShops> ShopList { get; set; }
        #endregion

        public string jsonProduct { get; set; }
        public string jsonGroup { get; set; }

        public partial class WProductPromotionDetails
        {

            #region 模型
            /// <summary>
            /// 主键ID
            /// </summary>
            [DataMember]
            [DisplayName("主键ID")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ID { get; set; }

            /// <summary>
            /// 促销ID(WProductPromotion.PromotionID)
            /// </summary>
            [DataMember]
            [DisplayName("促销ID(WProductPromotion.PromotionID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string PromotionID { get; set; }

            /// <summary>
            /// 仓库ID(Warehouse.WID 二级)
            /// </summary>
            [DataMember]
            [DisplayName("仓库ID(Warehouse.WID 二级)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int WID { get; set; }

            /// <summary>
            /// 仓库商品ID(WProducts.WProductID)
            /// </summary>
            [DataMember]
            [DisplayName("仓库商品ID(WProducts.WProductID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int WProductID { get; set; }

            /// <summary>
            /// 商品ID(product.ProductID)
            /// </summary>
            [DataMember]
            [DisplayName("商品ID(product.ProductID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ProductID { get; set; }

            /// <summary>
            /// 商品编码(Product.SKU)
            /// </summary>
            [DataMember]
            [DisplayName("商品编码(Product.SKU)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string SKU { get; set; }

            /// <summary>
            /// 商品名称(Products.ProductName)
            /// </summary>
            [DataMember]
            [DisplayName("商品名称(Products.ProductName)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string ProductName { get; set; }

            /// <summary>
            /// 库存单位(WProducts.Unit)
            /// </summary>
            [DataMember]
            [DisplayName("库存单位(WProducts.Unit)")]

            public string Unit { get; set; }

            /// <summary>
            /// 配送(销售)单位包装数)( WProducts.BigPackingQty)  2016-6-20 按照会议要求，类型统一由int改成decimal
            /// </summary>
            [DataMember]
            [DisplayName("配送(销售)单位包装数)( WProducts.BigPackingQty)")]

            public decimal? PackingQty { get; set; }

            /// <summary>
            /// 配送(销售)单位( WProducts.BigUnit)
            /// </summary>
            [DataMember]
            [DisplayName("配送(销售)单位( WProducts.BigUnit)")]

            public string SaleUnit { get; set; }

            /// <summary>
            /// 库存单位销售价格(WProducts.SalePrice)
            /// </summary>
            [DataMember]
            [DisplayName("库存单位销售价格(WProducts.SalePrice)")]

            public decimal? SalePrice { get; set; }

            /// <summary>
            /// 门店库存单位原积分(PomotionType=1:WProducts.ShopPoint;PomotionType=2: WProducts.AddShopPerc)
            /// </summary>
            [DataMember]
            [DisplayName("门店库存单位原积分(PomotionType=1:WProducts.ShopPoint;PomotionType=2: WProducts.AddShopPerc)")]

            public decimal? OldPoint { get; set; }

            /// <summary>
            /// PomotionType=1:门店库存单位促销积分;PomotionType=2:门店库存单位平台费率
            /// </summary>
            [DataMember]
            [DisplayName("PomotionType=1:门店库存单位促销积分;PomotionType=2:门店库存单位平台费率")]
            [Required(ErrorMessage = "{0}不能为空")]
            public decimal Point { get; set; }

            /// <summary>
            /// 每单订购上限数量(库存单位; 0:不受限;>0 受限) PomotionType=1才有用到;
            /// </summary>
            [DataMember]
            [DisplayName("每单订购上限数量(库存单位; 0:不受限;>0 受限) PomotionType=1才有用到;")]
            [Required(ErrorMessage = "{0}不能为空")]
            public decimal MaxOrderQty { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [DataMember]
            [DisplayName("创建时间")]
            [Required(ErrorMessage = "{0}不能为空")]
            public DateTime CreateTime { get; set; }

            /// <summary>
            /// 创建用户ID
            /// </summary>
            [DataMember]
            [DisplayName("创建用户ID")]

            public int? CreateUserID { get; set; }

            /// <summary>
            /// 创建用户名称
            /// </summary>
            [DataMember]
            [DisplayName("创建用户名称")]

            public string CreateUserName { get; set; }

            #endregion

        }

        public partial class WProductPromotionDetails
        {
            /// <summary>
            /// 商品条码
            /// </summary>
            [DataMember]
            [DisplayName("商品条码")]
            public string BarCode { get; set; }
        }

        public partial class WProductPromotionShops
        {
            #region 模型
            /// <summary>
            /// 主键ID
            /// </summary>
            [DataMember]
            [DisplayName("主键ID")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ID { get; set; }

            /// <summary>
            /// 促销ID(WProductPromotion.PromotionID)
            /// </summary>
            [DataMember]
            [DisplayName("促销ID(WProductPromotion.PromotionID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string PromotionID { get; set; }

            /// <summary>
            /// 仓库ID(Warehouse.WID 二级)
            /// </summary>
            [DataMember]
            [DisplayName("仓库ID(Warehouse.WID 二级)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int WID { get; set; }

            /// <summary>
            /// 门店ID(Shop.ShopID)
            /// </summary>
            [DataMember]
            [DisplayName("门店ID(Shop.ShopID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ShopID { get; set; }

            /// <summary>
            /// 门店编号(Shop.ShopCode)
            /// </summary>
            [DataMember]
            [DisplayName("门店编号(Shop.ShopCode)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string ShopCode { get; set; }

            /// <summary>
            /// 门店名称(Shop.ShopName)
            /// </summary>
            [DataMember]
            [DisplayName("门店名称(Shop.ShopName)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string ShopName { get; set; }

            /// <summary>
            /// 门店类型(0:加盟店;1:签约店;)
            /// </summary>
            [DataMember]
            [DisplayName("门店类型(0:加盟店;1:签约店;)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ShopType { get; set; }

            /// <summary>
            /// 门店地址全称
            /// </summary>
            [DataMember]
            [DisplayName("门店地址全称")]

            public string FullAddress { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [DataMember]
            [DisplayName("创建时间")]
            [Required(ErrorMessage = "{0}不能为空")]
            public DateTime CreateTime { get; set; }

            /// <summary>
            /// 创建用户ID
            /// </summary>
            [DataMember]
            [DisplayName("创建用户ID")]

            public int? CreateUserID { get; set; }

            /// <summary>
            /// 创建用户名称
            /// </summary>
            [DataMember]
            [DisplayName("创建用户名称")]

            public string CreateUserName { get; set; }

            #endregion
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetPlatformRatePageData(PlatformRateQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreatePromotionSdkClient();
                //Dictionary<string, object> conditionDict = base.PrePareFormParam();
                int? status = null;
                if (!String.IsNullOrEmpty(cpm.Status))
                {
                    status = int.Parse(cpm.Status);
                }
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionListGetRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    PromotionType = cpm.PromotionType,
                    PromotionID = cpm.PromotionID,
                    ProductName = cpm.ProductName,
                    PromotionName = cpm.PromotionName,
                    SKU = cpm.SKU,
                    BarCode = cpm.BarCode,
                    Status = status,
                    BeginTimeFrom = cpm.BeginTimeFrom,
                    BeginTimeEnd = cpm.BeginTimeEnd,
                    //SortBy = cpm.sort + " " + cpm.order,
                    SortBy = "CreateTime" + " " + "desc",
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
                throw ex;
            }
            return jsonStr;
        }

        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="buyids">主键</param>
        /// <returns>对象</returns>
        public int DeletePlatformRate(string ids, int warehouseId, int promotionType)
        {
            var IdList = ids.Split(',').ToList();
            var ServiceCenter = WorkContext.CreatePromotionSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionDelRequest()
            {
                IdList = IdList,
                PromotionType = promotionType,
                WarehouseId = warehouseId
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                if (promotionType.Equals(1))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4C,
                                           ConstDefinition.XSOperatorActionDel, string.Format("{0}门店积分促销单[{1}]", ConstDefinition.XSOperatorActionDel, ids));
                }
                else if (promotionType.Equals(2))
                {
                    OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2G,
                                           ConstDefinition.XSOperatorActionDel, string.Format("{0}门店平台费率调整单[{1}]", ConstDefinition.XSOperatorActionDel, ids));
                }

                result = resp.Data;
            }
            return result;
        }

        public PlatformRateModel GetPlatformRateData(int warehouseId, string promotionID)
        {
            var model = new PlatformRateModel();
            if (!String.IsNullOrEmpty(promotionID))
            {
                var ServiceCenter = WorkContext.CreatePromotionSdkClient();
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionGetRequest()
                {
                    WarehouseId = warehouseId,
                    PromotionID = promotionID
                });
                if (resp != null && resp.Flag == 0)
                {
                    model = AutoMapperHelper.MapTo<PlatformRateModel>(resp.Data.wProductPromotion);
                    IList<WProductPromotionDetails> productdetails = new List<WProductPromotionDetails>();
                    productdetails = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Promotion.SDK.Resp.FrxsErpPromotionWProductPromotionGetResp.WProductPromotionDetails, WProductPromotionDetails>(resp.Data.detailsList);
                    model.DetailsList = productdetails;
                    IList<WProductPromotionShops> shopGroups = new List<WProductPromotionShops>();
                    shopGroups = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Promotion.SDK.Resp.FrxsErpPromotionWProductPromotionGetResp.WProductPromotionShops, WProductPromotionShops>(resp.Data.shopList);
                    model.ShopList = shopGroups;
                }
            }
            return model;
        }

        /// <summary>
        /// 平台费率调整单“复制”功能，复制单据时要更新商品明细内容
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="promotionID">源单据号</param>
        /// <returns></returns>
        public PlatformRateModel GetPlatformRateDataCopy(int warehouseId, string promotionID)
        {
            var model = new PlatformRateModel();
            if (!String.IsNullOrEmpty(promotionID))
            {
                var ServiceCenter = WorkContext.CreatePromotionSdkClient();
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionGetRequest()
                {
                    WarehouseId = warehouseId,
                    PromotionID = promotionID,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                });
                if (resp != null && resp.Flag == 0)
                {
                    model = AutoMapperHelper.MapTo<PlatformRateModel>(resp.Data.wProductPromotion);
                    IList<WProductPromotionDetails> productdetails = new List<WProductPromotionDetails>();
                    //要更新数据的新明细列表
                    IList<WProductPromotionDetails> newDetails = new List<WProductPromotionDetails>();
                    productdetails = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Promotion.SDK.Resp.FrxsErpPromotionWProductPromotionGetResp.WProductPromotionDetails, WProductPromotionDetails>(resp.Data.detailsList);

                    #region 更新明细数据
                    //获取到商品服务中心客户端SDK访问对象
                    var productSdkClient = WorkContext.CreateProductSdkClient();
                    var productRequestDto = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetRequest()
                    {
                        WID = warehouseId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    };

                    var productdetailsCopy = new List<WProductPromotionDetails>();
                    if (productdetails != null && productdetails.Count > 0)
                    {
                        foreach (var item in productdetails)
                        {
                            productRequestDto.ProductId = item.ProductID;
                            var productResp = productSdkClient.Execute(productRequestDto);
                            if (productResp != null && productResp.Flag == 0 && productResp.Data != null && productResp.Data.WProduct != null)
                            {
                                if (productResp.Data.WProduct.WStatus == 1)
                                {
                                    item.SalePrice = productResp.Data.WProduct.SalePrice;
                                    productdetailsCopy.Add(item);
                                }
                            }
                        }
                    }
                    #endregion

                    model.DetailsList = productdetailsCopy;
                    IList<WProductPromotionShops> shopGroups = new List<WProductPromotionShops>();
                    shopGroups = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Promotion.SDK.Resp.FrxsErpPromotionWProductPromotionGetResp.WProductPromotionShops, WProductPromotionShops>(resp.Data.shopList);
                    model.ShopList = shopGroups;
                }
            }
            return model;
        }

        /// <summary>
        /// 积分促销调整单“复制”功能，复制单据时要更新商品明细内容
        /// </summary>
        /// <param name="warehouseId">仓库ID</param>
        /// <param name="promotionID">源单据号</param>
        /// <returns></returns>
        public PlatformRateModel GetScorePromotionDataCopy(int warehouseId, string promotionID)
        {
            var model = new PlatformRateModel();
            if (!String.IsNullOrEmpty(promotionID))
            {
                var ServiceCenter = WorkContext.CreatePromotionSdkClient();
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWProductPromotionGetRequest()
                {
                    WarehouseId = warehouseId,
                    PromotionID = promotionID,
                    UserId = WorkContext.UserIdentity.UserId,
                    UserName = WorkContext.UserIdentity.UserName
                });
                if (resp != null && resp.Flag == 0)
                {
                    model = AutoMapperHelper.MapTo<PlatformRateModel>(resp.Data.wProductPromotion);
                    IList<WProductPromotionDetails> productdetails = new List<WProductPromotionDetails>();
                    //要更新数据的新明细列表
                    IList<WProductPromotionDetails> newDetails = new List<WProductPromotionDetails>();
                    productdetails = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Promotion.SDK.Resp.FrxsErpPromotionWProductPromotionGetResp.WProductPromotionDetails, WProductPromotionDetails>(resp.Data.detailsList);

                    #region 更新明细数据
                    //获取到商品服务中心客户端SDK访问对象
                    var productSdkClient = WorkContext.CreateProductSdkClient();
                    var productRequestDto = new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetRequest()
                    {
                        WID = warehouseId,
                        UserId = WorkContext.UserIdentity.UserId,
                        UserName = WorkContext.UserIdentity.UserName
                    };

                    var productdetailsCopy = new List<WProductPromotionDetails>();
                    if (productdetails != null && productdetails.Count > 0)
                    {
                        foreach (var item in productdetails)
                        {
                            productRequestDto.ProductId = item.ProductID;
                            var productResp = productSdkClient.Execute(productRequestDto);
                            if (productResp != null && productResp.Flag == 0 && productResp.Data != null && productResp.Data.WProduct != null)
                            {
                                if (productResp.Data.WProduct.WStatus == 1)
                                {
                                    item.SalePrice = productResp.Data.WProduct.SalePrice * productResp.Data.WProduct.BigPackingQty;
                                    productdetailsCopy.Add(item);
                                }
                            }
                        }
                    }
                    #endregion

                    model.DetailsList = productdetailsCopy;
                    IList<WProductPromotionShops> shopGroups = new List<WProductPromotionShops>();
                    shopGroups = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Promotion.SDK.Resp.FrxsErpPromotionWProductPromotionGetResp.WProductPromotionShops, WProductPromotionShops>(resp.Data.shopList);
                    model.ShopList = shopGroups;
                }
            }
            return model;
        }
    }
}