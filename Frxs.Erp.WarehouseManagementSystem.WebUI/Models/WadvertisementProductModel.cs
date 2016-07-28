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
    [Serializable]
    public class WadvertisementProductModel : BaseModel
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 商品SKU
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 配送单位
        /// </summary>
        public string SaleUnit { get; set; }

        /// <summary>
        /// 包装数量
        /// </summary>
        public decimal PackingQty { get; set; }

        /// <summary>
        /// 配送价格
        /// </summary>
        public decimal SalePrice { get; set; }

        /// <summary>
        /// 最小单位
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 最小单位价格
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 条码
        /// </summary>
        public string BarCode { get; set; }

        public WadvertisementProductModel GetWadvertisementProduct(int wid, int productId)
        {
            //var serviceCenter = WorkContext.CreateProductSdkClient();
            //var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsGetRequest()
            //{
            //    WID = wid,
            //    ProductId = productId
            //});
            //WadvertisementProductModel model = AutoMapperHelper.MapTo<WadvertisementProductModel>(resp.Data);
            //return model;

            var serviceCenter = WorkContext.CreateProductSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductsSaleListGetRequest()
            {
                WID = wid,
                ProductId = productId.ToString()
            });
            if (resp.Data != null && resp.Data.ItemList.Count > 0)
            {
                WadvertisementProductModel model = AutoMapperHelper.MapTo<WadvertisementProductModel>(resp.Data.ItemList[0]);
                return model;
            }
            else
            {
                return null;
            }
        }
    }
}