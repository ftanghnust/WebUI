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
using Frxs.Erp.ServiceCenter.Promotion.SDK.Resp;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// WarehouseMessage实体类
    /// </summary>
    [Serializable]
    public partial class WarehouseMessageModel : BaseModel
    {

        /// <summary>
        /// 消息类型集合
        /// </summary>
        public SelectList MessageTypeList { get; set; }

        public void BindMessageTypeList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductSysDictDetailGetListRequest()
            {
                dictCode = "MessageType"
            });

            if (resp != null && resp.Flag == 0)
            {

                MessageTypeList = new SelectList(resp.Data, "DictValue", "DictLabel", true);
            }
            else
            {
                MessageTypeList = null;
            }
        }

        public IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> BindMessageTypeSysDictDetailList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductSysDictDetailGetListRequest()
            {
                dictCode = "MessageType"
            });
            if (resp != null && resp.Flag == 0)
            {
                return resp.Data;
            }
            else
            {
                return null;
            }
        }

        private string GetMessageTypeLabel(IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> dataList, int messageTypeId)
        {
            if (dataList != null && dataList.Count > 0)
            {
                string messageTypeStr = "";
                foreach (FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData obj in dataList)
                {
                    if (obj.DictValue == messageTypeId.ToString())
                    {
                        messageTypeStr = obj.DictLabel;
                    }
                }
                return messageTypeStr;
            }
            else
            {
                return "";
            }

        }


        #region 批量删除数据
        /// <summary>
        /// 批量删除数据
        /// </summary>
        /// <param name="buyids">主键</param>
        /// <returns>对象</returns>
        public int DeleteWarehouseMessage(string ids, int userId, string userName)
        {
            var ServiceCenter = WorkContext.CreatePromotionSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageDelRequest()
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
        public int WarehouseMessageChangeStatus(string ids, int status, int userId, string userName)
        {
            var ServiceCenter = WorkContext.CreatePromotionSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageChangeStatusRequest()
            {
                IDs = ids,
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
        public string GetWarehouseMessagePageData(WarehouseMessageQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var ServiceCenter = WorkContext.CreatePromotionSdkClient();
              //  Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageGetListRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = " ModityTime desc ",
                    MessageType = cpm.MessageType.HasValue ?  cpm.MessageType.Value : (int?)null,
                    Title = !string.IsNullOrEmpty(cpm.Title) ? Utils.NoHtml(cpm.Title) : null,
                    ConfUserName = !string.IsNullOrEmpty(cpm.ConfUserName) ? Utils.NoHtml(cpm.ConfUserName) : null,
                    Status = cpm.Status.HasValue ? cpm.Status.Value : (int?)null,
                    BeginTime = cpm.BeginTime.HasValue ? cpm.BeginTime : (DateTime?)null,
                    EndTime = cpm.EndTime.HasValue ? cpm.EndTime : (DateTime?)null,
                    WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    RangType = 2 //用于区分B2B及仓库后台，用此字段代替 为2时为仓库后台调用

                });

                if (resp != null && resp.Flag == 0)
                {
                    IList<FrxsErpPromotionWarehouseMessageGetListResp.WarehouseMessage> itemList = resp.Data.ItemList;
                    IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> dictDetailList = BindMessageTypeSysDictDetailList();
                    foreach (FrxsErpPromotionWarehouseMessageGetListResp.WarehouseMessage wmobj in itemList)
                    {
                        wmobj.MessageTypeStr = GetMessageTypeLabel(dictDetailList, wmobj.MessageType.Value);
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
        public WarehouseMessageOpertion GetWarehouseMessageData(string ID, int userId, string userName)
        {
            WarehouseMessageOpertion model = new WarehouseMessageOpertion();
            try
            {
                //获取
                var serviceCenter = WorkContext.CreatePromotionSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageGetModelRequest()
                {
                    ID = ID,
                    UserId = userId,
                    UserName = userName,
                    WareHouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp != null && resp.Flag == 0)
                {
                    IList<FrxsErpProductSysDictDetailGetListResp.FrxsErpProductSysDictDetailGetListRespData> dictDetailList = BindMessageTypeSysDictDetailList();
                    resp.Data.order.MessageTypeStr = GetMessageTypeLabel(dictDetailList, resp.Data.order.MessageType.Value);

                    model = Frxs.Platform.Utility.Map.AutoMapperHelper.MapTo<WarehouseMessageOpertion>(resp.Data.order);

                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return model;
        }

        #endregion


        #region 获取门店分组分页数据

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="searchQuery"></param>
        /// <returns></returns>
        public string GetMessageShopGroupData(WarehouseMessageShopsQuery searchQuery)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreatePromotionSdkClient();

                if (string.IsNullOrEmpty(searchQuery.WarehouseMessageID) || searchQuery.WarehouseMessageID == "null")
                {
                    searchQuery.WarehouseMessageID = "0";
                }

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Promotion.SDK.Request.FrxsErpPromotionWarehouseMessageShopsGetListRequest()
                {
                    PageIndex = searchQuery.page,
                    PageSize = searchQuery.rows,
                    SortBy = searchQuery.sort,
                    WarehouseMessageID = int.Parse(searchQuery.WarehouseMessageID),
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    WarehouseId=WorkContext.CurrentWarehouse.Parent.WarehouseId
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


        #region 获取门店分组分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetShopGroupModelPageData(WarehouseMessageShopsQuery searchQuery)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();


                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGroupTableListRequest()
                {
                    PageIndex = searchQuery.page,
                    PageSize = searchQuery.rows,
                    SortBy = searchQuery.sort,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    GroupName = searchQuery.GroupName
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

    }
    public class WarehouseMessageOpertion : BasePageModel
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
        public int? ID { get; set; }

        /// <summary>
        /// 仓库编号(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("加入门店")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? WID { get; set; }

        /// <summary>
        /// 消息头
        /// </summary>
        [DataMember]
        [DisplayName("消息标题")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// 消息类型(0：重要消息;1:促销;2:其他)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string MessageType { get; set; }

        [DataMember]
        [DisplayName("消息类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string MessageTypeName { get; set; }

        /// <summary>
        /// 消息类型(0:全部门店;1:指定群组)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型ID")]
        public int? RangType { get; set; }

        [DataMember]
        [DisplayName("消息类型")]
        public string RangTypeName { get; set; }

        /// <summary>
        /// 发布时间开始（yyyy-MM-dd hh:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("发布时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string BeginTime { get; set; }

        /// <summary>
        /// 发布时间结束（yyyy-MM-dd hh:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("至")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string EndTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [DataMember]
        [DisplayName("内容")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Message { get; set; }

        /// <summary>
        /// 状态(0:未发布;1:已发布;2:已停止/已删除)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? Status { get; set; }

        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string StatusName { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        [DataMember]
        [DisplayName("确认时间")]

        public string ConfTime { get; set; }

        /// <summary>
        /// 确认人员ID
        /// </summary>
        [DataMember]
        [DisplayName("确认人员ID")]

        public int? ConfUserID { get; set; }

        /// <summary>
        /// 确认人员名称
        /// </summary>
        [DataMember]
        [DisplayName("发布人")]

        public string ConfUserName { get; set; }

        /// <summary>
        /// 置顶显示
        /// </summary>
        [DataMember]
        [DisplayName("置顶显示")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? IsFirst { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string CreateTime { get; set; }

        /// <summary>
        /// 创建用户 ID
        /// </summary>
        [DataMember]
        [DisplayName("创建用户 ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>
        [DataMember]
        [DisplayName("创建用户名称")]

        public string CreateUserName { get; set; }

        /// <summary>
        /// 修改时间
        /// </summary>
        [DataMember]
        [DisplayName("修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ModityTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用户ID")]

        public int? ModityUserID { get; set; }

        /// <summary>
        /// 最后修改用记名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用记名称")]

        public string ModityUserName { get; set; }

        #endregion

        #region 扩展字段
        /// <summary>
        /// 置顶显示
        /// </summary>
        [DataMember]
        [DisplayName("置顶显示")]
        public string IsFirstStr { get; set; }

        /// <summary>
        /// 消息类型(0：重要消息;1:促销;2:其他)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型")]
        public string MessageTypeStr { get; set; }


        /// <summary>
        /// 消息类型(0:全部门店;1:指定群组)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型")]
        public string RangTypeStr { get; set; }


        /// <summary>
        /// 状态(0:未发布;1:已发布;2:已停止/已删除)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string StatusStr { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public bool? IsFirstByBool { get; set; }
        public void BindIsFirstByBool()
        {
            IsFirstByBool = IsFirst == 1 ? true : false;
        }
        /// <summary>
        /// 
        /// </summary>
        public bool? StatusByBool { get; set; }
        public void BindStatusByBool()
        {
            StatusByBool = Status == 1 ? false : true;
        }
        #endregion

    }

    /// <summary>
    /// 查询类
    /// </summary>
    public class WarehouseMessageQuery : BasePageModel
    {
        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? ID { get; set; }

        /// <summary>
        /// 仓库编号(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("加入门店")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? WID { get; set; }

        /// <summary>
        /// 消息头
        /// </summary>
        [DataMember]
        [DisplayName("消息标题")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Title { get; set; }

        /// <summary>
        /// 消息类型(0：重要消息;1:促销;2:其他)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? MessageType { get; set; }

        [DataMember]
        [DisplayName("消息类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string MessageTypeName { get; set; }

        /// <summary>
        /// 消息类型(0:全部门店;1:指定群组)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型ID")]
        public string RangType { get; set; }

        [DataMember]
        [DisplayName("消息类型")]
        public string RangTypeName { get; set; }

        /// <summary>
        /// 发布时间开始（yyyy-MM-dd hh:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("发布时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? BeginTime { get; set; }

        /// <summary>
        /// 发布时间结束（yyyy-MM-dd hh:mm:ss)
        /// </summary>
        [DataMember]
        [DisplayName("至")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime? EndTime { get; set; }

        /// <summary>
        /// 内容
        /// </summary>
        [DataMember]
        [DisplayName("内容")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Message { get; set; }

        /// <summary>
        /// 状态(0:未发布;1:已发布;2:已停止/已删除)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int? Status { get; set; }


        [DataMember]
        [DisplayName("状态")]
        public string StatusName { get; set; }

        /// <summary>
        /// 确认时间
        /// </summary>
        [DataMember]
        [DisplayName("确认时间")]

        public DateTime? ConfTime { get; set; }

        /// <summary>
        /// 确认人员ID
        /// </summary>
        [DataMember]
        [DisplayName("确认人员ID")]

        public int? ConfUserID { get; set; }

        /// <summary>
        /// 确认人员名称
        /// </summary>
        [DataMember]
        [DisplayName("发布人")]

        public string ConfUserName { get; set; }

        /// <summary>
        /// 置顶显示
        /// </summary>
        [DataMember]
        [DisplayName("置顶显示")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int IsFirst { get; set; }

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

        public int ModityUserID { get; set; }

        /// <summary>
        /// 最后修改用记名称
        /// </summary>
        [DataMember]
        [DisplayName("最后修改用记名称")]

        public string ModityUserName { get; set; }

        #endregion

        #region 扩展字段
        /// <summary>
        /// 置顶显示
        /// </summary>
        [DataMember]
        [DisplayName("置顶显示")]
        public string IsFirstStr { get; set; }

        /// <summary>
        /// 消息类型(0：重要消息;1:促销;2:其他)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型")]
        public string MessageTypeStr { get; set; }


        /// <summary>
        /// 消息类型(0:全部门店;1:指定群组)
        /// </summary>
        [DataMember]
        [DisplayName("消息类型")]
        public string RangTypeStr { get; set; }


        /// <summary>
        /// 状态(0:未发布;1:已发布;2:已停止/已删除)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string StatusStr { get; set; }

        #endregion
    }


    /// <summary>
    /// 
    /// </summary>
    public class WarehouseMessageShopsQuery : BasePageModel
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
        /// 消息主表ID(WarehouseMessage.ID)
        /// </summary>
        [DataMember]
        [DisplayName("消息主表ID(WarehouseMessage.ID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string WarehouseMessageID { get; set; }

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
        /// 门店群组名称
        /// </summary>
        [DataMember]
        [DisplayName("门店群组名称")]

        public string GroupName { get; set; }

        /// <summary>
        /// 门店群组编码
        /// </summary>
        [DataMember]
        [DisplayName("门店群组编码")]

        public string GroupCode { get; set; }

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
}