/*****************************
* Author:罗涛
*
* Date:2016/4/15 10:30:01
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Import
{
    public class ImportPriceModel
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 商品SKU
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 配送单位
        /// </summary>
        public string SaleUnit { get; set; }

        /// <summary>
        /// 包装数量
        /// </summary>
        public decimal PackingQty { get; set; }

        /// <summary>
        /// 配送价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 最小单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 最小单位价格
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 门店库存单位提点率(%)
        /// </summary>
        public decimal ShopAddPerc { get; set; }

        /// <summary>
        /// 门店库存单位积分
        /// </summary>
        public decimal ShopPoint { get; set; }

        /// <summary>
        /// 库存单位绩效积分（司机及门店绩效分)
        /// </summary>
        public decimal BasePoint { get; set; }

        /// <summary>
        /// 库存单位物流费率(供应商) (%)
        /// </summary>
        public decimal VendorPerc1 { get; set; }

        /// <summary>
        /// 库存单位仓储费率(供应商)(%)
        /// </summary>
        public decimal VendorPerc2 { get; set; }


        /// <summary>
        /// 新配送价格
        /// </summary>
        public decimal NewPrice { get; set; }

        /// <summary>
        /// 新门店积分
        /// </summary>
        public decimal NewShopPoint { get; set; }

        /// <summary>
        /// 新绩效分率
        /// </summary>
        public decimal NewBasePoint { get; set; }


        /// <summary>
        /// 新物流费率
        /// </summary>
        public decimal NewVendorPerc1 { get; set; }


        /// <summary>
        /// 新仓储费率
        /// </summary>
        public decimal NewVendorPerc2 { get; set; }

        /// <summary>
        /// 新平台费率
        /// </summary>
        public decimal NewShopAddPerc { get; set; }


        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal BuyPrice { get; set; }

        /// <summary>
        /// 返配数量
        /// </summary>
        public decimal AppQty { get; set; }

        /// <summary>
        /// 库存
        /// </summary>
        public decimal WStock { get; set; }
        

        /// <summary>
        /// 行号
        /// </summary>
        public int Index;

        /// <summary>
        /// 小单位 导入的时候用到
        /// </summary>
        public string MinUnit;

    }
}