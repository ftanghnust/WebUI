
using Frxs.Platform.Utility.Filter;
/*****************************
* Author:CR
*
* Date:2016-03-24
******************************/
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;


namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// SaleBackPreDetails实体类
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class SaleBackPreDetailsModel : BaseModel
    {
        /// <summary>
        /// 行号
        /// </summary>
        [DataMember]
        [DisplayName("行号")]
        public int RowNum { get; set; }

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
        [DisplayName("名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ProductName { get; set; }

        /// <summary>
        /// 退货单位
        /// </summary>
        [DataMember]
        [DisplayName("单位")]
        public string BackUnit { get; set; }

        /// <summary>
        /// 退货单位数量
        /// </summary>
        [DataMember]
        [DisplayName("退货数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BackQtystr { get; set; }

        /// <summary>
        /// 退货单位价格
        /// </summary>
        [DataMember]
        [DisplayName("退货价")]
        public string BackPricestr { get; set; }

        /// <summary>
        /// 门店库存单位平台率(%)(=WProducts.ShopAddPerc)
        /// </summary>
        [DataMember]
        [DisplayName("平台费率")]
        public string ShopAddPercstr { get; set; }

        /// <summary>
        /// 调整后的金额(=UnitQty*UnitPrice)
        /// </summary>
        [DataMember]
        [DisplayName("小计金额")]
        public string SubAmtstr { get; set; }


        /// <summary>
        /// 退货单位包装数(固定为库存单位)
        /// </summary>
        [DataMember]
        [DisplayName("包装数")]
        public string BackPackingQtystr { get; set; }


        /// <summary>
        /// 库存单位数量(=BackPackingQty*BackQty)
        /// </summary>
        [DataMember]
        [DisplayName("总数量")]
        public string UnitQtystr { get; set; }


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
        [DisplayName("商品分类")]
        public string ShopCategoryName { get; set; }


        /// <summary>
        /// 库存单位
        /// </summary>
        [DataMember]
        [DisplayName("库存单位")]
        public string Unit { get; set; }


        #region 模型
        /// <summary>
        /// 退货单位数量
        /// </summary>
        [DataMember]
        [DisplayName("退货数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public decimal BackQty { get; set; }

        /// <summary>
        /// 退货单位价格
        /// </summary>
        [DataMember]
        [DisplayName("退货价")]
        [ExcelNoExport]
        public double BackPrice { get; set; }

        /// <summary>
        /// 门店库存单位平台率(%)(=WProducts.ShopAddPerc)
        /// </summary>
        [DataMember]
        [DisplayName("平台费率")]
        [ExcelNoExport]
        public decimal ShopAddPerc { get; set; }

        /// <summary>
        /// 调整后的金额(=UnitQty*UnitPrice)
        /// </summary>
        [DataMember]
        [DisplayName("小计金额")]
        [ExcelNoExport]
        public decimal SubAmt { get; set; }


        /// <summary>
        /// 退货单位包装数(固定为库存单位)
        /// </summary>
        [DataMember]
        [DisplayName("包装数")]
        [ExcelNoExport]
        public decimal BackPackingQty { get; set; }


        /// <summary>
        /// 库存单位数量(=BackPackingQty*BackQty)
        /// </summary>
        [DataMember]
        [DisplayName("总数量")]
        [ExcelNoExport]
        public decimal UnitQty { get; set; }



        /// <summary>
        /// 编号(WID-GUID)
        /// </summary>
        [DataMember]
        [DisplayName("编号(WID-GUID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ID { get; set; }

        /// <summary>
        /// 退货单编号
        /// </summary>
        [DataMember]
        [DisplayName("退货单编号")]
        [ExcelNoExport]
        public string BackID { get; set; }

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
        /// 库存单位价格
        /// </summary>
        [DataMember]
        [DisplayName("库存单位价格")]
        [ExcelNoExport]
        public double UnitPrice { get; set; }




        /// <summary>
        /// 最小单位积分
        /// </summary>
        [DataMember]
        [ExcelNoExport]
        public decimal BasePoint { get; set; }



        /// <summary>
        /// 库存单位物流费率(=WProducts.VendorPerc1)
        /// </summary>
        [DataMember]
        [ExcelNoExport]
        public decimal VendorPerc1 { get; set; }

        /// <summary>
        /// 库存单位仓储费率(=WProducts.VendorPerc2)
        /// </summary>
        [DataMember]
        [ExcelNoExport]
        public decimal VendorPerc2 { get; set; }


        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [ExcelNoExport]
        public decimal SubAddAmt { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]
        [ExcelNoExport]
        public string Remark { get; set; }

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
        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称")]
        [ExcelNoExport]
        public string ModifyUserName { get; set; }

        #endregion

        [ExcelNoExport]
        public decimal MaxSalePrice { get; set; }
    }

}