/*****************************
* Author:罗涛
*
* Date:2016/4/26 17:57:44
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Again
{
    public class AgainDetail
    {
        /// <summary>
        /// 商品ID
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 申请数量
        /// </summary>
        public decimal AppQty { get; set; }
        /// <summary>
        /// 单位名称
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 包装数
        /// </summary>
        public decimal PackingQty { get; set; }
        /// <summary>
        /// 采购价格
        /// </summary>
        public decimal Price { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}