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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class WStationNumberModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]        
        public int ID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID)")]        
        public int WID { get; set; }

        /// <summary>
        /// 待装区编号(同一个仓库不能重复)
        /// </summary>
        [DataMember]
        [DisplayName("待装区编号(同一个仓库不能重复)")]        
        public int StationNumber { get; set; }

        /// <summary>
        /// 状态(0:空闲;1:正在使用;2:冻结; 可以物理删除)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:空闲;1:正在使用;2:冻结; 可以物理删除)")]        
        public int Status { get; set; }

        /// <summary>
        /// 门店ID(status=1时才有值)
        /// </summary>
        [DataMember]
        [DisplayName("门店ID(status=1时才有值)")]
        public int ShopID { get; set; }

        /// <summary>
        /// 订单编号(status=1时才有值)
        /// </summary>
        [DataMember]
        [DisplayName("订单编号(status=1时才有值)")]
        public string OrderID { get; set; }

        /// <summary>
        /// 配送日期(status=1时才有值 填入值为 SaleOrder.ConfDate)
        /// </summary>
        [DataMember]
        [DisplayName("配送日期(status=1时才有值 填入值为 SaleOrder.ConfDate)")]
        public DateTime OrderConfDate { get; set; }

        /// <summary>
        /// 所属路线(status=1时才有值)
        /// </summary>
        [DataMember]
        [DisplayName("所属路线(status=1时才有值)")]
        public int LineID { get; set; }

        /// <summary>
        /// 订单状态(status=1时才有值; 值：3:正在拣货;4:拣货完成;5:打印完成;6:正在配送中)
        /// </summary>
        [DataMember]
        [DisplayName("订单状态(status=1时才有值; 值：3:正在拣货;4:拣货完成;5:打印完成;6:正在配送中)")]
        public int OrderStatus { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember]
        [DisplayName("门店编号")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMember]
        [DisplayName("门店名称")]
        public string ShopName { get; set; }

        /// <summary>
        /// 配送线路
        /// </summary>
        [DataMember]
        [DisplayName("配送线路")]
        public string LineName { get; set; }

        /// <summary>
        /// 配送员
        /// </summary>
        [DataMember]
        [DisplayName("配送员")]
        public string EmpName { get; set; }


        /// <summary>
        /// 开始编号
        /// </summary>        
        [DisplayName("开始编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int StartID { get; set; }

        /// <summary>
        /// 结束编号
        /// </summary>        
        [DisplayName("结束编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int EndID { get; set; }

        #endregion


        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public int DeleteWStationNumber(string ids)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWStationNumberDelRequest()
            {
                ID = StringExtension.ToIntArray(ids, ',').ToList()                 
            });

            return resp.Data;
        }
        #endregion

        #region 获取分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetWStationNumberPageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWStationNumberTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    ShopCode = conditionDict.ContainsKey("ShopCode") ? Utils.NoHtml(conditionDict["ShopCode"].ToString()) : null,
                    ShopName = conditionDict.ContainsKey("ShopName") ? Utils.NoHtml(conditionDict["ShopName"].ToString()) : null,
                    StationNumber = conditionDict.ContainsKey("StationNumber") ? Utils.NoHtml(conditionDict["StationNumber"].ToString()) : null,
                    OrderStatus = conditionDict.ContainsKey("OrderStatus") ? Utils.NoHtml(conditionDict["OrderStatus"].ToString()) : null,
                    Status = conditionDict.ContainsKey("Status") ? Utils.NoHtml(conditionDict["Status"].ToString()) : null,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
                    jsonStr = obj.ToJsonString();
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
        #endregion



        #region 冻结数据
        /// <summary>
        /// 冻结数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public int FrozenWStationNumber(string ids, int frozen)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWStationNumberIsFrozenRequest()
            {
                ID = StringExtension.ToIntArray(ids, ',').ToList(),
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName,
                Status = frozen

            });

            return resp.Data;
        }
        #endregion
    }
}