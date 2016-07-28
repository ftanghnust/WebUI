using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Sale
{
    /// <summary>
    /// 单个对象显示模型
    /// </summary>
    public class SaleSettleViewModel
    {

        /// <summary>
        /// 结算主表
        /// </summary>
        public SaleSettle SaleSettle { get; set; }


        /// <summary>
        /// 结算明细
        /// </summary>
        public string SaleSettleDetailList { get; set; }

    }


    public class SaleSettleDetailList
    {
        public int total { get; set; }

        /// <summary>
        /// 结算明细
        /// </summary>
        public List<SaleSettleDetail> rows { get; set; }
    }

    // 摘要:
    //     SaleSettle销售结算主表
    public class SaleSettle
    {
        // 摘要:
        //     退货总金额
        [DisplayName("退货总金额")]
        public decimal BackAmt { get; set; }

        //
        // 摘要:
        //     费用总金额(可以正负)
        [DisplayName("费用总金额")]
        public decimal FeeAmt { get; set; }

        //
        // 摘要:
        //     备注
        [DisplayName("备注")]
        public string Remark { get; set; }
        //
        // 摘要:
        //     销售总金额
        [DisplayName("销售总金额")]
        public decimal SaleAmt { get; set; }
        //
        // 摘要:
        //     结算总金额(只能正，不能负)
        [DisplayName("结算总金额")]
        public decimal SettleAmt { get; set; }
        //
        // 摘要:
        //     结算ID
        [DisplayName("结算编号")]
        public string SettleID { get; set; }
        //
        // 摘要:
        //     结算时间
        [DisplayName("结算时间")]
        public DateTime SettleTime { get; set; }
        //
        // 摘要:
        //     状态(0:未结算[预留];1:已确认[预留];2:已过帐)
        public int Status { get; set; }
        //
        // 摘要:
        //     仓库ID(WarehouseID)
        [DisplayName("仓库编号")]
        public int WID { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DisplayName("仓库名称")]
        public string WName { get; set; }

        /// <summary>
        /// 仓库编码
        /// </summary>
        [DisplayName("仓库编码")]
        public string WCode { get; set; }


        /// <summary>
        /// 订单ID(SaleOrder.OrderID)
        /// </summary>
        [DisplayName("订单编号")]
        public string OrderID { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [DisplayName("门店ID")]
        public int ShopID { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DisplayName("门店编号")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店类型(0:加盟店;1:签约店;)
        /// </summary>
        [DisplayName("门店类型")]
        public int ShopType { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DisplayName("门店名称")]
        public string ShopName { get; set; }

        /// <summary>
        /// 信用额度
        /// </summary>
        [DisplayName("信用额度")]
        public decimal? CreditAmt { get; set; }

        /// <summary>
        /// 银行帐号
        /// </summary>
        [DisplayName("银行帐号")]
        public string BankAccount { get; set; }

        /// <summary>
        /// 银行开户名称
        /// </summary>

        [DisplayName("银行开户名称")]
        public string BankAccountName { get; set; }

        /// <summary>
        /// 银行类型(开户行)
        /// </summary>
        [DisplayName("银行类型(开户行)")]
        public string BankType { get; set; }

        /// <summary>
        /// 结帐方式(0:现金 + 数据字典: ShopSettleType =Shop.SettleType)
        /// </summary>
        [DisplayName("结算方式")]
        public string SettleType { get; set; }

        /// <summary>
        /// 结帐方式名称(数据字典: ShopSettleType 对应的Label)
        /// </summary>
        [DisplayName("结帐方式名称")]
        public string SettleName { get; set; }


        /// <summary>
        /// 确认时间
        /// </summary>
        [DisplayName("确认时间")]
        public DateTime? ConfTime { get; set; }

        /// <summary>
        /// 确认人编号
        /// </summary>
        [DisplayName("确认人编号")]
        public int? ConfUserID { get; set; }

        /// <summary>
        /// 确认人名称
        /// </summary>
        [DisplayName("确认人名称")]
        public string ConfUserName { get; set; }

        /// <summary>
        /// 录单时间
        /// </summary>
        [DisplayName("录单时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 录单人
        /// </summary>
        [DisplayName("录单人")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 录单人
        /// </summary>
        [DisplayName("录单人")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DisplayName("修改时间")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [DisplayName("修改人")]
        public int? ModifyUserID { get; set; }

        /// <summary>
        /// 修改人
        /// </summary>
        [DisplayName("修改人")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 过账时间
        /// </summary>
        [DisplayName("过账时间")]
        public DateTime? PostingTime { get; set; }

        /// <summary>
        /// 过账人
        /// </summary>
        [DisplayName("过账人")]
        public int? PostingUserID { get; set; }

        /// <summary>
        /// 过账人
        /// </summary>
        [DisplayName("过账人")]
        public string PostingUserName { get; set; }
    }

    // 摘要:
    //     SaleSettleDetail结算明细
    public class SaleSettleDetail
    {

        // 摘要:
        //     金额(SaleAmt,BackAmt[为负],FeeAmt[为费用金额时可以正负])
        /// <summary>
        /// 金额
        /// </summary>
        [DisplayName("金额")]
        public string BillAmt { get; set; }

        /// <summary>
        /// 平台费
        /// </summary>
        [DisplayName("平台费")]
        public string BillAddAmt { get; set; }

        /// <summary>
        /// 小计金额
        /// </summary>
        [DisplayName("小计金额")]
        public string BillPayAmt { get; set; }

        //
        // 摘要:
        //     单据日期(SaleOrder.OrderDate,SaleBack.BackDate,SaleFee.FeeDate)
        /// <summary>
        /// 单据日期
        /// </summary>
        [DisplayName("单据日期")]
        public DateTime BillDate { get; set; }
        //
        // 摘要:
        //     单据明细ID(SaleFeeDetails.ID 只有费用才有值)
        /// <summary>
        /// 单据明细编号
        /// </summary>
        [DisplayName("单据明细编号")]
        public string BillDetailsID { get; set; }
        //
        // 摘要:
        //     单据ID(SaleOrder.OrderID,SaleBack.backID,SaleFee.FeeID)
        /// <summary>
        /// 单据编号
        /// </summary>
        [DisplayName("单据编号")]
        public string BillID { get; set; }
        //
        // 摘要:
        //     单据类型(0:销售订单;1:销售退货单;2:销售费用单)
        /// <summary>
        ///  单据类型(0:销售订单;1:销售退货单;2:销售费用单)
        /// </summary>
        [DisplayName("单据类型")]
        public int BillType { get; set; }
        //
        // 摘要:
        //     单据类型(0:销售订单;1:销售退货单;2:销售费用单) 
        /// <summary>
        /// 单据类型名称
        /// </summary>
        [DisplayName("单据类型名称")]
        public string BillTypeName { get; set; }
        //
        // 摘要:
        //     费用项目ID(数据字典; SaleFeeCode)
        /// <summary>
        /// 费用项目编号
        /// </summary>
        [DisplayName("费用项目编号")]
        public string FeeCode { get; set; }

        //
        // 摘要:
        //     费用二级项目名称
        /// <summary>
        /// 费用二级项目名称
        /// </summary>
        [DisplayName("费用二级项目名称")]
        public string FeeName { get; set; }
        //
        // 摘要:
        //     主键ID(WID+GUID)
        /// <summary>
        /// 主键编号
        /// </summary>
        [DisplayName("主键编号")]
        public string ID { get; set; }

        // 摘要:
        //     费用原因/退货原因
        /// <summary>
        /// 费用原因/退货原因
        /// </summary>
        [DisplayName("费用原因/退货原因")]
        public string Remark { get; set; }
        //
        // 摘要:
        //     结算ID
        [DisplayName("结算编号")]
        public string SettleID { get; set; }
        //
        // 摘要:
        //     仓库ID(WarehouseID)
        [DisplayName("仓库编号")]
        public int WID { get; set; }
    }

}