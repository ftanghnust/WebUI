using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 绑定列表页搜索抽象基类
    /// </summary>
    public abstract class SearchModelBase
    {
        /// <summary>
        /// 分页大小
        /// </summary>
        public int PageSize { get; set; }

        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }

    }
}