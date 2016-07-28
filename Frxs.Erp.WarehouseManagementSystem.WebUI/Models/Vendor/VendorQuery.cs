using Frxs.Platform.Utility.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 供应商查询条件实体定义
    /// </summary>
    public class VendorQuery : BasePageModel
    {
        #region 模型

        /// <summary>
        /// 供应商编号
        /// </summary>
        [DisplayName("编号")]
        public string VendorCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [DisplayName("名称")]
        public string VendorName { get; set; }

        /// <summary>
        /// 供应商类型(VendorType.VendorTypeID)
        /// </summary>
        [DisplayName("类型")]
        public string VendorTypeID { get; set; }

        /// <summary>
        /// 联系人姓名
        /// </summary>
        [DisplayName("联系人")]
        public string LinkMan { get; set; }


        /// <summary>
        /// 状态(1:正常;0:冻结)
        /// </summary>
        [DisplayName("状态")]
        public int? Status { get; set; }

        /// <summary>
        /// 结算方式( 数据字典: VendorSettleTimeType)
        /// </summary>
        [DisplayName("结算方式")]
        public string SettleTimeType { get; set; }

        /// <summary>
        /// 仓库编号(指定了仓库编号的，只查询对应的仓库供应商)
        /// </summary>
        [DisplayName("仓库编号")]
        public int? WID { get; set; }

        /// <summary>
        /// 账期
        /// </summary>
        [DisplayName("账期")]
        public string PaymentDateType { get; set; }
        #endregion
    }
}