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
    [Serializable]
    public class WarehouseEmpShelfModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 用户编号
        /// </summary>       
        [DisplayName("用户编号")]       
        public int EmpID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>        
        [DisplayName("用户名称")]        
        public string EmpName { get; set; }

        /// <summary>
        /// 用户登录帐户(手机号码;员工编号;唯一[不包括删除的])
        /// </summary>        
        [DisplayName("用户登录帐户")]        
        public string UserAccount { get; set; }

        /// <summary>
        /// 是否冻结(0:未冻结;1:已冻结)
        /// </summary>       
        [DisplayName("状态")]        
        public string IsFrozen { get; set; }

        /// <summary>
        /// 仓库名称
        /// </summary>       
        [DisplayName("仓库名称")]
        public string WName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>        
        [DisplayName("状态")]
        public string StatusStr { get; set; }

        /// <summary>
        /// 货区名称
        /// </summary>        
        [DisplayName("货区名称")]        
        public string ShelfAreaName { get; set; }

        /// <summary>
        /// 货位数量
        /// </summary>        
        [DisplayName("货位数量")]
        public int ShelfNum { get; set; }

        /// <summary>
        /// ID(主键)
        /// </summary>        
        [DisplayName("拣货区")]        
        public int? ShelfAreaID { get; set; }

        public string ShelfIDs { get; set; }

        public SelectList shelfList { get; set; }

        public int ShelfID { get; set; }
        #endregion

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public void GetEmpShelfData(string id)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpShelfListRequest()
            {
                EmpID = id
            });
            IList<WarehouseEmpShelf> list = AutoMapperHelper.MapToList<Frxs.Erp.ServiceCenter.Product.SDK.Resp.FrxsErpProductWarehouseEmpShelfListResp.FrxsErpProductWarehouseEmpShelfListRespData, WarehouseEmpShelf>(resp.Data);

            if (list != null && list.Count > 0)
            {
               
                ShelfAreaID = list[0].ShelfAreaID;
            }
            shelfList = new SelectList(list, "ShelfID", "ShelfCode", true);
        }
        #endregion

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
                ShelfAreaList = new SelectList(resp.Data.ItemList, "ShelfAreaID", "ShelfAreaName");
            }
            else
            {
                ShelfAreaList = null;
            }
        }
       
        

        #region 获取分页数据
        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetWarehouseEmpShelfPageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpShelfTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    EmpName = conditionDict.ContainsKey("EmpName") ? Utils.NoHtml(conditionDict["EmpName"].ToString()) : null,
                    UserAccount = conditionDict.ContainsKey("UserAccount") ? Utils.NoHtml(conditionDict["UserAccount"].ToString()) : null,
                    IsFrozen = conditionDict.ContainsKey("IsFrozen") ? Utils.NoHtml(conditionDict["IsFrozen"].ToString()) : null,
                    ShelfAreaID = conditionDict.ContainsKey("ShelfAreaID") ? Utils.NoHtml(conditionDict["ShelfAreaID"].ToString()) : null,
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

    /// <summary>
    /// 仓库用户拣货区表WarehouseEmpShelf实体类
    /// </summary>
    [Serializable]   
    public partial class WarehouseEmpShelf 
    {

        #region 模型
        /// <summary>
        /// 主键
        /// </summary>        
        [DisplayName("主键")]        
        public int ID { get; set; }

        /// <summary>
        /// 用户编号
        /// </summary>        
        [DisplayName("用户编号")]        
        public int EmpID { get; set; }

        /// <summary>
        /// 货区编号(ShelfArea.ShelfAreaID)
        /// </summary>        
        [DisplayName("货区编号(ShelfArea.ShelfAreaID)")]        
        public int ShelfAreaID { get; set; }

        /// <summary>
        /// 货位编号(Shelf.ShelfID)
        /// </summary>        
        [DisplayName("货位编号")]
        public int ShelfID { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>        
        [DisplayName("创建日期")]        
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
        /// 货位编号(同一个仓库不能重复)
        /// </summary>        
        [DisplayName("货位编号")]       
        public string ShelfCode { get; set; }
        #endregion
    }
}