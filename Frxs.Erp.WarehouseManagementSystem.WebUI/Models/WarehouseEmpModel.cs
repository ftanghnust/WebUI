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
    /// 仓库用户员工表WarehouseEmp实体类
    /// </summary>
    [Serializable]
    public class WarehouseEmpModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 用户编号
        /// </summary>
        [DataMember]
        [DisplayName("用户编号")]
        public int EmpID { get; set; }

        /// <summary>
        /// 用户名称
        /// </summary>
        [DataMember]
        [DisplayName("用户名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string EmpName { get; set; }


        /// <summary>
        /// 用户登录帐户(手机号码;员工编号;唯一[不包括删除的])
        /// </summary>
        [DataMember]
        [DisplayName("帐户")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string UserAccount { get; set; }

        /// <summary>
        /// 用户类型(1:拣货员;2:配送员;3:装箱员)
        /// </summary>
        [DataMember]
        [DisplayName("职位")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int UserType { get; set; }

        /// <summary>
        /// 是否为组长(0:不是;1:是)
        /// </summary>
        [DataMember]
        [DisplayName("是否为组长")]
        public int IsMaster { get; set; }

        /// <summary>
        /// 所属仓库(Warehouse.WID)
        /// </summary>
        [DataMember]
        [DisplayName("所属机构ID")]
        public int WID { get; set; }

        /// <summary>
        /// 用户联络手机号码(手机号码注册同登录帐号)
        /// </summary>
        [DataMember]
        [DisplayName("联络手机号码")]
        public string UserMobile { get; set; }




        /// <summary>
        /// 是否冻结(0:未冻结;1:已冻结)
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public int IsFrozen { get; set; }



        /// <summary>
        /// 是否删除(0:未删除;1:已删除)
        /// </summary>
        [DataMember]
        [DisplayName("是否删除")]
        public int IsDeleted { get; set; }







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
        /// 最后修改时间
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间")]

        public DateTime ModifyTime { get; set; }

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
        /// 仓库名称
        /// </summary>
        [DataMember]
        [DisplayName("所属机构")]
        public string WName { get; set; }

        /// <summary>
        /// 状态
        /// </summary>
        [DataMember]
        [DisplayName("状态")]
        public string StatusStr { get; set; }

        /// <summary>
        /// 职位
        /// </summary>
        [DataMember]
        [DisplayName("职位")]
        public string UserTypeStr { get; set; }

        #endregion

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 仓库集合
        /// </summary>
        public SelectList WCList { get; set; }

        public void BindWCList()
        {


            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseTableListRequest()
            {
                PageIndex = 1,
                PageSize = 10000,
                ParentWID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                IsFreeze = 0
            });

            if (resp != null && resp.Flag == 0)
            {
                WCList = new SelectList(resp.Data.ItemList, "WID", "WName", true);
            }
            else
            {
                WCList = null;
            }
        }

        /// <summary>
        /// 职位集合
        /// </summary>
        public SelectList UserTypeList { get; set; }

        public void BindUserTypeList()
        {
            UserTypeList = new SelectList(new[] { new { Text = "拣货员", Value = 1 }, new { Text = "配送员", Value = 2 }, new { Text = "装箱员", Value = 3 }, new { Text = "采购员", Value = 4 } }, "Value", "Text", true);
        }

        /// <summary>
        /// 是否集合
        /// </summary>
        public SelectList IsMasterList { get; set; }

        public void BindIsMasterList()
        {
            IsMasterList = new SelectList(new[] { new { Text = "不是", Value = 0 }, new { Text = "是", Value = 1 } }, "Value", "Text", true);
        }

        #region 获取数据
        /// <summary>
        /// 获取数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public WarehouseEmpModel GetWarehouseEmpData(string id)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpGetRequest()
            {
                EmpID = int.Parse(id)
            });

            WarehouseEmpModel model = AutoMapperHelper.MapTo<WarehouseEmpModel>(resp.Data);

            return model;
        }
        #endregion

        #region 删除数据
        /// <summary>
        /// 删除数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public object DeleteWarehouseEmp(string ids)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpDelRequest()
            {
                EmpID = ids,
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

        #region 冻结数据
        /// <summary>
        /// 冻结数据
        /// </summary>
        /// <param name="id">主键</param>
        /// <returns>对象</returns>
        public int FrozenWarehouseEmp(string ids, int frozen)
        {
            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpIsFrozenRequest()
            {
                EmpID = ids,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName,
                IsFrozen = frozen

            });

            return resp.Data;
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
        public string GetWarehouseEmpModelPageData(int pageIndex, int pageSize, string orderField)
        {

            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();

                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWarehouseEmpTableListRequest()
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    EmpName = conditionDict.ContainsKey("EmpName") ? Utils.NoHtml(conditionDict["EmpName"].ToString()) : null,
                    UserAccount = conditionDict.ContainsKey("UserAccount") ? Utils.NoHtml(conditionDict["UserAccount"].ToString()) : null,
                    IsFrozen = conditionDict.ContainsKey("IsFrozen") ? Utils.NoHtml(conditionDict["IsFrozen"].ToString()) : null,
                    UserType = conditionDict.ContainsKey("UserType") ? Utils.NoHtml(conditionDict["UserType"].ToString()) : null
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