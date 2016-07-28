/*****************************
* Author:罗涛
*
* Date:2016/6/27 17:22:12
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.DaySettlement
{
    public class DaySettlementDetailSearchModel : BasePageModel
    {
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string RefSet_ID { get; set; }
    }
}