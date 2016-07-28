using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Product
{
    /// <summary>
    /// 商品图文详情
    /// </summary>
    public class ProductsDescriptionModel : BaseModel
    {

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }


        #region 模型

        /// <summary>
        /// 母商品编号
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 母商品编号
        /// </summary>
        public int BaseProductId { get; set; }

        /// <summary>
        /// 商品详情描述
        /// </summary>
        public string Description { get; set; }


        /// <summary>
        /// 商品详情图片
        /// </summary>
        public IList<FrxsErpProductProductGetResp.ProductsDescriptionPicture> ProductsDescriptionPictureList { get; set; }

        #endregion

        /// <summary>
        /// 电商商品图文表ProductsDescriptionPicture实体类
        /// </summary>
        public class ProductsDescriptionPicture
        {
            #region 模型

            /// <summary>
            /// 原图路径
            /// </summary>
            public string ImageUrlOrg { get; set; }

            /// <summary>
            /// zip为400*400的图路径
            /// </summary>
            public string ImageUrl400x400 { get; set; }

            /// <summary>
            /// zip为200*200的图路径
            /// </summary>

            public string ImageUrl200x200 { get; set; }

            /// <summary>
            /// zip为120*120的图路径
            /// </summary>

            public string ImageUrl120x120 { get; set; }

            /// <summary>
            /// zip为60*60的图路径
            /// </summary>

            public string ImageUrl60x60 { get; set; }

            /// <summary>
            /// 排序(1,2,3..)
            /// </summary>

            public int OrderNumber { get; set; }


            #endregion
        }

    }


}