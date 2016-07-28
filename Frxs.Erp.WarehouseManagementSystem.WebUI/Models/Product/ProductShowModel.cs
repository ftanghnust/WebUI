using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 商品显示模型(通过商品编号读取)
    /// </summary>
    public class ProductShowModel : BaseModel
    {
        #region 模型
        /// <summary>
        /// ERP编码
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 商品副标题
        /// </summary>
        public string ProdcutName2 { get; set; }


        /// <summary>
        /// 
        /// </summary>
        public string Mnemonic { get; set; }


        /// <summary>
        /// 商品第一品牌
        /// </summary>
        public string BrandName { get; set; }


        /// <summary>
        /// 一级基本分类
        /// </summary>
        public int? CategoriesId1 { get; set; }

        /// <summary>
        ///二级基本分类
        /// </summary>
        public int? CategoriesId2 { get; set; }


        /// <summary>
        ///三级基本分类
        /// </summary>
        public int? CategoriesId3 { get; set; }


        /// <summary>
        /// 一级运营分类
        /// </summary>
        public int? ShopCategoriesId1 { get; set; }

        /// <summary>
        /// 二级运营分类
        /// </summary>
        public int? ShopCategoriesId2 { get; set; }


        /// <summary>
        /// 三级运营分类
        /// </summary>
        public int? ShopCategoriesId3 { get; set; }


        /// <summary>
        /// 进货价 （进价）
        /// </summary>
        public decimal BuyPrice { get; set; }


        /// <summary>
        /// 销售价 （配送价）
        /// </summary>
        public decimal SalePrice { get; set; }


        /// <summary>
        /// 建议门店零售价
        /// </summary>
        public decimal MarketPrice { get; set; }


        /// <summary>
        /// 门店库存单位提点率(%) (平台费率)
        /// </summary>
        public decimal ShopAddPerc { get; set; }


        /// <summary>
        /// 库存单位物流费率(供应商) (%) --（物流费率）
        /// </summary>
        public decimal VendorPerc1 { get; set; }

        /// <summary>
        /// 库存单位仓储费率(供应商)(%) --（仓储费率）
        /// </summary>
        public decimal VendorPerc2 { get; set; }


        /// <summary>
        /// 门店库存单位积分 （门店积分）
        /// </summary>
        public decimal ShopPoint { get; set; }


        /// <summary>
        /// 库存单位绩效积分（司机及门店绩效分) （绩效积分） 
        /// </summary>
        public decimal BasePoint { get; set; }



        /// <summary>
        ///销售退货类型
        /// </summary>
        public decimal SaleBackFlag { get; set; }



        /// <summary>
        ///  多少天可退只有saleBackFlag=3时，才有值
        /// </summary>
        public int BackDays { get; set; }


        /// <summary>
        /// 货架号
        /// </summary>
        public int ShelfId { get; set; }


        /// <summary>
        /// 货架号
        /// </summary>
        public string ShelfName { get; set; }

        /// <summary>
        /// 商品状态
        /// </summary>
        public int WStatus { get; set; }


        /// <summary>
        /// 配送单位编号
        /// </summary>
        public int WProductsSaleUnitId { get; set; }



        /// <summary>
        /// 商品单位列表
        /// </summary>
        public List<WProductUnit> WProductUnitList { get; set; }


        /// <summary>
        /// 界面单位列表数据
        /// </summary>
        public class WProductUnit
        {
            /// <summary>
            /// 主键编号
            /// </summary>
            public int ProductsUnitId { get; set; }

            /// <summary>
            /// 即为：Product.ProductID
            /// </summary>
            public int ProductId { get; set; }

            /// <summary>
            /// 单位(同一个商品单位不能重复)
            /// </summary>
            public string Unit { get; set; }

            /// <summary>
            /// 包装数
            /// </summary>
            public decimal PackingQty { get; set; }

            /// <summary>
            /// 规格
            /// </summary>
            public string Spec { get; set; }

            /// <summary>
            /// 是否为库存单位(0:不是;1:是;只有一条)
            /// </summary>
            public int IsUnit { get; set; }


            /// <summary>
            /// 是否为配送单位(0:不是;1:是)
            /// </summary>
            public int IsSaleUnit { get; set; }


            /// <summary>
            /// 库存单位或者配送单位（前端数据）
            /// </summary>
            public string UnitTypeName { get; set; }

            /// <summary>
            /// 单位体积（前端数据）
            /// </summary>
            public decimal UnitVolume { get; set; }

            /// <summary>
            /// 单位重量（前端数据）
            /// </summary>
            public decimal UnitWeight { get; set; }





            ///// <summary>
            ///// 进货价 （进价）
            ///// </summary>
            //public decimal BuyPrice { get; set; }


            ///// <summary>
            ///// 销售价 （配送价）
            ///// </summary>
            //public decimal SalePrice { get; set; }


            ///// <summary>
            ///// 建议门店零售价
            ///// </summary>
            //public decimal MarketPrice { get; set; }


            ///// <summary>
            ///// 门店库存单位提点率(%) (平台费率)
            ///// </summary>
            //public decimal ShopAddPerc { get; set; }


            ///// <summary>
            ///// 库存单位物流费率(供应商) (%) --（物流费率）
            ///// </summary>
            //public decimal VendorPerc1 { get; set; }

            ///// <summary>
            ///// 库存单位仓储费率(供应商)(%) --（仓储费率）
            ///// </summary>
            //public decimal VendorPerc2 { get; set; }


            ///// <summary>
            ///// 门店库存单位积分 （门店积分）
            ///// </summary>
            //public decimal ShopPoint { get; set; }


            ///// <summary>
            ///// 库存单位绩效积分（司机及门店绩效分) （绩效积分） 
            ///// </summary>
            //public decimal BasePoint { get; set; }

        }


        #endregion 


    }
}