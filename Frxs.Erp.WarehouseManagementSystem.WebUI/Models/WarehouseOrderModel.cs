using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Json;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class WarehouseOrderModel
    {
        /// <summary>
        /// 获取OrderID
        /// </summary>
        /// <returns></returns>
        public string GetOrderID()
        {
            string orderid = string.Empty;
            var ServiceCenter = WorkContext.CreateIDSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.SaleOrder,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            if (resp != null && resp.Flag == 0)
            {
                orderid = resp.Data;
            }
            return orderid;
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetWarehouseOrderPageData(WarehouseOrderSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateOrderSdkClient();
                var req = AutoMapperHelper.MapTo<Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrdervSaleOrderQueryRequest>(searchModel);
                req.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                req.WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                if (searchModel.SubWID == null)
                {
                    if (WorkContext.CurrentWarehouse.Parent.WarehouseId != WorkContext.CurrentWarehouse.WarehouseId)
                    {
                        req.SubID = WorkContext.CurrentWarehouse.WarehouseId;
                    }
                }
                else
                {
                    if (searchModel.SubWID != "")
                    {
                        req.SubID = int.Parse(searchModel.SubWID);
                    }
                }
                if (searchModel.rows == 0)
                {
                    req.PageIndex = 1;
                    req.PageSize = int.MaxValue;
                }
                else
                {
                    req.PageIndex = searchModel.page;
                    req.PageSize = searchModel.rows;
                }
                req.SortBy = "OrderDate desc ";
                //if (searchModel.OrderDateEnd.HasValue)
                //{
                //    req.OrderDateEnd = Convert.ToDateTime(searchModel.OrderDateEnd).AddDays(1);
                //}
                if (searchModel.SendDateEnd.HasValue)
                {
                    req.SendDateEnd = Convert.ToDateTime(searchModel.SendDateEnd).AddDays(1);
                }
                //if (searchModel.ShippingBeginDateEnd.HasValue)
                //{
                //    req.ShippingBeginDateEnd = Convert.ToDateTime(searchModel.ShippingBeginDateEnd).AddDays(1);
                //}
                //if (searchModel.ConfDateEnd.HasValue)
                //{
                //    req.ConfDateEnd = Convert.ToDateTime(searchModel.ConfDateEnd).AddDays(1);
                //}
                var resp = ServiceCenter.Execute(req);
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalCount, rows = resp.Data.Orders, SubAmt = resp.Data.TotalAmt };
                    jsonStr = obj.ToJsonString();

                    if (resp.Data.Orders.Count > 0)
                    {
                        int count = 0;
                        IList<QueryHistory> temp = new List<QueryHistory>();
                        foreach (var item in resp.Data.Orders)
                        {
                            if (count >= 10)
                            {
                                break;
                            }
                            count = count + 1;
                            QueryHistory queryhistory = new QueryHistory();
                            queryhistory.ID = item.OrderId;
                            queryhistory.VendorCode = item.ShopCode;
                            queryhistory.VendorName = item.ShopName;
                            temp.Add(queryhistory);
                        }
                        QueryHistory.Clear("WarehouseOrder");
                        QueryHistory.SetQueryHistory("WarehouseOrder", temp.ToJsonString());
                    }
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
    }

    /// <summary>
    /// 查询模型
    /// </summary>
    public class WarehouseOrderSearch : BasePageModel
    {
        /// <summary>
        /// 订单编号
        /// </summary>
        public string OrderId { get; set; }

        /// <summary>
        /// 仓库子机构ID
        /// </summary>
        public string SubWID { get; set; }

        /// <summary>
        /// 门店编码
        /// </summary>
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        public string ShopName { get; set; }

        /// <summary>
        /// 配送线路
        /// </summary>
        public int? LineID { get; set; }

        /// <summary>
        /// 预计配送日期 开始
        /// </summary>
        public DateTime? SendDateBegin { get; set; }

        /// <summary>
        /// 预计配送日期 结束
        /// </summary>
        public DateTime? SendDateEnd { get; set; }

        /// <summary>
        /// 下单时间 开始
        /// </summary>
        public DateTime? OrderDateBegin { get; set; }

        /// <summary>
        /// 下单时间 结束
        /// </summary>
        public DateTime? OrderDateEnd { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 待装区编号
        /// </summary>
        public int? StationNumber { get; set; }

        /// <summary>
        /// 订单来源
        /// </summary>
        public int? OrderType { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        public string SKU { get; set; }


        ///// <summary>
        ///// 配送时间 开始
        ///// </summary>
        //public DateTime? ShippingBeginDateBegin { get; set; }

        ///// <summary>
        ///// 配送时间 结束
        ///// </summary>
        //public DateTime? ShippingBeginDateEnd { get; set; }

        /// <summary>
        /// 确认时间 开始
        /// </summary>
        public DateTime? ConfDateBegin { get; set; }

        /// <summary>
        /// 确认时间 结束
        /// </summary>
        public DateTime? ConfDateEnd { get; set; }

        /// <summary>
        /// 门店类型
        /// </summary>
        public int? ShopType { get; set; }


    }
}