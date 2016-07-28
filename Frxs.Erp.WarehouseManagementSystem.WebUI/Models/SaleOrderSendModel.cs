using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using System.Web.Mvc;
using Frxs.Platform.Utility.Map;
using System.Runtime.Serialization;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class SaleOrderSendQuery
    {
        public string ShopCode { get; set; }
        public string ShopName { get; set; }
    }
}