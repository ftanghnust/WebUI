using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.StockCheck
{
    public class StockAdjDetailExt
    {

        #region 改变的数量类型

        /// <summary>
        /// 库存单位数量(=AdjQty*AdjPackingQty)
        /// </summary>
        public string UnitQty { get; set; }
        /// <summary>
        /// (库存单位)采购单价(=WProducts.SalePrice)
        /// </summary>
        public string BuyPrice { get; set; }
        /// <summary>
        /// (库存单位)配送单价(=WProductsBuy.BuyPrice)
        /// </summary>
        public string SalePrice { get; set; }
        /// <summary>
        /// 调整金额(=UnitQty*BuyPrice)
        /// </summary>
        public string AdjAmt { get; set; }

        /// <summary>
        /// 调整数量
        /// </summary>
        public string AdjQty { get; set; }

        /// <summary>
        /// 包装数
        /// </summary>
        public decimal AdjPackingQty { get; set; }

      
        /// <summary>
        /// 盘点时该单位时的子机构的库存(StockQty.StockQty)
        /// </summary>
        public decimal StockQty { get; set; }

        /// <summary>
        /// 盘点库存数量(StockCheckDetails.CheckUnitQty)
        /// </summary>
        public decimal? CheckUnitQty { get; set; }

        #endregion

        //
        public string AdjID { get; set; }
        //
        public string AdjUnit { get; set; }
        //
        public string BarCode { get; set; }
        //
        public int? BrandId1 { get; set; }
        //
        public string BrandId1Name { get; set; }
        //
        public int? BrandId2 { get; set; }
        //
        public string BrandId2Name { get; set; }

        //
        public int? CategoryId1 { get; set; }
        //
        public string CategoryId1Name { get; set; }
        //
        public int? CategoryId2 { get; set; }
        //
        public string CategoryId2Name { get; set; }
        //
        public int? CategoryId3 { get; set; }
        //
        public string CategoryId3Name { get; set; }

        //
        public int? CheckUserID { get; set; }
        //
        public string CheckUserName { get; set; }
        //
        public string ID { get; set; }
        //
        public DateTime ModifyTime { get; set; }
        //
        public int? ModifyUserID { get; set; }
        //
        public string ModifyUserName { get; set; }
        //
        public int ProductId { get; set; }
        //
        public string ProductImageUrl200 { get; set; }
        //
        public string ProductImageUrl400 { get; set; }
        //
        public string ProductName { get; set; }
        //
        public string Remark { get; set; }

        //
        public int SerialNumber { get; set; }
        //
        public int? ShopCategoryId1 { get; set; }
        //
        public string ShopCategoryId1Name { get; set; }
        //
        public int? ShopCategoryId2 { get; set; }
        //
        public string ShopCategoryId2Name { get; set; }
        //
        public int? ShopCategoryId3 { get; set; }
        //
        public string ShopCategoryId3Name { get; set; }
        //
        public string SKU { get; set; }
        //
        public string StockCheckDetailsID { get; set; }
        //
        public string StockCheckID { get; set; }

        //
        public string Unit { get; set; }

        //
        public string VendorCode { get; set; }
        //
        public int VendorID { get; set; }
        //
        public string VendorName { get; set; }
        //
        public int WID { get; set; }
    }
}