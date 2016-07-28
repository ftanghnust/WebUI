/*****************************
* Author:罗涛
*
* Date:2016/4/7 10:30:33
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class AdjustPriceListSearchModel:BasePageModel
    {
        /// <summary>
        /// 调整单单号
        /// </summary>
        public int? AdjID { get; set; }
        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品（以前叫ERP编码）
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// 状态(0:未提交;1:已确认;2:已过帐;)
        /// </summary>
        public int? Status { get; set; }

        //单据类型（0:采购(进货)价; 1:配送(批发)价; 3:费率及积分）
        public int AdjType { get; set; }

        /// <summary>
        /// 商品条形码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 生效时间1
        /// </summary>
        public DateTime? PostingTime1 { get; set; }
        /// <summary>
        /// 生效时间2
        /// </summary>
        public DateTime? PostingTime2 { get; set; }
        
    }
}