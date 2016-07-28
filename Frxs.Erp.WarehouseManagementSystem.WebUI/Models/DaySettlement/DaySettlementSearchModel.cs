/*****************************
* Author:罗涛
*
* Date:2016/6/27 16:24:29
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.DaySettlement
{
    public class DaySettlementSearchModel : BasePageModel
    {
        public string SerchTime { get; set; }
        public int SubWID { get; set; }
        public string StockName { get; set; }
    }
}