using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.WProduct
{
    /// <summary>
    /// 商品列表显示模型
    /// </summary>
    public class WProductListShowModel : BaseModel
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 基本分类
        /// </summary>
        [DisplayName("基本分类")]
        public string CategoryName { get; set; }

        /// <summary>
        /// ERP编码
        /// </summary> 
        [DisplayName("ERP编码")]
        public string Sku { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 国际条码(条码1,条码2)
        /// </summary>
        [DisplayName("国际条码")]
        public string BarcodesString { get; set; }


        /// <summary>
        /// 商品规格
        /// </summary>
        [DisplayName("商品规格")]
        public string UnitSpec { get; set; }

        /// <summary>
        /// 货位号
        /// </summary>
        [DisplayName("货位号")]
        public string ShelfName { get; set; }


        /// <summary>
        /// 库存单位
        /// </summary>
        [DisplayName("库存单位")]
        public string StockUnit { get; set; }


        /// <summary>
        /// 销售价格
        /// </summary>
        [DisplayName("销售价格")]
        public string SalePrice { get; set; }


        /// <summary>
        /// 供应商
        /// </summary>
        [DisplayName("供应商")]
        public string VendorName { get; set; }


        /// <summary>
        /// 库存数量
        /// </summary>
        [DisplayName("库存数量")]
        public string StockNum { get; set; }

    }
}