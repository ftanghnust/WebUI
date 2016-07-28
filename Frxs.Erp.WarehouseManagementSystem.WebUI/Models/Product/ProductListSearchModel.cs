using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 商品加入列表查询实体
    /// </summary>
    public class ProductListSearchModel : BasePageModel
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