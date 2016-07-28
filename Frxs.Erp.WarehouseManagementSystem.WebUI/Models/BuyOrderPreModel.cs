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
using System.Collections;
using Frxs.Platform.Utility.Filter;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// BuyOrderPre实体类
    /// </summary>
    [Serializable]
    public partial class BuyOrderPreModel : BaseModel
    {

        #region 模型
        /// <summary>
        /// 采购单编号
        /// </summary>
        [DataMember]
        [DisplayName("收货单号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BuyID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WID { get; set; }

        /// <summary>
        /// 仓库柜台
        /// </summary>
        [DataMember]
        [DisplayName("仓库柜台")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int SubWID { get; set; }

        /// <summary>
        /// 预定订单编号(预留)
        /// </summary>
        [DataMember]
        [DisplayName("预定订单编号(预留)")]
        public string PreBuyID { get; set; }

        /// <summary>
        /// 采购入库时间(OrderStatus=1;格式:yyyy-MM-dd HH:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("收货时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime OrderDate { get; set; }

        /// <summary>
        /// 仓库编号(Warehouse.WCode)
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号(Warehouse.WCode)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WCode { get; set; }

        /// <summary>
        /// 仓库名称(Warehouse.WName)
        /// </summary>
        [DataMember]
        [DisplayName("仓库")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WName { get; set; }

        /// <summary>
        /// 仓库子机构编号(Warehouse.WCode)
        /// </summary>
        [DataMember]
        [DisplayName("仓库子机构编号(Warehouse.WCode)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubWCode { get; set; }

        /// <summary>
        /// 仓库子机构名称(Warehouse.WName)
        /// </summary>
        [DataMember]
        [DisplayName("仓库子机构名称(Warehouse.WName)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubWName { get; set; }

        /// <summary>
        /// 采购员ID(WarehouseEmp.EmpID and UserType=4)
        /// </summary>
        [DataMember]
        [DisplayName("采购员ID(WarehouseEmp.EmpID and UserType=4)")]
        public int BuyEmpID { get; set; }

        /// <summary>
        /// 采购员名称(WarehouseEmp.EmpName and UserType=4)
        /// </summary>
        [DataMember]
        [DisplayName("采购员")]
        public string BuyEmpName { get; set; }

        /// <summary>
        /// 供应商分类ID
        /// </summary>
        [DataMember]
        [DisplayName("供应商分类ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int VendorID { get; set; }

        /// <summary>
        /// 供应商编号
        /// </summary>
        [DataMember]
        [DisplayName("供应商编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string VendorCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        [DataMember]
        [DisplayName("供应商")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string VendorName { get; set; }

        /// <summary>
        /// 状态(0:未提交;1:已提交;2:已过帐;3:已结算)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Status { get; set; }

        /// <summary>
        /// 采购总金额(BuyOrderDetails.SubBuyAmt)
        /// </summary>
        [DataMember]
        [DisplayName("采购总金额")]
        [Required(ErrorMessage = "{0}不能为空")]
        public double TotalOrderAmt { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        [DisplayName("确认时间")]
        [Required(ErrorMessage = "{0}不能为空")]
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
        [DisplayName("确认人员")]
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
        [DisplayName("过帐人员")]
        public string PostingUserName { get; set; }


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
        [DisplayName("录单时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户ID")]
        [ExcelNoExport]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("录单人员")]
        public string CreateUserName { get; set; }
        #endregion
    }

    public partial class BuyOrderPreModel : BaseModel
    {

        /// <summary>
        /// 
        /// </summary>
        public IList<BuyOrderPreDetailsModel> Orderdetails { get; set; }


        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string StatusStr { get; set; }


        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        [DisplayName("仓库")]
        public string WNameStr { get; set; }


        /// <summary>
        /// 商品编号
        /// </summary>
        [DataMember]
        [DisplayName("商品编号")]
        public int ProductId { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        [DisplayName("商品名称")]
        public string ProductName { get; set; }


        /// <summary>
        /// 供应商名称或编码
        /// </summary>
        [DataMember]
        [DisplayName("供应商")]
        public string VendorCodeOrName { get; set; }

        /// <summary>
        /// 标志位
        /// </summary>
        [DataMember]
        [DisplayName("标志位")]
        public string Flag { get; set; }


        /// <summary>
        /// 仓库集合
        /// </summary>
        public SelectList WCList { get; set; }

        public void BindWCList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                ParentWID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                IsFreeze = 0,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            if (resp != null && resp.Flag == 0)
            {
                WCList = new SelectList(resp.Data.ItemList, "WID", "WName", true);
            }
            else
            {
                WCList = null;
            }
        }


        #region 获取分页数据

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="searchModel"> </param>
        /// <returns>json格式字符串</returns>
        public string GetBuyOrderPrePageData(BuyOrderSearch searchModel)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var req = AutoMapperHelper.MapTo<Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyOrderPreGetListRequest>(searchModel);
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
                req.SortBy = "OrderDate desc ";
                //if (!string.IsNullOrEmpty(searchModel.OrderDateEnd))
                //{
                //    req.OrderDateEnd = req.OrderDateEnd + " 23:59:59";
                //}
                var resp = serviceCenter.Execute(req);
                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList, SubAmt = resp.Data.SubAmt };
                    jsonStr = obj.ToJsonString();

                    if (resp.Data.ItemList.Count > 0)
                    {
                        int count = 0;
                        IList<QueryHistory> temp = new List<QueryHistory>();
                        foreach (var item in resp.Data.ItemList)
                        {
                            if (count >= 10)
                            {
                                break;
                            }
                            count = count + 1;
                            QueryHistory queryhistory = new QueryHistory();
                            queryhistory.ID = item.BuyID;
                            queryhistory.VendorCode = item.VendorCode;
                            queryhistory.VendorName = item.VendorName;
                            temp.Add(queryhistory);
                        }
                        QueryHistory.Clear("BuyOrder");
                        QueryHistory.SetQueryHistory("BuyOrder", temp.ToJsonString());
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
        /// <param name="buyids">主键</param>
        /// <returns>对象</returns>
        public int DeleteBuyOrderPre(string buyids)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyOrderPreDelRequest()
            {
                BuyIDs = buyids,
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
        /// 
        /// </summary>
        /// <param name="buyids"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public int BuyOrderPreChangeStatus(string buyids, int status)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyOrderPreChangeStatusRequest()
            {
                BuyIDs = buyids,
                Status = status,
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


        /// <summary>
        /// 获取BuyOrderID
        /// </summary>
        /// <returns></returns>
        public string GetBuyOrderID()
        {
            string buyorderid = string.Empty;
            var ServiceCenter = WorkContext.CreateIDSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.BuyOrder,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            if (resp != null && resp.Flag == 0)
            {
                buyorderid = resp.Data;
            }
            return buyorderid;
        }

    }


    /// <summary>
    /// 查询模型
    /// </summary>
    public class BuyOrderSearch : BasePageModel
    {
        #region 模型
        /// <summary>
        /// 采购单编号
        /// </summary>
        public string BuyID { get; set; }

        /// <summary>
        /// 供应商Code或者供应商名称
        /// </summary>
        public string VendorCodeOrName { get; set; }

        /// <summary>
        /// 仓库子机构ID
        /// </summary>
        public int? SubWID { get; set; }

        /// <summary>
        /// 商品编号(Prouct.ProductID)
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// 描述商品名称(Product.ProductName)
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 状态(0:未提交;1:已提交;2:已过帐;3:已结算)
        /// </summary>
        public int? Status { get; set; }

        /// <summary>
        /// 收货时间 开始
        /// </summary>
        public string OrderDateBegin { get; set; }

        /// <summary>
        /// 收货时间 结束
        /// </summary>
        public string OrderDateEnd { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        #endregion
    }
}