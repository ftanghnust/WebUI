/*****************************
* Author:罗涛
*
* Date:2016/4/26 17:56:58
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Again
{
    public class Again
    {
        /// <summary>
        /// 自仓库ID
        /// </summary>
        public int SubWID { get; set; }
        /// <summary>
        /// 单号
        /// </summary>
        public string AppId { get; set; }

        /// <summary>
        /// 单据类型（0返配 1补货）
        /// </summary>
        public int Type { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

    }
}