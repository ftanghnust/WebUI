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
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class OperatorLogModel
    {

    }

    public class OperatorLogQuery : BasePageModel
    {
        /// <summary>
        /// 系统ID
        /// </summary>
        [Required]
        public int WID { get; set; }

        /// <summary>
        /// IP地址
        /// </summary>
        public string IPAddress { get; set; }

        /// <summary>
        /// 菜单ID
        /// </summary>
        [Required]
        public int? MenuID { get; set; }

        /// <summary>
        /// 动作值
        /// </summary>
        [Required]
        public string Action { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [Required]
        public string OperatorName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [Required]
        public string Remark { get; set; }

        /// <summary>
        /// 搜索开始时间
        /// </summary>
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 搜索结束时间
        /// </summary>
        public DateTime? EndTime { get; set; }
    }
}