using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Json;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class PromotionOrderModel
    {


        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetPromotionOrderPageData(PromotionOrderSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreatePromotionSdkClient();
                var req = AutoMapperHelper.MapTo<Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionOrderShopQueryRequest>(searchModel);
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
                req.PageIndex = searchModel.page;
                req.PageSize = searchModel.rows;
                req.SortBy = "OrderDate desc ";
                //if (searchModel.OrderDateEnd.HasValue)
                //{
                //    req.OrderDateEnd = Convert.ToDateTime(searchModel.OrderDateEnd).AddDays(1);
                //}
                if (searchModel.SendDateEnd.HasValue)
                {
                    req.SendDateEnd = Convert.ToDateTime(searchModel.SendDateEnd).AddDays(1);
                }
                var resp = ServiceCenter.Execute(req);
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalCount, rows = resp.Data.Orders };
                    jsonStr = obj.ToJsonString();
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
    public class PromotionOrderSearch : BasePageModel
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
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 门店类型
        /// </summary>
        public int? ShopType { get; set; }

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
    }
}