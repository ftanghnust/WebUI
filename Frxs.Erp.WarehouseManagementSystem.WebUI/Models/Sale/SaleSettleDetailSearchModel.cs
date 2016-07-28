using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Sale
{
    /// <summary>
    /// 查询要结算的单据查询条件
    /// </summary>
    public class SaleSettleDetailSearchModel
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public int ShopID { get; set; }

        /// <summary>
        /// 单据编号
        /// </summary>
        public int? BillID { get; set; }


        /// <summary>
        /// 账单日期开始时间
        /// </summary>
        public DateTime? StartDate { get; set; }

        /// <summary>
        /// 账单日期结束时间
        /// </summary>
        public DateTime? EndDate { get; set; }

        /// <summary>
        /// 单据类型
        /// </summary>
        public int? BillType { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        public int? WID { get; set; }

    }
}