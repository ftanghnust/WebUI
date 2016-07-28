using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK;
using Frxs.Platform.Utility.Map;
using System.Runtime.Serialization;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using Frxs.Platform.Utility.Json;
using Frxs.Platform.Utility.Log;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class VendorTypeModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// 供应商分类ID
        /// </summary>
        [DataMember]
        [DisplayName("供应商分类ID")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int VendorTypeID { get; set; }

        /// <summary>
        /// 供应商分类名称
        /// </summary>
        [DataMember]
        [DisplayName("供应商分类名称")]
        [Required(ErrorMessage = "{0}不能为空")]
        public string VendorTypeName { get; set; }

        /// <summary>
        /// 是否删除(0:未删除;1:已删除);
        /// </summary>
        [DataMember]
        [DisplayName("是否删除(0:未删除;1:已删除);")]
        [Required(ErrorMessage = "{0}不能为空")]
        public int IsDeleted { get; set; }

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

        /// <summary>
        /// 最后修改时间
        /// </summary>
        [DataMember]
        [DisplayName("最后修改时间")]
        [Required(ErrorMessage = "{0}不能为空")]
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

        #endregion

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetVendorTypePageData(int pageIndex, int pageSize, string sort)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                Dictionary<string, object> conditionDict = base.PrePareFormParam();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductVendorTypeListGetRequest()
                {
                    IsDeleted = 0,
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    SortBy = sort
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

        public VendorTypeModel GetVendorType(int id)
        {
            try
            {
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductVendorTypeGetRequest()
                {
                    VendorTypeID = id
                });
                var model = AutoMapperHelper.MapTo<VendorTypeModel>(resp.Data);
                return model;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ResultData SaveVendorType(VendorTypeModel model)
        {
            try
            {
                int flag = model.VendorTypeID.Equals(0) ? 0 : 1;
                var serviceCenter = WorkContext.CreateProductSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductVendorTypeSaveRequest()
                {
                    Flag = flag,
                    VendorTypeID = model.VendorTypeID,
                    VendorTypeName = model.VendorTypeName
                });
                if (resp.Flag == 0)
                {
                    return new ResultData()
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功"
                    };
                }
                else
                {
                    return new ResultData()
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    };
                }
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                return new ResultData()
                {
                    Flag = ConstDefinition.FLAG_EXCEPTION,
                    Info = string.Format("出现异常：{0}", ex.Message)
                };
            }
        }

        public int DeleteVendorType(string ids)
        {
            var vendorTypeIDList = ids.Split(',').ToArray().ToList();
            var ServiceCenter = WorkContext.CreateProductSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductVendorTypeDelRequest()
            {
                VendorTypeIDList = vendorTypeIDList.Select(x => int.Parse(x)).ToList()
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                result = resp.Data;
            }
            return result;
        }
    }
}