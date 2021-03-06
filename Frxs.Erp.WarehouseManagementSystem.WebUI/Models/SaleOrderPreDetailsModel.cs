﻿
using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility.Filter;
/*****************************
* Author:CR
*
* Date:2016-04-18
******************************/
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;



namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// SaleOrderPreDetails实体类
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class SaleOrderPreDetailsModel : BaseModel
    {

        /// <summary>
        /// 是否促销
        /// </summary>
        [DataMember]
        [DisplayName("是否促销")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string IsPromotion { get; set; }

        /// <summary>
        /// 商品SKU(ERP编码)
        /// </summary>
        [DataMember]
        [DisplayName("商品编码")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SKU { get; set; }

        /// <summary>
        /// 描述商品名称(Product.ProductName)
        /// </summary>
        [DataMember]
        [DisplayName("商品名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ProductName { get; set; }

        /// <summary>
        /// 配送(销售)单位
        /// </summary>
        [DataMember]
        [DisplayName("单位")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SaleUnit { get; set; }

        /// <summary>
        /// 配送(销售)单位包装数
        /// </summary>
        [DataMember]
        [DisplayName("包装数")]
        [Required(ErrorMessage = "{0}不能为空")]
        public decimal SalePackingQty { get; set; }

        /// <summary>
        /// 配送(销售)预定数量
        /// </summary>
        [DataMember]
        [DisplayName("订货数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        public decimal PreQty { get; set; }

        /// <summary>
        /// 实发数量(拣货完成写入)预设为PreQty
        /// </summary>
        [DataMember]
        [DisplayName("发货数量")]
        public decimal? SaleQty { get; set; }

        /// <summary>
        /// 配送(销售)单位价格(=ifnull(PromotionPrice,UnitPrice) *SalePackingQty)
        /// </summary>
        [DataMember]
        [DisplayName("配送价")]
        [Required(ErrorMessage = "{0}不能为空")]
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 门店库存单位平台率(%)(=WProducts.ShopAddPerc)
        /// </summary>
        [DataMember]
        [DisplayName("平台费率")]
        public decimal? ShopAddPerc { get; set; }

        /// <summary>
        /// 小计门店提点金额,平台费用(=ShopAddPerc*SubAmt*0.01)
        /// </summary>
        [DataMember]
        [DisplayName("平台费")]
        public decimal? SubAddAmt { get; set; }

        /// <summary>
        /// 库存单位实发数量=SalePackingQty*SaleQty
        /// </summary>
        [DataMember]
        [DisplayName("总数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        public decimal UnitQty { get; set; }

        /// <summary>
        /// 调整后的最终金额(=ifnull(PromotionPrice,UnitPrice) *UnitQty)
        /// </summary>
        [DataMember]
        [DisplayName("金额")]
        public decimal? SubAmt { get; set; }

        /// <summary>
        /// 小计门店积分(=ifnull(PromotionShopPoint,ShopPoint)*UnitQty)
        /// </summary>
        [DataMember]
        [DisplayName("门店积分")]
        public decimal? SubPoint { get; set; }

        /// <summary>
        /// 库存单位(WProducts.Unit)
        /// </summary>
        [DataMember]
        [DisplayName("基本单位")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Unit { get; set; }

        /// <summary>
        /// 商品的国际条码
        /// </summary>
        [DataMember]
        [DisplayName("国际条码")]
        public string BarCode { get; set; }

        /// <summary>
        /// 运营分类
        /// </summary>
        [DataMember]
        [DisplayName("运营分类")]
        public string ShopCategoryName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]
        public string Remark { get; set; }

        #region 模型
        /// <summary>
        /// 编号(仓库ID+GUID)
        /// </summary>
        [DataMember]
        [DisplayName("编号(仓库ID+GUID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ID { get; set; }

        /// <summary>
        /// 订单编号(SaleOrder.OrderID)
        /// </summary>
        [DataMember]
        [DisplayName("订单编号(SaleOrder.OrderID)")]
        [ExcelNoExport]
        public string OrderID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public int WID { get; set; }

        /// <summary>
        /// 商品编号(Prouct.ProductID)
        /// </summary>
        [DataMember]
        [DisplayName("商品编号(Prouct.ProductID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public int ProductId { get; set; }

       
        /// <summary>
        /// 商品图片用于移动端(Products.ImageUrl200*200)
        /// </summary>
        [DataMember]
        [DisplayName("商品图片用于移动端(Products.ImageUrl200*200)")]
        [ExcelNoExport]
        public string ProductImageUrl200 { get; set; }

        /// <summary>
        /// 商品图片用于PC端(Products.ImageUrl400*400)
        /// </summary>
        [DataMember]
        [DisplayName("商品图片用于PC端(Products.ImageUrl400*400)")]
        [ExcelNoExport]
        public string ProductImageUrl400 { get; set; }

        /// <summary>
        /// 仓库商品主键ID（WCProduct.WCProductID）
        /// </summary>
        [DataMember]
        [DisplayName("仓库商品主键ID（WCProduct.WCProductID）")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public int WCProductID { get; set; }

        /// <summary>
        /// 库存单位价格(SalePrice/SalePackingQty,四舍五入到4位小数)
        /// </summary>
        [DataMember]
        [DisplayName("库存单位价格(SalePrice/SalePackingQty,四舍五入到4位小数)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 库存单位促销价格(WProductPromotionDetails.PromotionPrice)
        /// </summary>
        [DataMember]
        [DisplayName("库存单位促销价格(WProductPromotionDetails.PromotionPrice)")]
        [ExcelNoExport]
        public decimal? PromotionUnitPrice { get; set; }


        /// <summary>
        /// 门店库存单位积分(=WProducts.ShopPoint)
        /// </summary>
        [DataMember]
        [DisplayName("门店库存单位积分(=WProducts.ShopPoint)")]
        [ExcelNoExport]
        public decimal? ShopPoint { get; set; }

        /// <summary>
        /// 门店库存单位促销积分(=WProductPromotionDetails.PromotionShopPoint)
        /// </summary>
        [DataMember]
        [DisplayName("门店库存单位促销积分(=WProductPromotionDetails.PromotionShopPoint)")]
        [ExcelNoExport]
        public decimal? PromotionShopPoint { get; set; }

        /// <summary>
        /// 库存单位绩效积分(=WProducts.BasePoint)
        /// </summary>
        [DataMember]
        [DisplayName("库存单位绩效积分(=WProducts.BasePoint)")]
        [ExcelNoExport]
        public decimal? BasePoint { get; set; }

        /// <summary>
        /// 库存单位物流费率(=WProducts.VendorPerc1)
        /// </summary>
        [DataMember]
        [DisplayName("库存单位物流费率(=WProducts.VendorPerc1)")]
        [ExcelNoExport]
        public decimal? VendorPerc1 { get; set; }

        /// <summary>
        /// 库存单位仓储费率(=WProducts.VendorPerc2)
        /// </summary>
        [DataMember]
        [DisplayName("库存单位仓储费率(=WProducts.VendorPerc2)")]
        [ExcelNoExport]
        public decimal? VendorPerc2 { get; set; }

        /// <summary>
        /// 小计绩效积分(=BasePoint*UnitQty)
        /// </summary>
        [DataMember]
        [DisplayName("小计绩效积分(=BasePoint*UnitQty)")]
        [ExcelNoExport]
        public decimal? SubBasePoint { get; set; }

        /// <summary>
        /// 小计供应商物流费(=VendorPerc1*SubAmt*0.01)
        /// </summary>
        [DataMember]
        [DisplayName("小计供应商物流费(=VendorPerc1*SubAmt*0.01)")]
        [ExcelNoExport]
        public decimal? SubVendor1Amt { get; set; }

        /// <summary>
        /// 小计供应商仓储费(=VendorPerc2*SubAmt*0.01)
        /// </summary>
        [DataMember]
        [DisplayName("小计供应商仓储费(=VendorPerc2*SubAmt*0.01)")]
        [ExcelNoExport]
        public decimal? SubVendor2Amt { get; set; }

        /// <summary>
        /// 是否为后来录单员添加商品(0:不是;1:是的)
        /// </summary>
        [DataMember]
        [DisplayName("是否为后来录单员添加商品(0:不是;1:是的)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public int IsAppend { get; set; }

        /// <summary>
        /// 追加的ID(SaleEdit.Edit)
        /// </summary>
        [DataMember]
        [DisplayName("追加的ID(SaleEdit.Edit)")]
        [ExcelNoExport]
        public string EditID { get; set; }

        /// <summary>
        /// 是否缺货(1:缺货;0/null:不缺货)
        /// </summary>
        [DataMember]
        [DisplayName("是否缺货(1:缺货;0/null:不缺货)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public int IsStockOut { get; set; }

        /// <summary>
        /// 应用的单品促销编号(WProductPromotion.PromotionID)
        /// </summary>
        [DataMember]
        [DisplayName("应用的单品促销编号(WProductPromotion.PromotionID)")]
        [ExcelNoExport]
        public string PromotionID { get; set; }

        /// <summary>
        /// 应用的单品促销名称(WProductPromotion.PromotionName)
        /// </summary>
        [DataMember]
        [DisplayName("应用的单品促销名称(WProductPromotion.PromotionName)")]
        [ExcelNoExport]
        public string PromotionName { get; set; }



        /// <summary>
        /// 序号(输入的序号,每一个单据从1开始)
        /// </summary>
        [DataMember]
        [DisplayName("序号(输入的序号,每一个单据从1开始)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public int SerialNumber { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]
        [ExcelNoExport]
        public int? ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称")]
        [ExcelNoExport]
        public string ModifyUserName { get; set; }

        #endregion
    }
}