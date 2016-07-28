/*****************************
* Author:罗涛
*
* Date:2016/4/27 15:14:52
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Again
{
    public class AgainSearchModel : BasePageModel
    {
        /// <summary>
        /// 仓库编号，必须填写
        /// </summary>
        public int WID { get; set; }

        /// <summary>
        /// 申请单
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 申请类型(0:返配;1:补货)
        /// </summary>
        public int AppType { get; set; }

        /// <summary>
        /// Status
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 所属子仓库ID
        /// </summary>
        public int? SubWID { get; set; }

        /// <summary>
        /// 开单时间
        /// </summary>
        public DateTime? AppDateStart { get; set; }

        /// <summary>
        /// 开单时间I
        /// </summary>
        public DateTime? AppDateEnd { get; set; }

        /// <summary>
        /// 对账时间
        /// </summary>
        public DateTime? PostingTimeStart { get; set; }

        /// <summary>
        /// 对账时间
        /// </summary>
        public DateTime? PostingTimeEnd { get; set; }
    }
}