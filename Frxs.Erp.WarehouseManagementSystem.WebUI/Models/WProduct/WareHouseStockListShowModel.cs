using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.WProduct
{
    public class WareHouseStockListShowModel : BaseModel
    {
        public int ProductId { get; set; }

        public int WId { get; set; }

        public int SubWId { get; set; }

        public string SubWarehouseName { get; set; }

        public decimal StockQty { get; set; }

    }
}