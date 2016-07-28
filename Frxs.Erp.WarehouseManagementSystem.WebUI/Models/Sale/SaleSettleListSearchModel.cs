using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Sale
{
    /// <summary>
    /// 销售结算单列表查询
    /// </summary>
    public class SaleSettleListSearchModel : BasePageModel
    {
        /// <summary>
        /// 门店编号
        /// </summary>
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 结算单号
        /// </summary>
        public string SettleID { get; set; }

        /// <summary>
        /// 仓库
        /// </summary>
        public int Wid { get; set; }


        /// <summary>
        /// 结算开始时间
        /// </summary>
        public DateTime? StartTime { get; set; }

        /// <summary>
        /// 结算结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 结算方式
        /// </summary>
        public string SettleType { get; set; }


        /// <summary>
        /// 结算单状态
        /// </summary>
        public int? Status { get; set; }


    }

    /// <summary>
    ///获取结算单单个数据查询条件模型
    /// </summary>
    public class GetSaleSettleInfoModel
    {
        /// <summary>
        /// 结算单号
        /// </summary>
        public string SettleID { get; set; }



    }
}