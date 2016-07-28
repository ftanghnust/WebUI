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

    /// <summary>
    /// 查询条件
    /// </summary>
    public class ShopGroupSearchModel : SearchModelBase
    {
        #region 模型

        /// <summary>
        /// 群组名称
        /// </summary>
        [DataMember]
        [DisplayName("群组名称")]        
        public string GroupName { get; set; }

        

        /// <summary>
        /// 群组编号
        /// </summary>
        [DataMember]
        [DisplayName("群组编号")]
        public string GroupCode { get; set; }

        #endregion

    }

    /// <summary>
    /// 显示列表
    /// </summary>
    public class ShopGroupListModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]       
        public long GroupID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID)")]        
        public int WID { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        [DataMember]
        [DisplayName("群组名称")]        
        public string GroupName { get; set; }

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

        /// <summary>
        /// 门店数量
        /// </summary>
        [DataMember]
        [DisplayName("门店数量")]
        public int ShopNum { get; set; }

        /// <summary>
        /// 门店群组
        /// </summary>
        [DataMember]
        [DisplayName("群组编号")]
        public string GroupCode { get; set; }

        #endregion


    }

    public class ShopGroupModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 主键ID
        /// </summary>
        [DataMember]
        [DisplayName("主键ID")]
        public int GroupID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("仓库ID(Warehouse.WID)")]
        public int WID { get; set; }

        /// <summary>
        /// 群组名称
        /// </summary>
        [DataMember]
        [DisplayName("群组名称")]
        public string GroupName { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        [DataMember]
        [DisplayName("备注")]
        public string Remark { get; set; }

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

        /// <summary>
        /// 门店群组
        /// </summary>
        [DataMember]
        [DisplayName("群组编号")]       
        public string GroupCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        [DataMember]
        [DisplayName("创建时间")]        
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 门店集合
        /// </summary>
        [DataMember]
        public IList<ShopGroupDetails> List { get; set; }

        #endregion


        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public ShopGroupModel GetShopGroupData(string id)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGroupGetRequest()
            {
                GroupID = int.Parse(id)

            });

            ShopGroupModel model = new ShopGroupModel();
            model.GroupID = resp.Data.GroupID;
            model.GroupCode = resp.Data.GroupCode;
            model.WID = resp.Data.WID;
            model.GroupName = resp.Data.GroupName;
            model.Remark = resp.Data.Remark;
            model.CreateTime = resp.Data.CreateTime;
            model.CreateUserName = resp.Data.CreateUserName;
            
            model.List = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductShopGroupGetResp.ShopGroupDetails,ShopGroupDetails>(resp.Data.List);
            //AutoMapperHelper.MapTo<ShopGroupModel>(resp.Data);

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
        public string GetShopGroupModelPageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGroupTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    GroupCode = conditionDict.ContainsKey("GroupCode") ? Utils.NoHtml(conditionDict["GroupCode"].ToString()) : null,
                    GroupName = conditionDict.ContainsKey("GroupName") ? Utils.NoHtml(conditionDict["GroupName"].ToString()) : null,
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

        #region 保存数据
        public object SaveShopGroupData(ShopGroupModel model)
        {
            if (model.List == null)
            {
                return new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info ="明细不能为空！"
                };
            }

            model.List.Where(m => m.ShopID == 0).ToList().ForEach(x => { model.List.Remove(x); });
            var serviceCenter = WorkContext.CreateProductSdkClient();

            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGroupSaveRequest()
            {                
                GroupID = model.GroupID,
                GroupCode = model.GroupCode,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                GroupName = model.GroupName,
                Remark = model.Remark,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName,
                List = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<ShopGroupDetails,Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGroupSaveRequest.ShopGroupDetails>(model.List),
                Flag=model.GroupID==0?"Add":""
              
            });

            if (resp.Flag == 0)
            {
                return new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = "操作成功"
                };
            }
            else
            {
                return new ResultData
                {
                    Flag = ConstDefinition.FLAG_FAIL,
                    Info = resp.Info
                };
            }



        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public int DeleteShopGroup(string ids)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShopGroupDelRequest()
            {
                GroupID = StringExtension.ToIntArray(ids,',').ToList(),
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName

            });

            return resp.Data;
        }
        #endregion
    }
}