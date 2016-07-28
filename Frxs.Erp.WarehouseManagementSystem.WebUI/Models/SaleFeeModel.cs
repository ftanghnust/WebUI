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
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class SaleFeeModel : BaseModel
    {

        #region 批量删除数据
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="buyids">主键</param>
        /// <returns>对象</returns>
        public int DeleteSaleFee(string ids, int userId, string userName)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeDelRequest()
            {
                IDs = ids,
                UserId = userId,
                UserName = userName,
                WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
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
        public int SaleFeeChangeStatus(string ids, int status, int userId, string userName)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeChangeStatusRequest()
            {
                FeeIDs = ids,
                Status = status,
                UserId = userId,
                UserName = userName,
                WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                result = resp.Data;
            }
            return result;
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
        public string GetSaleFeePageData(SaleFeeQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateOrderSdkClient();
               
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeGetListRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort,
                    FeeID = !string.IsNullOrEmpty(cpm.FeeID) ? Utils.NoHtml(cpm.FeeID) : null,
                    ShopCode =!string.IsNullOrEmpty(cpm.ShopCode) ? Utils.NoHtml(cpm.ShopCode) : null,
                    ShopName = !string.IsNullOrEmpty(cpm.ShopName) ? Utils.NoHtml(cpm.ShopName) : null,
                   // ConfUserName = conditionDict.ContainsKey("ConfUserName") ? Utils.NoHtml(conditionDict["ConfUserName"].ToString()) : null,
                    Status = cpm.Status.HasValue ? cpm.Status : (int?)null,
                    StartFeeDate = cpm.StartFeeDate.HasValue ? cpm.StartFeeDate : (DateTime?)null,
                    EndFeeDate = cpm.EndFeeDate.HasValue ? cpm.EndFeeDate : (DateTime?)null,
                    WarehouseID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SubWID = cpm.SubWID.HasValue ? cpm.SubWID : (int?)null
                });

                if (resp != null && resp.Flag == 0)
                {
                    IList<FrxsErpOrderSaleFeeGetListResp.SaleFee> itemList = resp.Data.ItemList;
                    foreach (FrxsErpOrderSaleFeeGetListResp.SaleFee salefee in itemList) 
                    {
       
                        salefee.StatusToStr = salefee.Status == 0 ? "录单" : (salefee.Status == 1 ? "确认" : (salefee.Status == 2 ? "过帐" : "结算"));
                    }

                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList ,SubAmt=resp.Data.SubAmt}; 

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

        #region 获取消息数据
        /// <summary>
        /// 获取消息数据
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public SaleFeeOpertion GetSaleFeeData(string ID, int userId, string userName)
        {
            SaleFeeOpertion model = new SaleFeeOpertion();
            try
            {
                //获取
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeGetModelRequest()
                {
                    FeeID = ID,
                    UserId = userId,
                    UserName = userName,
                    WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp != null && resp.Flag == 0)
                {
                    model = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<SaleFeeOpertion>(resp.Data.saleFee);
                    model.StatusToStr = model.Status == "0" ? "录单" : (model.Status == "1" ? "确认" : (model.Status == "2" ? "过帐" : "结算"));
              
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        /// <summary>
        /// 获取消息数据
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public string GetSaleFeeDetailData(string ID, int userId, string userName)
        {
            
            string jsonStr = "[]";
            try
            {
                //获取
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleFeeDetailGetModelRequest()
                {
                    FeeID = ID,
                    UserId = userId,
                    UserName = userName,
                    WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.saleFeePreDetailsList.Count, rows = resp.Data.saleFeePreDetailsList };
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

        /// <summary>
        /// 获取FeeID
        /// </summary>
        /// <returns></returns>
        public string GetFeeID()
        {
            string feeId = string.Empty;
            var ServiceCenter = WorkContext.CreateIDSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.FeeID,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
                
            });

            if (resp != null && resp.Flag == 0)
            {
                feeId = resp.Data;
            }
            return feeId;
        }


        #region 获取门店信息

       
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetShopModelPageData(SaleFeeQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopTableListRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    ShopCode = !string.IsNullOrEmpty(cpm.ShopCode) ? Utils.NoHtml(cpm.ShopCode.Trim()) : null,
                    ShopName = !string.IsNullOrEmpty(cpm.ShopName) ? Utils.NoHtml(cpm.ShopName.Trim()) : null,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString(),
                    SortBy = "ShopCode asc"  

                });
                 

                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.TotalRecords, rows = resp.Data.ItemList };
                    jsonStr = obj.ToJsonString();
                    return jsonStr;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

            return jsonStr;
        }
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public FrxsErpProductShopTableListResp.Shop GetShopModel(string shopCode)
        {
          
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopTableListRequest()
                {
                    PageIndex = 1,
                    PageSize = 1,
                    ShopCode = !string.IsNullOrEmpty(shopCode) ? Utils.NoHtml(shopCode) : null,
                   // ShopName = !string.IsNullOrEmpty(shopCode) ? Utils.NoHtml(shopCode) : null,
                   // WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString(),
                    SortBy = "ShopCode asc"

                });
                if (resp != null && resp.Flag == 0 && resp.Data.ItemList.Count>0 )
                {
                    return resp.Data.ItemList[0];
                }
                else
                {
                    return null;
                }

            }
            catch (Exception ex)
            {
                throw ex;
            }

        }
      


        #endregion


    }


    public class SaleFeeQuery : BasePageModel
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        #region 模型
        /// <summary>
        /// 费用ID(SaleFee.FeeID)
        /// </summary>
        [DataMember]
        [DisplayName("费用ID(SaleFee.FeeID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeID { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? WID { get; set; }

        /// <summary>
        /// 仓库编号(Warehouse.WCode)
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号(Warehouse.WCode)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WCode { get; set; }

        /// <summary>
        /// 仓库名称(Warehouse.WarehouseName)
        /// </summary>
        [DataMember]
        [DisplayName("仓库名称(Warehouse.WarehouseName)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WName { get; set; }

        /// <summary>
        /// 仓库子机构ID
        /// </summary>
        [DataMember]
        [DisplayName("仓库子机构ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? SubWID { get; set; }

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
        /// 状态(0:录单;1:确认;2:过帐;3:结算)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:录单;1:确认;2:过帐;3:结算)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? Status { get; set; }

        /// <summary>
        /// 费用金额(小于0代表销售退回;大于0代表销售增加)(=sum(SaleFeeDetail.FeeAmt)
        /// </summary>
        [DataMember]
        [DisplayName("费用金额(小于0代表销售退回;大于0代表销售增加)(=sum(SaleFeeDetail.FeeAmt)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public double? TotalFeeAmt { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        [DisplayName("提交时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? ConfTime { get; set; }

        /// <summary>
        /// 提交用户ID
        /// </summary>
        [DataMember]
        [DisplayName("提交用户ID")]

        public int? ConfUserID { get; set; }

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

        public int? PostingUserID { get; set; }

        /// <summary>
        /// 过帐用户名称
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户名称")]

        public string PostingUserName { get; set; }

        /// <summary>
        /// 结算完成时间
        /// </summary>
        [DataMember]
        [DisplayName("结算完成时间")]

        public DateTime? SettleTime { get; set; }

        /// <summary>
        /// 结算用户ID
        /// </summary>
        [DataMember]
        [DisplayName("结算用户ID")]

        public int? SettleUserID { get; set; }


        /// <summary>
        /// 结算用户
        /// </summary>
        [DataMember]
        [DisplayName("结算用户")]
        public string SettleUserName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]

        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? CreateUserID { get; set; }

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
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]

        public int? ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称")]

        public string ModifyUserName { get; set; }

        /// <summary>
        /// 费用日期
        /// </summary>
        [DataMember]
        [DisplayName("费用日期")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? FeeDate { get; set; }



        /// <summary>
        /// 费用日期(开始)
        /// </summary>
        [DataMember]
        [DisplayName("费用日期")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? StartFeeDate { get; set; }


        /// <summary>
        /// 费用日期（结束）
        /// </summary>
        [DataMember]
        [DisplayName("费用日期")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? EndFeeDate { get; set; }



        public string ShopCode { get; set; }

        public string ImportGuid { get; set; }

        public string folder { get; set; }



        public string ShopName { get; set; } 
        #endregion

        /// <summary>
        /// 明细列表
        /// </summary>
        [DataMember]
        public IList<FrxsErpOrderSaleFeeGetModelResp.SaleFeeDetails> detailList;  
    }


    public class SaleFeeOpertion : BasePageModel
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        #region 模型
        /// <summary>
        /// 费用ID(SaleFee.FeeID)
        /// </summary>
        [DataMember]
        [DisplayName("费用ID(SaleFee.FeeID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeID { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WID { get; set; }

        /// <summary>
        /// 仓库编号(Warehouse.WCode)
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号(Warehouse.WCode)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WCode { get; set; }

        /// <summary>
        /// 仓库名称(Warehouse.WarehouseName)
        /// </summary>
        [DataMember]
        [DisplayName("仓库名称(Warehouse.WarehouseName)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WName { get; set; }

        /// <summary>
        /// 仓库子机构ID
        /// </summary>
        [DataMember]
        [DisplayName("仓库子机构ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SubWID { get; set; }

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
        /// 状态(0:录单;1:确认;2:过帐;3:结算)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:录单;1:确认;2:过帐;3:结算)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Status { get; set; }



        /// <summary>
        /// 状态(0:录单;1:确认;2:过帐;3:结算)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:录单;1:确认;2:过帐;3:结算)")]

        public string StatusToStr { get; set; }

        /// <summary>
        /// 费用金额(小于0代表销售退回;大于0代表销售增加)(=sum(SaleFeeDetail.FeeAmt)
        /// </summary>
        [DataMember]
        [DisplayName("费用金额(小于0代表销售退回;大于0代表销售增加)(=sum(SaleFeeDetail.FeeAmt)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public double TotalFeeAmt { get; set; }

        /// <summary>
        /// 提交时间
        /// </summary>
        [DataMember]
        [DisplayName("提交时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ConfTime { get; set; }

        /// <summary>
        /// 提交用户ID
        /// </summary>
        [DataMember]
        [DisplayName("提交用户ID")]

        public int? ConfUserID { get; set; }

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
        public string PostingTime { get; set; }

        /// <summary>
        /// 过帐用户ID
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户ID")]

        public int? PostingUserID { get; set; }

        /// <summary>
        /// 过帐用户名称
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户名称")]

        public string PostingUserName { get; set; }

        /// <summary>
        /// 结算完成时间
        /// </summary>
        [DataMember]
        [DisplayName("结算完成时间")]

        public string SettleTime { get; set; }

        /// <summary>
        /// 结算用户ID
        /// </summary>
        [DataMember]
        [DisplayName("结算用户ID")]

        public int? SettleUserID { get; set; }


        /// <summary>
        /// 结算用户
        /// </summary>
        [DataMember]
        [DisplayName("结算用户")]
        public string SettleUserName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]

        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户ID")]
        [Required(ErrorMessage = "{0}不能为空")]
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
        public string ModifyTime { get; set; }

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

        /// <summary>
        /// 费用日期
        /// </summary>
        [DataMember]
        [DisplayName("费用日期")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeDate { get; set; }


        #endregion  
        
        /// <summary>
        /// 明细列表
        /// </summary>
        [DataMember]
        public IList<FrxsErpOrderSaleFeeGetModelResp.SaleFeeDetails> detailList;  
    }

    /// <summary>
    /// 门店费用
    /// </summary>
    public class SaleFeeDetailsOpertion : BasePageModel
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }


        #region 模型
        /// <summary>
        /// 主键ID(GUID)
        /// </summary>
        [DataMember]
        [DisplayName("主键ID(GUID)")]
        [ExcelNoExport]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ID { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string WID { get; set; }

        /// <summary>
        /// 费用ID(SaleFee.FeeID)
        /// </summary>
        [DataMember]
        [DisplayName("费用ID(SaleFee.FeeID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string FeeID { get; set; }

        /// <summary>
        /// 结算ID
        /// </summary>
        [DataMember]
        [DisplayName("结算ID")]
        public string SettleID { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [DataMember]
        [DisplayName("门店ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopID { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember]
        [DisplayName("门店编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMember]
        [DisplayName("门店名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopName { get; set; }

        /// <summary>
        /// 费用项目ID(数据字典; SaleFeeCode)
        /// </summary>
        [DataMember]
        [DisplayName("费用项目ID(数据字典; SaleFeeCode)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeCode { get; set; }

        /// <summary>
        /// 费用项目名称
        /// </summary>
        [DataMember]
        [DisplayName("费用项目名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeName { get; set; }

        /// <summary>
        /// 费用原因
        /// </summary>
        [DataMember]
        [DisplayName("费用原因")]

        public string Reason { get; set; }

        /// <summary>
        /// 费用订单编号(SaleOrders.OrderId)
        /// </summary>
        [DataMember]
        [DisplayName("费用订单编号(SaleOrders.OrderId)")]

        public string OrderId { get; set; }

        /// <summary>
        /// 费用金额(小于0代表销售退回;大于0代表销售增加)
        /// </summary>
        [DataMember]
        [DisplayName("费用金额(小于0代表销售退回;大于0代表销售增加)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public double FeeAmt { get; set; }

        /// <summary>
        /// 结算完成时间
        /// </summary>
        [DataMember]
        [DisplayName("结算完成时间")]
        public string SettleTime { get; set; }

        /// <summary>
        /// 序号(输入的序号,每一个单据从1开始)
        /// </summary>
        [DataMember]
        [DisplayName("序号(输入的序号,每一个单据从1开始)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string SerialNumber { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]
        [ExcelNoExport]
        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称")]
        [ExcelNoExport]
        public string ModifyUserName { get; set; }

        #endregion

        /// <summary>
        /// 明细列表
        /// </summary>
        [DataMember]
        public IList<FrxsErpOrderSaleFeeGetModelResp.SaleFeeDetails> detailList;
    }

    /// <summary>
    /// 门店费用明细导出
    /// </summary>
    public class SaleFeeDetailsExcel : BaseModel
    {


        #region 模型
        /// <summary>
        /// 主键ID(GUID)
        /// </summary>
        [DataMember]
        [DisplayName("主键ID(GUID)")]
        [ExcelNoExport]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ID { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string WID { get; set; }


        /// <summary>
        /// 费用ID(SaleFee.FeeID)
        /// </summary>
        [DataMember]
        [DisplayName("费用ID(SaleFee.FeeID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string FeeID { get; set; }

        /// <summary>
        /// 结算ID
        /// </summary>
        [DataMember]
        [DisplayName("结算ID")]
        [ExcelNoExport]
        public string SettleID { get; set; }

        /// <summary>
        /// 门店ID
        /// </summary>
        [DataMember]
        [DisplayName("门店ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ShopID { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DataMember]
        [DisplayName("门店编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DataMember]
        [DisplayName("门店名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShopName { get; set; }

        /// <summary>
        /// 费用项目ID(数据字典; SaleFeeCode)
        /// </summary>
        [DataMember]
        [DisplayName("费用项目ID(数据字典; SaleFeeCode)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeCode { get; set; }

        /// <summary>
        /// 费用项目名称
        /// </summary>
        [DataMember]
        [DisplayName("费用项目名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeName { get; set; }

        /// <summary>
        /// 费用原因
        /// </summary>
        [DataMember]
        [DisplayName("备注")]
        public string Reason { get; set; }

        /// <summary>
        /// 费用订单编号(SaleOrders.OrderId)
        /// </summary>
        [DataMember]
        [DisplayName("费用订单编号(SaleOrders.OrderId)")]
        public string OrderId { get; set; }

        /// <summary>
        /// 费用金额(小于0代表销售退回;大于0代表销售增加)
        /// </summary>
        [DataMember]
        [DisplayName("费用金额(小于0代表销售退回;大于0代表销售增加)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string FeeAmt { get; set; }

        /// <summary>
        /// 结算完成时间
        /// </summary>
        [DataMember]
        [DisplayName("结算完成时间")]
        [ExcelNoExport]
        public string SettleTime { get; set; }

        /// <summary>
        /// 序号(输入的序号,每一个单据从1开始)
        /// </summary>
        //2016-6-21 测试组bug单提出 该列不需要 经过和舒姐确认，该列可以不要
        //[DataMember]
        //[DisplayName("序号(输入的序号,每一个单据从1开始)")]
        //[Required(ErrorMessage = "{0}不能为空")]
        //public string SerialNumber { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]
        [ExcelNoExport]
        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户")]
        public string ModifyUserName { get; set; }

        #endregion

    }
}