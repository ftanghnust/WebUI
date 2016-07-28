using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Platform.Utility.Json;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class BatchRecommendModel
    {
        /// <summary>
        /// 获取EditID
        /// </summary>
        /// <returns></returns>
        public string GetEditID()
        {
            string orderid = string.Empty;
            var ServiceCenter = WorkContext.CreateIDSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.SaleEdit,
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
        public string GetBatchRecommendPageData(BatchRecommendSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateOrderSdkClient();
                var req = AutoMapperHelper.MapTo<Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleEditQueryRequest>(searchModel);
                req.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                req.WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId;
                if (searchModel.SubWID == null)
                {
                    if (WorkContext.CurrentWarehouse.Parent.WarehouseId != WorkContext.CurrentWarehouse.WarehouseId)
                    {
                        req.SubWID = WorkContext.CurrentWarehouse.WarehouseId;
                    }
                }
                else
                {
                    if (searchModel.SubWID != "")
                    {
                        req.SubWID = int.Parse(searchModel.SubWID);
                    }
                }
                req.PageIndex = searchModel.page;
                req.PageSize = searchModel.rows;
                req.SortBy = "EditDate desc ";
                //if (searchModel.CreateTimeEnd.HasValue)
                //{
                //    req.CreateTimeEnd = Convert.ToDateTime(searchModel.CreateTimeEnd).AddDays(1);
                //}
                var resp = ServiceCenter.Execute(req);
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.Total, rows = resp.Data.ItemList };
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
    public class BatchRecommendSearch : BasePageModel
    {
        /// <summary>
        /// 单号
        /// </summary>
        public string EditID { get; set; }

        /// <summary>
        /// 仓库子机构ID
        /// </summary>
        public string SubWID { get; set; }

        /// <summary>
        /// 录单时间 开始
        /// </summary>
        public DateTime? CreateTimeBegin { get; set; }

        /// <summary>
        /// 录单时间 结束
        /// </summary>
        public DateTime? CreateTimeEnd { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 录单人员
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }
}