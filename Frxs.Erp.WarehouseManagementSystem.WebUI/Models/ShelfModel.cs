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

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 仓库货架表Shelf实体类
    /// </summary>
    [Serializable]
    public partial class ShelfModel : BaseModel
    {

        #region 模型
        /// <summary>
        /// ID(主键)
        /// </summary>        
        [DisplayName("ID(主键)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ShelfID { get; set; }

        /// <summary>
        /// 货位编号(同一个仓库不能重复)
        /// </summary>        
        [DisplayName("货位编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShelfCode { get; set; }

        /// <summary>
        /// 所属货区ID(ShelfArea.ShelfAreaID)
        /// </summary>        
        [DisplayName("所属货区")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int ShelfAreaID { get; set; }

        /// <summary>
        /// 货位类型(0:存储;1:)
        /// </summary>        
        [DisplayName("货位类型")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShelfType { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>        
        [DisplayName("仓库")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WID { get; set; }

        /// <summary>
        /// 状态(0:正常;1:冻结)
        /// </summary>        
        [DisplayName("状态")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string Status { get; set; }

        /// <summary>
        /// 最新修改删除时间
        /// </summary>        
        [DisplayName("最新修改删除时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 最后修改删除用户ID
        /// </summary>        
        [DisplayName("最后修改删除用户ID")]
        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改删除用户名称
        /// </summary>        
        [DisplayName("最后修改删除用户名称")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>
        [DisplayName("仓库名称")]
        public string WName { get; set; }

        /// <summary>
        /// 货区
        /// </summary>
        [DisplayName("货区")]
        public string ShelfAreaName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>       
        [DisplayName("状态")]
        public string StatusStr { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        [DisplayName("备注")]
        public string Remark { get; set; }

        #endregion

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

      

        /// <summary>
        /// 货区集合
        /// </summary>
        public SelectList ShelfAreaList { get; set; }

        public void BindShelfAreaList()
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfAreaTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });

            if (resp != null && resp.Flag == 0)
            {
                ShelfAreaList = new SelectList(resp.Data.ItemList, "ShelfAreaID", "ShelfAreaName", true);
            }
            else
            {
                ShelfAreaList = null;
            }
        }

        /// <summary>
        /// 存储集合
        /// </summary>
        public SelectList ShelfTypeList { get; set; }

        public void BindShelfTypeList()
        {
            ShelfTypeList = new SelectList(new[] { new { Text = "存储", Value = "0" } }, "Value", "Text", true);
        }

        /// <summary>
        /// 状态集合
        /// </summary>
        public SelectList StatusList { get; set; }

        public void BindStatusList()
        {
            StatusList = new SelectList(new[] { new { Text = ConstDefinition.ERP_BASEDATA_NAME_NORMAL, Value = 0 }, new { Text = ConstDefinition.ERP_BASEDATA_NAME_FREEZE, Value = 1 } }, "Value", "Text", true);
        }

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public ShelfModel GetShelfData(string id)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfGetRequest()
            {
                ShelfID=int.Parse(id)
            });

            ShelfModel model = AutoMapperHelper.MapTo<ShelfModel>(resp.Data);

            return model;
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public object DeleteShelf(string ids)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfDelRequest()
            {
                ShelfID = ids 
            });

            if (resp.Flag == 0)
            {
                return new ResultData
                {
                    Flag = ConstDefinition.FLAG_SUCCESS,
                    Info = string.Format("成功删除{0}条数据", resp.Data)
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

        #region 获取分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>        
        public string GetShelfPageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                int? shelfareaid=null;
                if (conditionDict.ContainsKey("ShelfAreaID"))
                {
                    shelfareaid = int.Parse(Utils.NoHtml(conditionDict["ShelfAreaID"].ToString()));
                }

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    ShelfCode = conditionDict.ContainsKey("ShelfCode") ? Utils.NoHtml(conditionDict["ShelfCode"].ToString()) : null,
                    ShelfAreaID = shelfareaid,
                    Status = conditionDict.ContainsKey("Status") ? Utils.NoHtml(conditionDict["Status"].ToString()) : null,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });

                if (resp!=null&&resp.Flag == 0)
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


}