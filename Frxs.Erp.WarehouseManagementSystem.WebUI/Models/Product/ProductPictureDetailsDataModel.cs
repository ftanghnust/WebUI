using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Product
{
    public class ProductPictureDetailsDataModel
    {
        /// <summary>
        /// 是否是主图标志
        /// </summary>
        public int IsBaseProductPicture { get; set; }

        /// <summary>
        ///商品图片列表
        /// </summary>
        public List<FrxsErpProductProductsPictureDetailGetResp.ProductsPictureDetail> ProductsPictureDetailList { get; set; }
    }


}