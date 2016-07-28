using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Enum
{
    /// <summary>
    /// 网仓后台枚举类
    /// </summary>
    public class WarehouseEnum
    {
        
        /// <summary>
        /// 仓库商品状态(0:已移除1:正常;2:淘汰;3:冻结;) ;淘汰商品和冻结商品不能销售;加入或重新加入时为正常；移除后再加入时为正常
        /// </summary>
        public enum WProductStatus
        {
            /// <summary>
            /// 0 已移除
            /// </summary>
            已移除 = 0,
            /// <summary>
            /// 1:正常
            /// </summary>
            正常 = 1,
            /// <summary>
            /// 3淘汰[淘汰的商品不再采购,不能再销售]
            /// </summary>
            淘汰 = 2,
            /// <summary>
            /// 2:冻结[冻结的商品不再采购,不能再销售])
            /// </summary>
            冻结 = 3
        }


    }
}