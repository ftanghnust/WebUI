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
    public class WadvertisementQuery : BasePageModel
    {

    }

    public class ProductListQuery : BasePageModel
    {
        [DataMember]
        [DisplayName("")]
        public string searchType { get; set; }

        [DataMember]
        [DisplayName("")]
        public string searchKey { get; set; }
    }

    public class WadvertisementProductListQuery : BasePageModel
    {
        [DataMember]
        [DisplayName("")]
        public string AdvertisementID { get; set; }
    }

    [Serializable]
    public class WadvertisementModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int AdvertisementID { get; set; }

        /// <summary>
        /// 仓库编号(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库编号(Warehouse.WID)")]

        public int WID { get; set; }

        /// <summary>
        /// 广告位置（1、轮播广告，2、底部广告，3、橱窗）
        /// </summary>
        [DataMember]
        [DisplayName("广告位置")]

        public int AdvertisementPosition { get; set; }

        /// <summary>
        /// 名称
        /// </summary>
        [DataMember]
        [DisplayName("名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string AdvertisementName { get; set; }

        /// <summary>
        /// 排序
        /// </summary>
        [DataMember]
        [DisplayName("排序")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int Sort { get; set; }

        /// <summary>
        /// 图片地址
        /// </summary>
        [DataMember]
        [DisplayName("图片")]

        public string ImagesSrc { get; set; }

        /// <summary>
        /// 选中后的图标
        /// </summary>
        [DataMember]
        [DisplayName("选中后的图标")]

        public string SelectImagesSrc { get; set; }

        /// <summary>
        /// 广告类型（1、促销，2、分类，3、商品）注：橱窗与此字段无关
        /// </summary>
        [DataMember]
        [DisplayName("广告类型（1、促销，2、分类，3、商品）注：橱窗与此字段无关")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int AdvertisementType { get; set; }

        /// <summary>
        /// 是否删除
        /// </summary>
        [DataMember]
        [DisplayName("是否删除")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int IsDelete { get; set; }

        /// <summary>
        /// 是否锁定
        /// </summary>
        [DataMember]
        [DisplayName("是否锁定")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int IsLocked { get; set; }

        /// <summary>
        /// 开始时间
        /// </summary>
        [DataMember]
        [DisplayName("开始时间")]

        public DateTime StartTime { get; set; }

        /// <summary>
        /// 结束时间
        /// </summary>
        [DataMember]
        [DisplayName("结束时间")]

        public DateTime EndTime { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户 ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户 ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        [DisplayName("修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime ModityTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ModityUserID { get; set; }

        /// <summary>
        /// 最后修改用记名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用记名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ModityUserName { get; set; }

        public string AdvertisementProduct { get; set; }
        #endregion

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public WadvertisementModel GetWadvertisement(string id)
        {
            var serviceCenter = WorkContext.CreatePromotionSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementGetModelRequest()
            {
                ID = id
            });

            WadvertisementModel model = AutoMapperHelper.MapTo<WadvertisementModel>(resp.Data);

            return model;
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
        public string GetWadvertisementPageData(int pageIndex, int pageSize, string orderField)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreatePromotionSdkClient();
                //Dictionary<string, object> conditionDict = base.PrePareFormParam();
                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementGetListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize  //,
                    //MessageType = conditionDict.ContainsKey("MessageType") ? int.Parse(conditionDict["MessageType"].ToString()) : (int?) null,
                    //Title = conditionDict.ContainsKey("Title") ? int.Parse(conditionDict["Title"].ToString()) : (int?) null,
                    //ConfUserName = conditionDict.ContainsKey("ConfUserName") ? Utils.NoHtml(conditionDict["ConfUserName"].ToString()) : null,
                    //Status = conditionDict.ContainsKey("Status") ? int.Parse(conditionDict["Status"].ToString()) : (int?)null,
                    //BeginTime = conditionDict.ContainsKey("BeginTime") ? DateTime.Parse(conditionDict["BeginTime"].ToString()) : (DateTime?)null,
                    //EndTime = conditionDict.ContainsKey("EndTime") ? DateTime.Parse(conditionDict["EndTime"].ToString()) : (DateTime?)null
                    ,WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                    ,IsDelete = 0
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
        public int DeleteWadvertisement(string ids, int warehouseId)
        {
            var ServiceCenter = WorkContext.CreatePromotionSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWAdvertisementDelRequest()
            {
                IDs = ids,
                WID = warehouseId
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                //写操作日志
                OperatorLogHelp.Write(Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdXSOperatorLogWriteRequest.MenuIDEnum.WMS_4A,
                                       ConstDefinition.XSOperatorActionDel, string.Format("{0}橱窗推荐[{1}]", ConstDefinition.XSOperatorActionDel, ids));
                result = resp.Data;
            }
            return result;
        }
        #endregion

        public SelectList AdvertisementPositionList = new SelectList(new[] { new { Text = "轮播广告", Value = "1" }, new { Text = "底部广告", Value = "2" }, new { Text = "橱窗", Value = "3" } }, "Value", "Text", true);
    }
}