
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Web;
/*****************************
* Author:Tang.Fan
*
* Date:2016-03-24
******************************/
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Serialization;
using Frxs.Platform.Utility.Json;


namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// SaleBackPre实体类
    /// </summary>
    [Serializable]
    [DataContract]
    public partial class SaleBackPreModel : BaseModel
    {

        #region 模型
        /// <summary>
        /// 退货单编号
        /// </summary>
        [DataMember]
        [DisplayName("退货单编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BackID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WID { get; set; }

        /// <summary>
        /// 仓库编号(Warehouse.WCode)
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号(Warehouse.WCode)")]

        public string WCode { get; set; }

        /// <summary>
        /// 仓库名称(Warehouse.WName)
        /// </summary>
        [DataMember]
        [DisplayName("仓库名称(Warehouse.WName)")]
        public string WName { get; set; }

        /// <summary>
        /// 仓库柜台ID
        /// </summary>
        [DataMember]
        [DisplayName("仓库柜台ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int SubWID { get; set; }

        /// <summary>
        /// 仓库柜台编号(Warehouse.WCode)
        /// </summary>
        [DataMember]
        [DisplayName("仓库柜台编号(Warehouse.WCode)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubWCode { get; set; }

        /// <summary>
        /// 仓库柜台名称(Warehouse.WName)
        /// </summary>
        [DataMember]
        [DisplayName("仓库柜台名称(Warehouse.WName)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubWName { get; set; }

        /// <summary>
        /// 退货日期(格式:yyyy-MM-dd)
        /// </summary>
        [DataMember]
        [DisplayName("退货日期(格式:yyyy-MM-dd)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime BackDate { get; set; }

        /// <summary>
        /// 兴盛用户ID(预留)
        /// </summary>
        [DataMember]
        [DisplayName("兴盛用户ID(预留)")]
        public long XSUserID { get; set; }

        /// <summary>
        /// 下单门店ID
        /// </summary>
        [DataMember]
        [DisplayName("下单门店ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ShopID { get; set; }

        /// <summary>
        /// 下单门店编号
        /// </summary>
        [DataMember]
        [DisplayName("下单门店编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 下单门店名称
        /// </summary>
        [DataMember]
        [DisplayName("下单门店名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopName { get; set; }

        /// <summary>
        /// 状态(0:未提交;1:已确认;2:已过帐;3:已结算)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:未提交;1:已确认;2:已过帐;3:已结算)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Status { get; set; }

        /// <summary>
        /// 退货金额(SaleBackDetails.BackAmt)
        /// </summary>
        [DataMember]
        [DisplayName("退货金额(SaleBackDetails.BackAmt)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public double TotalBackAmt { get; set; }

        /// <summary>
        /// 积分(SaleBackDetails.SubPoint)
        /// </summary>
        [DataMember]
        [DisplayName("积分(SaleBackDetails.SubPoint)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public double TotalPoint { get; set; }


        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public decimal TotalAddAmt { get; set; }


        /// <summary>
        ///
        /// </summary>
        [DataMember]
        public decimal PayAmount { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        [DisplayName("提交时间")]
        public DateTime? ConfTime { get; set; }

        /// <summary>
        /// 提交用户ID
        /// </summary>
        [DataMember]
        [DisplayName("提交用户ID")]
        public int ConfUserID { get; set; }

        /// <summary>
        /// 提交用户名称
        /// </summary>
        [DataMember]
        [DisplayName("提交用户名称")]
        public string ConfUserName { get; set; }

        /// <summary>
        /// 过帐时间
        /// </summary>
        [DataMember]
        [DisplayName("过帐时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? PostingTime { get; set; }

        /// <summary>
        /// 过帐用户ID
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户ID")]
        public int PostingUserID { get; set; }

        /// <summary>
        /// 过帐用户名称
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户名称")]
        public string PostingUserName { get; set; }

        /// <summary>
        /// 结算时间
        /// </summary>
        [DataMember]
        [DisplayName("结算时间")]
        public DateTime? SettleTime { get; set; }

        /// <summary>
        /// 结算用户ID
        /// </summary>
        [DataMember]
        [DisplayName("结算用户ID")]
        public int SettleUserID { get; set; }

        /// <summary>
        /// 结算用户名称
        /// </summary>
        [DataMember]
        [DisplayName("结算用户名称")]
        public string SettleUserName { get; set; }

        /// <summary>
        /// 结算ID(SaleSettle.SettleID)
        /// </summary>
        [DataMember]
        [DisplayName("结算ID(SaleSettle.SettleID)")]
        public string SettleID { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>
        [DataMember]
        [DisplayName("创建日期")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户ID")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]
        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称")]
        public string ModifyUserName { get; set; }

        #endregion
    }

    public partial class SaleBackPreModel : BaseModel
    {

        /// <summary>
        /// 销售退货详情
        /// </summary>
        public IList<SaleBackPreDetailsModel> Backdetails { get; set; }


        /// <summary>
        /// 获取SaleBackID
        /// </summary>
        /// <returns></returns>
        public string GetSaleBackID()
        {
            string buybackid = string.Empty;
            var ServiceCenter = WorkContext.CreateIDSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.SaleBack,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            if (resp != null && resp.Flag == 0)
            {
                buybackid = resp.Data;
            }
            return buybackid;
        }


        #region 获取分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="searchModel"></param>
        /// <returns></returns>
        public string GetSaleBackPrePageData(SaleBackSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateOrderSdkClient();
                var req = AutoMapperHelper.MapTo<Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreGetListRequest>(searchModel);
                req.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                if (!searchModel.SubWID.HasValue)
                {
                    if (WorkContext.CurrentWarehouse.Parent.WarehouseId != WorkContext.CurrentWarehouse.WarehouseId)
                    {
                        req.SubWID = WorkContext.CurrentWarehouse.WarehouseId;
                    }
                }
                req.PageIndex = searchModel.page;
                req.PageSize = searchModel.rows;
                req.SortBy = "BackDate desc ";
                //if (!string.IsNullOrEmpty(searchModel.OrderDateEnd))
                //{
                //    req.OrderDateEnd = req.OrderDateEnd + " 23:59:59";
                //}
                var resp = ServiceCenter.Execute(req);
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
                    jsonStr = obj.ToJsonString();

                    if (resp.Data.ItemList.Count > 0)
                    {
                        IList<QueryHistory> temp = new List<QueryHistory>();
                        int count = 0;
                        foreach (var item in resp.Data.ItemList)
                        {
                            if (count >= 10)
                            {
                                break;
                            }
                            count = count + 1;
                            QueryHistory queryhistory = new QueryHistory();
                            queryhistory.ID = item.BackID;
                            queryhistory.VendorCode = item.ShopCode;
                            queryhistory.VendorName = item.ShopName;
                            temp.Add(queryhistory);
                        }
                        QueryHistory.Clear("SaleBack");
                        QueryHistory.SetQueryHistory("SaleBack", temp.ToJsonString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
        #endregion


        #region 批量删除数据
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="backids">主键</param>
        /// <returns></returns>
        public int DeleteSaleBackPre(string backids)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreDelRequest()
            {
                BackIDs = backids,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                result = resp.Data;
            }
            return result;
        }
        #endregion


        #region 批量状态改变
        /// <summary>
        /// 批量状态改变
        /// </summary>
        /// <param name="backids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int SaleBackPreChangeStatus(string backids, int status)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreChangeStatusRequest()
            {
                BackIDs = backids,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Status = status,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                result = resp.Data;
            }
            return result;
        }
        #endregion

    }

    /// <summary>
    /// 查询模型
    /// </summary>
    public class SaleBackSearch : BasePageModel
    {
        /// <summary>
        /// 销售退货单编号
        /// </summary>
        public string BackID { get; set; }

        /// <summary>
        /// 仓库子机构ID
        /// </summary>
        public int? SubWID { get; set; }

        /// <summary>
        /// 门店Code
        /// </summary>
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 状态(0:未提交;1:已提交;2:已过帐;3:已结算)
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 退货时间 开始
        /// </summary>
        public string OrderDateBegin { get; set; }

        /// <summary>
        /// 退货时间 结束
        /// </summary>
        public string OrderDateEnd { get; set; }
    }
}