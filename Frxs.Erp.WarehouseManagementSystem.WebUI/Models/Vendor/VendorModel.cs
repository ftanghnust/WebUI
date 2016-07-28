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
    public class VendorModel : BaseModel
    {

        /// <summary>
        /// 获取当前登录用户的信息
        /// </summary>
        public UserIdentity UserIdentity
        {
            get
            {
                return WorkContext.UserIdentity;
            }
        }

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }

        /// <summary>
        /// 
        /// </summary>
        private Vendor reqModel = new Vendor();

        /// <summary>
        /// 增加请求对象
        /// </summary>
        public Vendor ReqModel
        {
            get { return reqModel; }
            set { reqModel = value; }
        }

        /// <summary>
        /// 获取分页数据
        /// </summary>
        /// <param name="pageIndex">当前页码</param>
        /// <param name="pageSize">页容量</param>
        /// <param name="orderField">排序字段</param>
        /// <returns>json格式字符串</returns>
        public string GetVendorPageData(VendorQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                Frxs.Erp.ServiceCenter.Product.SDK.IApiClient serviceCenter = WorkContext.CreateProductSdkClient();                
                var req = AutoMapperHelper.MapTo<Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductVendorTableListRequest>(cpm);
                req.PageIndex = cpm.page;
                req.PageSize = cpm.rows;
                req.SortBy = cpm.sort;
                req.WID = WorkContext.CurrentWarehouse.Parent.WarehouseId;//当前仓库ID

                var resp = serviceCenter.Execute(req);
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
                
        /// <summary>
        /// 获取当前供应商信息
        /// </summary>
        /// <param name="vendorID"></param>
        /// <returns></returns>
        public FrxsErpProductVendorSaveRequest.Vendor GetModelByID(int vendorID)
        {
            Frxs.Erp.ServiceCenter.Product.SDK.IApiClient ServiceCenter = WorkContext.CreateProductSdkClient();
            var resp = ServiceCenter.Execute(new FrxsErpProductVendorGetByIDRequest()
            {
                VendorID = vendorID
            });
            if (resp == null || resp.Data == null)
            {
                return null;
            }
            FrxsErpProductVendorSaveRequest.Vendor model = AutoMapperHelper.MapTo<FrxsErpProductVendorSaveRequest.Vendor>(resp.Data);
            return model;
        }
    }
    

}