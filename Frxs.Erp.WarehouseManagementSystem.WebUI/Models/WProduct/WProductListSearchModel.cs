using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.WProduct
{
    /// <summary>
    /// 网仓商品列表
    /// </summary>
    public class WProductListSearchModel : BasePageModel
    {
        /// <summary>
        /// 商品名称
        /// </summary>
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// ERP编码
        /// </summary>
        [DisplayName("ERP编码")]
        public string Sku { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        [DisplayName("商品条码")]
        public string BarCode { get; set; }


        /// <summary>
        /// 主供应商
        /// </summary>
        [DisplayName("主供应商")]
        public string VendorName { get; set; }


        ///// <summary>
        ///// 是否添加供应商
        ///// </summary>
        //[DisplayName("是否添加供应商")]
        //public int? AddedVendor { get; set; }


        /// <summary>
        /// 商品状态
        /// </summary>
        [DisplayName("状态")]
        public int? WStatus { get; set; }


        /// <summary>
        /// 一级基本分类
        /// </summary>
        [DisplayName("基本分类")]
        public int? CategoriesId1 { get; set; }

        /// <summary>
        ///二级基本分类
        /// </summary>
        public int? CategoriesId2 { get; set; }


        /// <summary>
        ///三级基本分类
        /// </summary>
        public int? CategoriesId3 { get; set; }
    }
}