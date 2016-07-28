using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Sale
{
    /// <summary>
    /// 门店结算单 单个模型
    /// </summary>
    public class SaleSettleModel
    {
        /// <summary>
        /// 
        /// </summary>
        public SaleSettle SaleSettle { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public IList<SaleSettleDetail> SaleSettleDetailList { get; set; }
    }
}