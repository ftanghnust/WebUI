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
    /// 仓库货区ShelfArea实体类
    /// </summary>
    [Serializable]
    public partial class ShelfAreaModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// ID(主键)
        /// </summary>        
        [DisplayName("ID(主键)")]
        public int ShelfAreaID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>        
        [DisplayName("仓库ID")]
        public int WID { get; set; }

        /// <summary>
        /// 货区编号
        /// </summary>        
        [DisplayName("货区编号")]
        [Required(ErrorMessage = "{0}不能为空")]        
        public string ShelfAreaCode { get; set; }

        /// <summary>
        /// 货区名称
        /// </summary>        
        [DisplayName("货区名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string ShelfAreaName { get; set; }

        /// <summary>
        /// 拣货APP最大显示数
        /// </summary>        
        [DisplayName("拣货APP最大显示数")]       
        public int PickingMaxRecord { get; set; }

        /// <summary>
        /// 排序号
        /// </summary>        
        [DisplayName("排序")]
        [Required(ErrorMessage = "{0}不能为空")]  
        public int SerialNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        [DisplayName("备注")]
        public string Remark { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>        
        [DisplayName("创建时间")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>

        [DisplayName("创建用户ID")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>        
        [DisplayName("创建用户名称")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 最新修改删除时间
        /// </summary>        
        [DisplayName("最新修改删除时间")]
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
        public ShelfAreaModel GetShelfAreaData(string id)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfAreaGetRequest()
            {
                ShelfAreaID = int.Parse(id)
            });

            ShelfAreaModel model = AutoMapperHelper.MapTo<ShelfAreaModel>(resp.Data);

            return model;
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public object DeleteShelfArea(string ids)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfAreaDelRequest()
            {
                ShelfAreaID = ids
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
        public string GetShelfAreaPageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductShelfAreaTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
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

    }
}