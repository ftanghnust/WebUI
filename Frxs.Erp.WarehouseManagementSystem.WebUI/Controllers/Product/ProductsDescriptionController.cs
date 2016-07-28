using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Product;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.Product
{
    public class ProductsDescriptionController : Controller
    {

        /// <summary>
        /// 图文详情信息查看
        /// </summary>
        /// <param name="productid">商品编号</param>
        /// <returns></returns>
        public ActionResult ProductsDescriptionView(int productid)
        {
            if (productid <= 0)
            {
                return null;
            }

            ProductsDescriptionModel model = new ProductsDescriptionModel();
            FrxsErpProductProductsDescriptionGetRequest getdata = new FrxsErpProductProductsDescriptionGetRequest()
            {
                ProductId = productid
            };
            var resp = WorkContext.CreateProductSdkClient().Execute(getdata);
            if (resp != null && resp.Data != null)
            {
                model.ProductId = resp.Data.ProductId;
                model.BaseProductId = resp.Data.BaseProductId;
                model.Description = resp.Data.Description;
                model.ProductsDescriptionPictureList = Frxs.Platform.Utility.Map.AutoMapperHelper.MapToList<FrxsErpProductProductsDescriptionGetResp.ProductsDescriptionPicture, FrxsErpProductProductGetResp.ProductsDescriptionPicture>(resp.Data.ProductsDescriptionPicture);
            }
            return View(model);
        }

    }
}
