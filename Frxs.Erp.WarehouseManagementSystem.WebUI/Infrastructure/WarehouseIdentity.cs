/*********************************************************
 * FRXS(ISC) zhangliang4629@163.com 2016/3/18 20:14:26
 * *******************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI
{
    /// <summary>
    /// 当前登录用户所属的仓库
    /// </summary>
    public class WarehouseIdentity
    {
        /// <summary>
        /// 仓库ID
        /// </summary>
        public int WarehouseId { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        public string WarehouseCode { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        public string WarehouseName { get; set; }

        /// <summary>
        /// 父结构，当登录用户属于仓库根节点的时候，此对象属性与当前登录的仓库信息完全一致
        /// </summary>
        public WarehouseIdentity Parent { get; set; }

        /// <summary>
        /// 仓库下面【子机构】集合
        /// </summary>
        public List<WarehouseIdentity> ParentSubWarehouses { get; set; }

    }
}