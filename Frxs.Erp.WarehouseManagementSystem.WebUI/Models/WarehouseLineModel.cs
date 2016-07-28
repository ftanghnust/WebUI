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
    public class WarehouseLineModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 主键(ID)
        /// </summary>        
        [DisplayName("主键(ID)")]
        public int LineID { get; set; }

        /// <summary>
        /// 配送线路编号(不能重复,不包括已删除的)
        /// </summary>        
        [DisplayName("线路编号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string LineCode { get; set; }

        /// <summary>
        /// 所属仓库(Warehouse.WID)
        /// </summary>        
        [DisplayName("所属仓库(Warehouse.WID)")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int WID { get; set; }

        /// <summary>
        /// 配送线路名称
        /// </summary>        
        [DisplayName("配送线路名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string LineName { get; set; }

        /// <summary>
        /// 用户编号(WarehouseEmp.EmpID and EmpType=2)
        /// </summary>        
        [DisplayName("配送负责人")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int EmpID { get; set; }

        /// <summary>
        /// 配送周期周1(0:不送;1:送)
        /// </summary>        
        [DisplayName("周一")]
        public bool SendW1 { get; set; }
        /// <summary>
        /// 配送周期周2(0:不送;1:送)
        /// </summary>        
        [DisplayName("周二")]
        public bool SendW2 { get; set; }

        /// <summary>
        /// 配送周期周3(0:不送;1:送)
        /// </summary>        
        [DisplayName("周三")]
        public bool SendW6 { get; set; }

        /// <summary>
        /// 配送周期周4(0:不送;1:送)
        /// </summary>

        [DisplayName("周四")]
        public bool SendW5 { get; set; }

        /// <summary>
        /// 配送周期周5(0:不送;1:送)
        /// </summary>        
        [DisplayName("周五")]
        public bool SendW4 { get; set; }

        /// <summary>
        /// 配送周期周6(0:不送;1:送)
        /// </summary>        
        [DisplayName("周六")]
        public bool SendW3 { get; set; }

        /// <summary>
        /// 配送周期周7(0:不送;1:送)
        /// </summary>        
        [DisplayName("周日")]
        public bool SendW7 { get; set; }

        /// <summary>
        /// 门店下单截止时间(hh:mm:ss)
        /// </summary>        
        [DisplayName("下单截止时间")]
        [Required(ErrorMessage = "{0}不能为空")]
        public TimeSpan OrderEndTime { get; set; }

        /// <summary>
        /// 配送距离KM
        /// </summary>        
        [DisplayName("配送距离")]
        
        public int? Distance { get; set; }

        /// <summary>
        /// 发货所要的时间(小时）
        /// </summary>        
        [DisplayName("配送花费时间")]
        public int? SendNeedTime { get; set; }

        /// <summary>
        /// 发货排序
        /// </summary>        
        [DisplayName("排序号")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int SerialNumber { get; set; }

        /// <summary>
        /// 备注
        /// </summary>        
        [DisplayName("备注")]

        public string Remark { get; set; }

        /// <summary>
        /// 是否暂停发货(0:未暂停发货;1暂停发货:;暂停的路线暂停发货)
        /// </summary>        
        [DisplayName("是否暂停发货(0:未暂停发货;1暂停发货:;暂停的路线暂停发货)")]
        public int IsLocked { get; set; }

        /// <summary>
        /// 是否删除(0:未删除;1:已删除)
        /// </summary>        
        [DisplayName("是否删除(0:未删除;1:已删除)")]
        public int IsDeleted { get; set; }

        /// <summary>
        /// 创建日期
        /// </summary>        
        [DisplayName("创建日期")]
        public DateTime CreateTime { get; set; }

        /// <summary>
        /// 创建用户ID
        /// </summary>        
        [DisplayName("创建用户ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int CreateUserID { get; set; }

        /// <summary>
        /// 创建用户名称
        /// </summary>        
        [DisplayName("创建用户名称")]
        public string CreateUserName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>        
        [DisplayName("最后修改时间")]
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>        
        [DisplayName("最后修改用户ID")]
        public int ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        /// </summary>        
        [DisplayName("最后修改用户名称")]
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 配送负责人
        /// </summary>
        [DisplayName("配送负责人")]
        public string EmpName { get; set; }

        /// <summary>
        /// 门店编号
        /// </summary>
        [DisplayName("门店编号")]
        public string ShopCode { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DisplayName("门店名称")]
        public string ShopName { get; set; }

        /// <summary>
        /// 门店名称
        /// </summary>
        [DisplayName("负责人电话")]
        public string UserMobile { get; set; }

        /// <summary>
        /// 配送门店数
        /// </summary>
        [DisplayName("配送门店数")]
        public int ShopNum { get; set; }

        /// <summary>
        /// 配送周期
        /// </summary>
        [DisplayName("配送周期")]
        public string SendW { get; set; }

        #endregion

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 配送员集合
        /// </summary>
        public SelectList EmpIDList { get; set; }

        public void BindEmpIDList()
        {

            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                UserType = "2"
            });
            SelectListItem item = new SelectListItem { Text = "", Value = "请选择" };
            if (resp != null && resp.Flag == 0)
            {
                EmpIDList = new SelectList(resp.Data.ItemList, "EmpID", "EmpName", true);

            }
            else
            {
                EmpIDList = null;
            }


        }

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public object DeleteWarehouseLine(string ids)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineDelRequest()
            {
                LineID = ids,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName

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

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public WarehouseLineModel GetWarehouseLineData(string id)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineGetRequest()
            {
                LineID = int.Parse(id)
            });

            WarehouseLineModel model = AutoMapperHelper.MapTo<WarehouseLineModel>(resp.Data);

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
        public string GetWarehouseLinePageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseLineTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    LineName = conditionDict.ContainsKey("LineName") ? Utils.NoHtml(conditionDict["LineName"].ToString()) : null,
                    EmpName = conditionDict.ContainsKey("EmpName") ? Utils.NoHtml(conditionDict["EmpName"].ToString()) : null,
                    UserMobile = conditionDict.ContainsKey("UserMobile") ? Utils.NoHtml(conditionDict["UserMobile"].ToString()) : null,
                    SendW = conditionDict.ContainsKey("SendW") ? Utils.NoHtml(conditionDict["SendW"].ToString()) : null,
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