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
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{


    public class WProductAdjShelfModel : BaseModel
    {

        #region 批量删除数据
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="buyids">主键</param>
        /// <returns>对象</returns>
        public int DeleteWProductAdjShelf(string ids, int userId, string userName)
        {
            var ServiceCenter = WorkContext.CreateProductSdkClient();
            var idsAry = ids.Split(',');
            IList<string> idsList = new List<string>();
            foreach (var id in idsAry)
            {
                idsList.Add(id);
            }
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfDelRequest()
            {
                AdjIDs = idsList,
                UserId = userId,
                UserName = userName
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
        public int WProductAdjShelfChangeStatus(string ids, int status, int userId, string userName)
        {
            var ServiceCenter = WorkContext.CreateProductSdkClient();

            var idsAry = ids.Split(',');
            IList<string> idsList = new List<string>();
            foreach (var id in idsAry)
            {
                idsList.Add(id);
            }
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfChangeStatusRequest()
            {
                AdjIDs = idsList,
                Status = status,
                UserId = userId,
                UserName = userName
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
        public string GetWProductAdjShelfPageData(WProductAdjShelfQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateProductSdkClient();

                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfGetListRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = " AdjID Desc",
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AdjID = !string.IsNullOrEmpty(cpm.AdjID) ? Utils.NoHtml(cpm.AdjID.Trim()) : null,
                    Status = !string.IsNullOrEmpty(cpm.Status) ? int.Parse(cpm.Status) : (int?)null,
                    StartDate = !string.IsNullOrEmpty(cpm.StartDate) ? DateTime.Parse(cpm.StartDate) : (DateTime?)null,
                    EndDate = !string.IsNullOrEmpty(cpm.EndDate) ? DateTime.Parse(cpm.EndDate) : (DateTime?)null

                });

                if (resp != null && resp.Flag == 0)
                {
                    IList<FrxsErpProductWProductAdjShelfGetListResp.WProductAdjShelf> itemList = resp.Data.ItemList;
                    foreach (FrxsErpProductWProductAdjShelfGetListResp.WProductAdjShelf WProductAdjShelf in itemList)
                    {

                        WProductAdjShelf.StatusToStr = WProductAdjShelf.Status == 0 ? "录单" : (WProductAdjShelf.Status == 1 ? "确认" : (WProductAdjShelf.Status == 2 ? "过帐" : "其他"));
                    }
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

        #region 获取消息数据
        /// <summary>
        /// 获取消息数据
        /// </summary>
        /// <param name="ID"></param>
        /// <param name="userId"></param>
        /// <param name="userName"></param>
        /// <returns></returns>
        public WProductAdjShelfOpertion GetWProductAdjShelfData(string ID, int userId, string userName)
        {
            WProductAdjShelfOpertion model = new WProductAdjShelfOpertion();
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfGetModelRequest()
                {
                    AdjId = ID,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    UserId = userId,
                    UserName = userName
                });

                if (resp != null && resp.Flag == 0)
                {
                    model = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<WProductAdjShelfOpertion>(resp.Data.wProductAdjShelf);
                    model.StatusToStr = model.Status == "0" ? "录单" : (model.Status == "1" ? "确认" : (model.Status == "2" ? "过帐" : "其他"));

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
        public string GetWProductAdjShelfDetailData(string ID, int userId, string userName)
        {
            string jsonStr = "[]";
            try
            {
                //获取
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjShelfGetModelRequest()
                {
                    AdjId = ID,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    UserId = userId,
                    UserName = userName
                });


                if (resp != null && resp.Flag == 0)
                {
                    var obj = new { total = resp.Data.wProductAdjShelfDetailsList.Count, rows = resp.Data.wProductAdjShelfDetailsList };
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
        /// 获取
        /// </summary>
        /// <returns></returns>
        public string GetShelfAdjustID()
        {
            string feeId = string.Empty;
            var ServiceCenter = WorkContext.CreateIDSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.ShelfAdjust,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            if (resp != null && resp.Flag == 0)
            {
                feeId = resp.Data;
            }
            return feeId;
        }


    }


    public class WProductAdjShelfQuery : BasePageModel
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }
        /// <summary>
        /// 路径
        /// </summary>
        public string folder { get; set; }

        /// <summary>
        /// 路径
        /// </summary>
        public string ImportGuid { get; set; }


        #region 模型
        /// <summary>
        /// 调整ID
        /// </summary>
        [DataMember]
        [DisplayName("调整单据号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string AdjID { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WID { get; set; }

        /// <summary>
        /// 状态(0:未提交;1:已确认;2:已过帐;)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:未提交;1:已确认;2:已过帐;)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Status { get; set; }



        /// <summary>
        /// 调整类型(0:货架[固定])
        /// </summary>
        [DataMember]
        [DisplayName("调整类型(0:货架[固定])")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string AdjType { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string StartDate { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string EndDate { get; set; }


        #endregion


        /// <summary>
        /// 明细列表
        /// </summary>
        public IList<FrxsErpProductWProductAdjShelfGetModelResp.WProductAdjShelfDetails> wProductAdjShelfDetailsList { get; set; }
    }


    public class WProductAdjShelfOpertion : BasePageModel
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }


        #region 模型
        /// <summary>
        /// 调整ID
        /// </summary>
        [DataMember]
        [DisplayName("调整单据号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string AdjID { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WID { get; set; }

        /// <summary>
        /// 状态(0:未提交;1:已确认;2:已过帐;)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:未提交;1:已确认;2:已过帐;)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Status { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        [DataMember]
        [DisplayName("确认时间")]
        public string ConfTime { get; set; }

        /// <summary>
        /// 确认用户ID
        /// </summary>
        [DataMember]
        [DisplayName("确认用户ID")]

        public string ConfUserID { get; set; }

        /// <summary>
        /// 确认用户名称
        /// </summary>
        [DataMember]
        [DisplayName("确认用户名称")]
        public string ConfUserName { get; set; }

        /// <summary>
        /// 过帐时间
        /// </summary>
        [DataMember]
        [DisplayName("过帐时间")]

        public string PostingTime { get; set; }

        /// <summary>
        /// 过帐用户ID
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户ID")]

        public string PostingUserID { get; set; }

        /// <summary>
        /// 过帐用户名称
        /// </summary>
        [DataMember]
        [DisplayName("过帐用户名称")]
        public string PostingUserName { get; set; }

        /// <summary>
        /// 调整类型(0:货架[固定])
        /// </summary>
        [DataMember]
        [DisplayName("调整类型(0:货架[固定])")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string AdjType { get; set; }

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
        public string CreateUserID { get; set; }

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
        public string ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称")]
        public string ModifyUserName { get; set; }

        #endregion

        #region 扩展

        /// <summary>
        /// 总货位调整数
        /// </summary>
        [DataMember]
        [DisplayName("总货位调整数")]
        public string TotalShelfCount { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [DisplayName("")]
        public string StatusToStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [DisplayName("Remark")]
        public string Remark { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [DisplayName("WIDName")]
        public string WIDName { get; set; }
        #endregion

    }

    /// <summary>
    /// 
    /// </summary>
    public class WProductAdjShelfDetailsOpertion : BasePageModel
    {
        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ID { get; set; }

        /// <summary>
        /// 调整ID(WProductAdjPrice.AdjID)
        /// </summary>
        [DataMember]
        [DisplayName("调整ID(WProductAdjPrice.AdjID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string AdjID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID 二级)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID 二级)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WID { get; set; }

        /// <summary>
        /// 仓库商品ID(WProducts.WProductID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库商品ID(WProducts.WProductID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WProductID { get; set; }

        /// <summary>
        /// 商品ID(product.ProductID)
        /// </summary>
        [DataMember]
        [DisplayName("商品ID(product.ProductID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ProductId { get; set; }

        /// <summary>
        /// 库存单位
        /// </summary>
        [DataMember]
        [DisplayName("库存单位")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Unit { get; set; }

        /// <summary>
        /// 原货架ID
        /// </summary>
        [DataMember]
        [DisplayName("原货架ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string OldShelfID { get; set; }

        /// <summary>
        /// 原货架编号
        /// </summary>
        [DataMember]
        [DisplayName("原货架编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string OldShelfCode { get; set; }

        /// <summary>
        /// 新货架ID
        /// </summary>
        [DataMember]
        [DisplayName("新货架ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShelfID { get; set; }

        /// <summary>
        /// 新货架编号
        /// </summary>
        [DataMember]
        [DisplayName("新货架编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShelfCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]
        [Required(ErrorMessage = "{0}不能为空")]
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

        public string CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]
        public string CreateUserName { get; set; }

        #endregion

        #region 扩展
        /// <summary>
        /// 包装数
        /// </summary>
        [DataMember]
        [DisplayName("BigPackingQty")]
        public string BigPackingQty { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        [DisplayName("商品编码")]
        public string SKU { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        [DataMember]
        [DisplayName("商品条码")]
        public string BarCode { get; set; }
        #endregion
    }

    /// <summary>
    /// EXCEL导出
    /// </summary>
    public class WProductAdjShelfDetailsExcel : BaseModel
    {

        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]
        [ExcelNoExport]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ID { get; set; }

        /// <summary>
        /// 调整ID(WProductAdjPrice.AdjID)
        /// </summary>
        [DataMember]
        [DisplayName("调整ID(WProductAdjPrice.AdjID)")]
        [ExcelNoExport]
        [Required(ErrorMessage = "{0}不能为空")]
        public string AdjID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID 二级)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID 二级)")]
        [ExcelNoExport]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WID { get; set; }

        /// <summary>
        /// 仓库商品ID(WProducts.WProductID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库商品ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WProductID { get; set; }

        /// <summary>
        /// 商品ID(product.ProductID)
        /// </summary>
        [DataMember]
        [DisplayName("商品ID(product.ProductID)")]
        [ExcelNoExport]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ProductId { get; set; }

        /// <summary>
        /// 包装数
        /// </summary>
        [DataMember]
        [ExcelNoExport]
        [DisplayName("BigPackingQty")]
        public string BigPackingQty { get; set; }

        /// <summary>
        /// 商品编码
        /// </summary>
        [DataMember]
        [DisplayName("商品编码")]
        public string SKU { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DataMember]
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品条码
        /// </summary>
        [DataMember]
        [DisplayName("商品条码")]
        public string BarCode { get; set; }
   
        /// <summary>
        /// 配送单位
        /// </summary>
        [DataMember]
        [DisplayName("库存单位")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Unit { get; set; }

        /// <summary>
        /// 原货架ID
        /// </summary>
        [DataMember]
        [DisplayName("原货架ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string OldShelfID { get; set; }

        /// <summary>
        /// 原货架编号
        /// </summary>
        [DataMember]
        [DisplayName("原货位号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string OldShelfCode { get; set; }

        /// <summary>
        /// 新货架ID
        /// </summary>
        [DataMember]
        [DisplayName("新货架ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string ShelfID { get; set; }

        /// <summary>
        /// 新货架编号
        /// </summary>
        [DataMember]
        [DisplayName("新货位号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShelfCode { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("录单人员")]
        [Required(ErrorMessage = "{0}不能为空")]
        [ExcelNoExport]
        public string CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户ID")]
        [ExcelNoExport]
        public string CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]
        public string CreateUserName { get; set; }

        #endregion


    }
    public class StatusParam
    {
        public string UserName { get; set; }

        public string Time { get; set; }

        public string AdjId { get; set; }
    }
}