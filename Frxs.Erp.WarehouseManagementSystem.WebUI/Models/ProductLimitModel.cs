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
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class ProductLimitQuery : BasePageModel
    {
        [DataMember]
        [DisplayName("单据号")]
        public string NoSaleID { get; set; }

        [DataMember]
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        [DataMember]
        [DisplayName("ERP编码")]
        public string SKU { get; set; }

        [DataMember]
        [DisplayName("商品条码")]
        public string BarCode { get; set; }

        [DataMember]
        [DisplayName("单据状态")]
        public string Status { get; set; }

        [DataMember]
        [DisplayName("生效开始时间")]
        public DateTime? BeginTimeFrom { get; set; }

        [DataMember]
        [DisplayName("生效结束时间")]
        public DateTime? BeginTimeEnd { get; set; }

        [DataMember]
        [DisplayName("活动名称")]
        public string PromotionName { get; set; }
    }

    public class ShopgroupListQuery : BasePageModel
    {
        [DataMember]
        [DisplayName("")]
        public string searchType { get; set; }

        [DataMember]
        [DisplayName("")]
        public string searchKey { get; set; }
    }

    [Serializable]
    public class ProductLimitModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 主键ID(WID + GUID)
        /// </summary>
        [DataMember]
        [DisplayName("主键ID(WID + GUID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string NoSaleID { get; set; }

        /// <summary>
        /// 仓库ID(WarehouseID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(WarehouseID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WID { get; set; }

        /// <summary>
        /// 仓库编号
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WCode { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DataMember]
        [DisplayName("仓库名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WName { get; set; }

        /// <summary>
        /// 活动主题
        /// </summary>
        [DataMember]
        [DisplayName("活动主题")]

        public string PromotionName { get; set; }

        /// <summary>
        /// 生效开始时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("生效开始时间(yyyy-MM-dd HH:mm:ss)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 生效结束时间(yyyy-MM-dd HH:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("生效结束时间(yyyy-MM-dd HH:mm:ss)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 状态(0:未提交;1:已提交;2:(立即生效)已过帐/已开始;3:已停用)
        /// </summary>
        [DataMember]
        [DisplayName("状态(0:未提交;1:已提交;2:立即生效;3:已停用)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Status { get; set; }

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
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]

        public string CreateUserName { get; set; }

        /// <summary>
        /// 最后修改时间(停用时也更新该字段)
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间(停用时也更新该字段)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID(停用时也更新该字段)
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID(停用时也更新该字段)")]

        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称(停用时也更新该字段)
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户名称(停用时也更新该字段)")]

        public string ModifyUserName { get; set; }

        #endregion

        #region 从表
        /// <summary>
        /// 详情表集合
        /// </summary>
        public IList<WProductNoSaleDetails> DetailsList { get; set; }

        /// <summary>
        /// 门店关联表集合
        /// </summary>
        public IList<WProductNoSaleShops> ShopList { get; set; }
        #endregion

        public string jsonProduct { get; set; }
        public string jsonGroup { get; set; }

        public partial class WProductNoSaleDetails
        {
            #region 模型
            /// <summary>
            /// 主键ID
            /// </summary>
            [DataMember]
            [DisplayName("主键ID")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ID { get; set; }

            /// <summary>
            /// 限购ID(WProductNoSale.NoSaleID)
            /// </summary>
            [DataMember]
            [DisplayName("限购ID(WProductNoSale.NoSaleID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string NoSaleID { get; set; }

            /// <summary>
            /// 仓库ID(Warehouse.WID 二级)
            /// </summary>
            [DataMember]
            [DisplayName("仓库ID(Warehouse.WID 二级)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int WID { get; set; }

            /// <summary>
            /// 仓库商品ID(WProducts.WProductID)
            /// </summary>
            [DataMember]
            [DisplayName("仓库商品ID(WProducts.WProductID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int WProductID { get; set; }

            /// <summary>
            /// 商品ID(product.ProductID)
            /// </summary>
            [DataMember]
            [DisplayName("商品ID(product.ProductID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ProductID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [DataMember]
            [DisplayName("创建时间")]
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

            #endregion
        }

        public partial class WProductNoSaleDetails
        {
            /// <summary>
            /// 库存单位
            /// </summary>
            [DataMember]
            public string Unit { get; set; }

            /// <summary>
            /// 商品名称
            /// </summary>
            [DataMember]
            public string ProductName { get; set; }


            /// <summary>
            /// 配送单位包装数
            /// </summary>
            [DataMember]
            public decimal BigPackingQty { get; set; }

            /// <summary>
            /// 配送单位
            /// </summary>
            [DataMember]
            public string BigUnit { get; set; }

            /// <summary>
            /// 配送价格 SalePrice*包装数
            /// </summary>
            [DataMember]
            public decimal SalePrice { get; set; }

            /// <summary>
            /// 商品条码
            /// </summary>
            public string BarCode { get; set; }

            /// <summary>
            /// ERP编码
            /// </summary>
            [DataMember]
            public string SKU { get; set; }
        }

        public partial class WProductNoSaleShops
        {
            #region 模型
            /// <summary>
            /// 主键ID
            /// </summary>
            [DataMember]
            [DisplayName("主键ID")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int ID { get; set; }

            /// <summary>
            /// 限购ID(WProductNoSale.NoSaleID)
            /// </summary>
            [DataMember]
            [DisplayName("限购ID(WProductNoSale.NoSaleID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public string NoSaleID { get; set; }

            /// <summary>
            /// 仓库ID(Warehouse.WID 二级)
            /// </summary>
            [DataMember]
            [DisplayName("仓库ID(Warehouse.WID 二级)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int WID { get; set; }

            /// <summary>
            /// 门店群组ID(ShopGroup.GroupID)
            /// </summary>
            [DataMember]
            [DisplayName("门店群组ID(ShopGroup.GroupID)")]
            [Required(ErrorMessage = "{0}不能为空")]
            public int GroupID { get; set; }

            /// <summary>
            /// 创建时间
            /// </summary>
            [DataMember]
            [DisplayName("创建时间")]
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

            #endregion
        }

        public partial class WProductNoSaleShops
        {
            /// <summary>
            /// 分组名称 取自门店分组表 仅用于前台显示关联的数据
            /// </summary>
            [DataMember]
            public string GroupName { get; set; }

            /// <summary>
            /// 分组编号 取自门店分组表 仅用于前台显示关联的数据
            /// </summary>
            [DataMember]
            public string GroupCode { get; set; }

            /// <summary>
            /// 分组备注 取自门店分组表 仅用于前台显示关联的数据
            /// </summary>
            [DataMember]
            public string Remark { get; set; }
            /// <summary>
            /// 门店分组下的门店个数 取自门店分组详情表 仅用于前台显示关联的数据
            /// </summary>
            [DataMember]
            public int ShopNum { get; set; }
        }

        #region 获取分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetProductLimitPageData(ProductLimitQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreateProductSdkClient();
                //Dictionary<string, object> conditionDict = base.PrePareFormParam();
                int? status = null;
                if (!String.IsNullOrEmpty(cpm.Status))
                {
                    status = int.Parse(cpm.Status);
                }
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleListGetRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    NoSaleID = cpm.NoSaleID,
                    ProductName = cpm.ProductName,
                    SKU = cpm.SKU,
                    BarCode = cpm.BarCode,
                    Status = status,
                    BeginTimeFrom = cpm.BeginTimeFrom,
                    BeginTimeEnd = cpm.BeginTimeEnd,
                    PromotionName = cpm.PromotionName,
                    //SortBy = cpm.sort + " " + cpm.order
                    SortBy = "CreateTime" + " " + "desc"
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

        #region 批量删除数据
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="buyids">主键</param>
        /// <returns>对象</returns>
        public int DeleteProductLimit(string ids)
        {
            var IdList = ids.Split(',').ToList();
            var ServiceCenter = WorkContext.CreateProductSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleDelRequest()
            {
                IdList = IdList
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_2H,
                                       ConstDefinition.XSOperatorActionDel, string.Format("{0}商品限购单[{1}]", ConstDefinition.XSOperatorActionDel, ids));
                result = resp.Data;
            }
            return result;
        }
        #endregion

        public ProductLimitModel GetProductLimitData(string noSaleID)
        {
            var model = new ProductLimitModel();
            if (!String.IsNullOrEmpty(noSaleID))
            {
                var ServiceCenter = WorkContext.CreateProductSdkClient();
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleGetRequest()
                {
                    NoSaleID = noSaleID
                });
                if (resp != null && resp.Flag == 0)
                {
                    model = AutoMapperHelper.MapTo<ProductLimitModel>(resp.Data.wProductNoSale);
                    IList<WProductNoSaleDetails> productdetails = new List<WProductNoSaleDetails>();
                    productdetails = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductNoSaleGetResp.WProductNoSaleDetails, WProductNoSaleDetails>(resp.Data.productdetails);
                    model.DetailsList = productdetails;
                    IList<WProductNoSaleShops> shopGroups = new List<WProductNoSaleShops>();
                    shopGroups = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWProductNoSaleGetResp.WProductNoSaleShops, WProductNoSaleShops>(resp.Data.shopGroups);
                    model.ShopList = shopGroups;
                }
            }
            return model;
        }
    }
}