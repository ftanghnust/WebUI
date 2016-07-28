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
    [Serializable]
    public class WarehouseLineShopModel
    {
        #region 模型
        /// <summary>
        /// 线路ID(WarehouseLine.LineID)
        /// </summary>
        [Required(ErrorMessage = "{0}不能为空")]
        public int LineID { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        public string idList { get; set; } 

        #endregion
    }
}