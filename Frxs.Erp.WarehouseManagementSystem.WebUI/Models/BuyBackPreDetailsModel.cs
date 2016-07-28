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
using Frxs.Platform.Utility.Filter;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// BuyBackPreDetailsModel实体类
    /// </summary>
    [Serializable]
    public partial class BuyBackPreDetailsModel : BaseModel
    {
        /// <summary>
        /// 采购单编号
        /// </summary>
        [DataMember]
        [DisplayName("单号")]
        [ExcelNoExport]
        public string BackID { get; set; }

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
        /// 采购单位
        /// </summary>
        [DataMember]
        [DisplayName("单位")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BackUnit { get; set; }

        /// <summary>
        /// 采购单位数量
        /// </summary>
        [DataMember]
        [DisplayName("退货数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BackQtystr { get; set; }

        /// <summary>
        /// 采购单位价格
        /// </summary>
        [DataMember]
        [DisplayName("进价")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BackPricestr { get; set; }

        /// <summary>
        /// 采购的总金额(=UnitQty*UnitPrice)
        /// </summary>
        [DataMember]
        [DisplayName("金额")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubAmtstr { get; set; }


        /// <summary>
        /// 采购单位包装数(固定为库存单位)
        /// </summary>
        [DataMember]
        [DisplayName("包装数")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BackPackingQtystr { get; set; }

        /// <summary>
        /// 库存单位数量(=BackPackingQty*BackQty)
        /// </summary>
        [DataMember]
        [DisplayName("总数量")]
        [Required(ErrorMessage = "{0}不能为空")]
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
        [Required(ErrorMessage = "{0}不能为空")]
        public string Unit { get; set; }


        #region 模型
        /// <summary>
        /// 采购单位数量
        /// </summary>
        [DataMember]
        [DisplayName("退货数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public decimal BackQty { get; set; }

        /// <summary>
        /// 采购单位价格
        /// </summary>
        [DataMember]
        [DisplayName("进价")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public double BackPrice { get; set; }

        /// <summary>
        /// 采购的总金额(=UnitQty*UnitPrice)
        /// </summary>
        [DataMember]
        [DisplayName("金额")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public double SubAmt { get; set; }


        /// <summary>
        /// 采购单位包装数(固定为库存单位)
        /// </summary>
        [DataMember]
        [DisplayName("包装数")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public decimal BackPackingQty { get; set; }

        /// <summary>
        /// 库存单位数量(=BackPackingQty*BackQty)
        /// </summary>
        [DataMember]
        [DisplayName("总数量")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public decimal UnitQty { get; set; }


        /// <summary>
        /// 编号(仓库ID+GUID)
        /// </summary>
        [DataMember]
        [DisplayName("编号(仓库ID+GUID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ID { get; set; }

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
        [DisplayName("商品编号")]
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
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ProductImageUrl400 { get; set; }







        /// <summary>
        /// 采购单位价格 备份
        /// </summary>
        [DataMember]
        [DisplayName("采购价")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public double OldBuyPrice { get; set; }


        /// <summary>
        /// 库存数量
        /// </summary>
        [DataMember]
        [DisplayName("库存数量")]
        [ExcelNoExport]
        public decimal WStock { get; set; }





        /// <summary>
        /// 库存单位价格
        /// </summary>
        [DataMember]
        [DisplayName("库存单位价格")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public double UnitPrice { get; set; }



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

        /// <summary>
        /// 配送价格 库存单位
        /// </summary>
        [DataMember]
        [DisplayName("配送价格")]
        [ExcelNoExport]
        public decimal SalePrice { get; set; }

        #endregion

        [ExcelNoExport]
        public decimal MaxSalePrice { get; set; }
        [ExcelNoExport]
        public decimal MinSalePrice { get; set; }
        [ExcelNoExport]
        public decimal MaxBuyPrice { get; set; }
        [ExcelNoExport]
        public decimal MinBuyPrice { get; set; }

    }
}