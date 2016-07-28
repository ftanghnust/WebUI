using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 供应商商品列表查询参数模型
    /// </summary>
    public class VendorProductsListSearchModel : BasePageModel
    {
        /// <summary>
        /// 供应商编号
        /// </summary>
        public int VendorID { get; set; }

        /// <summary>
        /// 编码，条码，商品名称
        /// </summary>
        public string KeyWord { get; set; }


        public int? IsMaster { get; set; }

    }
}