using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Erp.ServiceCenter.Product.SDK.Request;
using Frxs.Erp.ServiceCenter.Product.SDK.Resp;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Comm;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Enum;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    /// <summary>
    /// 页面显示模型
    /// </summary>
    public class WProductAddModel : BaseModel
    {
        /// <summary>
        ///添加或编辑
        /// </summary>
        public FrxsErpProductWProductsAddOrUpdateRequest.Flag AddOrEdit { get; set; }

        /// <summary>
        /// 页面标题
        /// </summary>
        public string PageTitle { get; set; }


        #region 模型

        /// <summary>
        ///选中配送单位
        /// </summary>
        public int DeliveryUnitID { get; set; }


        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductId { get; set; }


        /// <summary>
        /// ERP编码
        /// </summary>
        [DisplayName("ERP编码")]
        public string Sku { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品副标题
        /// </summary>
        [DisplayName("商品副标题")]
        public string ProductName2 { get; set; }


        /// <summary>
        /// 助记码
        /// </summary>
        [DisplayName("助记码")]
        public string Mnemonic { get; set; }


        /// <summary>
        /// 商品第一品牌
        /// </summary>
        [DisplayName("品牌")]
        public string BrandName { get; set; }

        /// <summary>
        /// 基本分类名称
        /// </summary>
        [DisplayName("基本分类")]
        public string CategoriesName { get; set; }


        /// <summary>
        /// 基本分类名称
        /// </summary>
        [DisplayName("运营分类")]
        public string ShopCategoriesName { get; set; }

        /// <summary>
        /// 进货价 （进价）
        /// </summary>
        [DisplayName("进价")]
        public decimal BuyPrice { get; set; }

        /// <summary>
        /// 建议门店零售价
        /// </summary>
        [DisplayName("建议零售价")]
        public decimal? MarketPrice { get; set; }


        /// <summary>
        ///销售退货类型
        /// </summary>
        [DisplayName("建议零售单位")]
        public string MarketUnit { get; set; }


        /// <summary>
        /// 销售价 （配送价）
        /// </summary>
        [DisplayName("配送价")]
        public decimal? SalePrice { get; set; }


        /// <summary>
        /// 库存单位物流费率(供应商) (%) --（物流费率）
        /// </summary>
        [DisplayName("物流费率")]
        public decimal? VendorPerc1 { get; set; }

        /// <summary>
        /// 库存单位仓储费率(供应商)(%) --（仓储费率）
        /// </summary>
        [DisplayName("仓储费率")]
        public decimal? VendorPerc2 { get; set; }


        /// <summary>
        /// 门店库存单位提点率(%) (平台费率)
        /// </summary>
        [DisplayName("平台费率")]
        public decimal? ShopAddPerc { get; set; }

        /// <summary>
        /// 门店库存单位积分 （门店积分）
        /// </summary>
        [DisplayName("门店积分")]
        public decimal? ShopPoint { get; set; }


        /// <summary>
        /// 库存单位绩效积分（司机及门店绩效分) （绩效积分） 
        /// </summary>
        [DisplayName("绩效积分")]
        public decimal? BasePoint { get; set; }


        /// <summary>
        ///销售退货类型
        /// </summary>
        [DisplayName("是否可退")]
        public string SaleBackFlag { get; set; }


        /// <summary>
        /// 货架号
        /// </summary>
        [DisplayName("货架号")]
        public string ShelfCode { get; set; }


        /// <summary>
        /// 商品状态
        /// </summary>
        [DisplayName("商品状态")]
        public int WStatus { get; set; }


        /// <summary>
        /// 配送单位编号
        /// </summary>
        [DisplayName("配送单位编号")]
        public int BigProductsUnitId { get; set; }

        /// <summary>
        /// 主供应商编号
        /// </summary>
        [DisplayName("主供应商编号")]
        public int VendorID { get; set; }


        /// <summary>
        /// 主供应商名称
        /// </summary>
        [DisplayName("主供应商名称")]
        public string VendorName { get; set; }


        /// <summary>
        /// 主供应商编码
        /// </summary>
        [DisplayName("主供应商编码")]
        public string VendorCode { get; set; }


        /// <summary>
        /// 主供应商编码
        /// </summary>
        [DisplayName("主供应商编码")]
        public string hidVendorCode { get; set; }




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
            [DisplayName("选择配送单位")]
            public int ProductsUnitId { get; set; }

            ///// <summary>
            ///// 即为：Product.ProductID
            ///// </summary>
            //public int ProductId { get; set; }

            /// <summary>
            /// 单位(同一个商品单位不能重复)
            /// </summary>
            [DisplayName("单位")]
            public string Unit { get; set; }

            /// <summary>
            /// 库存单位或者配送单位（前端数据）
            /// </summary>
            [DisplayName("类型")]
            public string UnitTypeName { get; set; }


            /// <summary>
            /// 规格包装
            /// </summary>
            [DisplayName("包装")]
            public string Spec { get; set; }

            /// <summary>
            /// 包装数
            /// </summary>
            [DisplayName("包装数")]
            public decimal PackingQty { get; set; }


            /// <summary>
            /// 单位体积（前端数据）
            /// </summary>
            [DisplayName("商品体积")]
            public decimal UnitVolume { get; set; }

            /// <summary>
            /// 单位重量（前端数据）
            /// </summary>
            [DisplayName("商品重量")]
            public decimal UnitWeight { get; set; }

            /// <summary>
            /// 是否为库存单位(0:不是;1:是;只有一条)
            /// </summary>
            public int IsUnit { get; set; }

            /// <summary>
            /// 是否为默认配送单位(0:不是;1:是)
            /// </summary>
            public int IsSaleUnit { get; set; }


            /// <summary>
            /// 进货价 （进价）
            /// </summary>
            [DisplayName("进价")]
            public decimal? UnitBuyPrice { get; set; }

            /// <summary>
            /// 建议门店零售价
            /// </summary>
            [DisplayName("建议零售价")]
            public decimal? UnitMarketPrice { get; set; }


            /// <summary>
            /// 销售价 （配送价）
            /// </summary>
            [DisplayName("配送价")]
            public decimal? UnitSalePrice { get; set; }


            /// <summary>
            /// 库存单位物流费率(供应商) (%) --（物流费率）
            /// </summary>
            [DisplayName("物流费率")]
            public decimal? UnitVendorPerc1 { get; set; }


            /// <summary>
            /// 物流费金额=物流费率*总金额
            /// </summary>
            [DisplayName("物流费金额")]
            public decimal? UnitVendorPerc1Money { get; set; }


            /// <summary>
            /// 库存单位仓储费率(供应商)(%) --（仓储费率）
            /// </summary>
            [DisplayName("仓储费率")]
            public decimal? UnitVendorPerc2 { get; set; }



            /// <summary>
            /// 仓储费金额=仓储费率*总金额
            /// </summary>
            [DisplayName("仓储费金额")]
            public decimal? UnitVendorPerc2Money { get; set; }


            /// <summary>
            /// 门店库存单位提点率(%) (平台费率)
            /// </summary>
            [DisplayName("平台费率")]
            public decimal? UnitShopAddPerc { get; set; }


            /// <summary>
            /// 平台费金额=平台费率*总金额
            /// </summary>
            [DisplayName("平台费金额")]
            public decimal? UnitShopAddPercMoney { get; set; }

            /// <summary>
            /// 门店库存单位积分 （门店积分）
            /// </summary>
            [DisplayName("门店积分")]
            public decimal? UnitShopPoint { get; set; }

        }


        #endregion



        /// <summary>
        /// 零售单位列表
        /// </summary>
        public SelectList WProductMarketUnitList { get; set; }



        /// <summary>
        /// 网仓商品状态
        /// </summary>
        public SelectList WProductStatusList { get; set; }


        /// <summary>
        /// 商品退货功能列表
        /// </summary>
        public SelectList ProductSaleBackFlagList { get; set; }

        /// <summary>
        /// 绑定数据
        /// </summary>
        public void BindData()
        {
            BindWarehouseSysParams();
            ProductSaleBackFlagList = BindSaleBackFlagList();
            BindSaleBackFlagList();
            WProductStatusList = new SelectList(new[] { new { Text = "正常", Value = ((int)WarehouseEnum.WProductStatus.正常).ToString("d") },
                                                        new { Text = "淘汰", Value =((int)WarehouseEnum.WProductStatus.淘汰).ToString("d") } ,
                                                        new { Text = "冻结", Value = ((int)WarehouseEnum.WProductStatus.冻结).ToString("d") } 
            }, "Value", "Text", true);
        }

        /// <summary>
        /// 销售退货列表
        /// </summary>
        private SelectList BindSaleBackFlagList()
        {
            IList<SelectListItem> items = new List<SelectListItem>();
            var lst = CommModel.GetSysDictDetailList("SaleBackFlag"); //销售退货字典
            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    items.Add(new SelectListItem()
                    {
                        Text = item.DictLabel,
                        Value = item.DictValue
                    });
                }
            }
            return new SelectList(items, "Value", "Text", true);
        }



        /// <summary>
        /// 绑定系统参数信息
        /// </summary>
        private void BindWarehouseSysParams()
        {
            var lst = CommModel.GetWarehouseSysParams(null);
            if (lst != null && lst.Count > 0)
            {
                foreach (var item in lst)
                {
                    if (item.ParamCode.ToUpper() == SysParams.默认仓储费率.ToUpper())
                    {
                        VendorPerc2 = Convert.ToDecimal(item.ParamValue);
                    }
                    if (item.ParamCode.ToUpper() == SysParams.默认物流费率.ToUpper())
                    {
                        VendorPerc1 = Convert.ToDecimal(item.ParamValue);
                    }
                    if (item.ParamCode.ToUpper() == SysParams.默认平台费率.ToUpper())
                    {
                        ShopAddPerc = Convert.ToDecimal(item.ParamValue);
                    }
                    if (item.ParamCode.ToUpper() == SysParams.默认门店积分.ToUpper())
                    {
                        ShopPoint = Convert.ToDecimal(item.ParamValue);
                    }
                    if (item.ParamCode.ToUpper() == SysParams.默认绩效分率.ToUpper())
                    {
                        BasePoint = Convert.ToDecimal(item.ParamValue);
                    }
                }
            }
        }
    }


    /// <summary>
    /// 网仓商品商品编号传递模型
    /// </summary>
    public class WProductViewModel : BaseModel
    {
        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductId { get; set; }

    }



    public class WProductModeNew
    {
        /// <summary>
        ///选中配送单位
        /// </summary>
        public int DeliveryUnitID { get; set; }


        /// <summary>
        /// 商品编号
        /// </summary>
        public int ProductId { get; set; }


        /// <summary>
        /// ERP编码
        /// </summary>
        [DisplayName("ERP编码")]
        public string Sku { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        [DisplayName("商品名称")]
        public string ProductName { get; set; }

        /// <summary>
        /// 商品副标题
        /// </summary>
        [DisplayName("商品副标题")]
        public string ProductName2 { get; set; }


        /// <summary>
        /// 助记码
        /// </summary>
        [DisplayName("助记码")]
        public string Mnemonic { get; set; }


        /// <summary>
        /// 商品第一品牌
        /// </summary>
        [DisplayName("品牌")]
        public string BrandName { get; set; }

        /// <summary>
        /// 基本分类名称
        /// </summary>
        [DisplayName("基本分类")]
        public string CategoriesName { get; set; }


        /// <summary>
        /// 基本分类名称
        /// </summary>
        [DisplayName("运营分类")]
        public string ShopCategoriesName { get; set; }

        /// <summary>
        /// 进货价 （进价）
        /// </summary>
        [DisplayName("进价")]
        public decimal BuyPrice { get; set; }

        /// <summary>
        /// 建议门店零售价
        /// </summary>
        [DisplayName("建议零售价")]
        public decimal MarketPrice { get; set; }


        /// <summary>
        ///销售退货类型
        /// </summary>
        [DisplayName("建议零售单位")]
        public string MarketUnit { get; set; }


        /// <summary>
        /// 销售价 （配送价）
        /// </summary>
        [DisplayName("配送价")]
        public decimal SalePrice { get; set; }


        /// <summary>
        /// 库存单位物流费率(供应商) (%) --（物流费率）
        /// </summary>
        [DisplayName("物流费率")]
        public decimal VendorPerc1 { get; set; }

        /// <summary>
        /// 库存单位仓储费率(供应商)(%) --（仓储费率）
        /// </summary>
        [DisplayName("仓储费率")]
        public decimal VendorPerc2 { get; set; }


        /// <summary>
        /// 门店库存单位提点率(%) (平台费率)
        /// </summary>
        [DisplayName("平台费率")]
        public decimal ShopAddPerc { get; set; }

        /// <summary>
        /// 门店库存单位积分 （门店积分）
        /// </summary>
        [DisplayName("门店积分")]
        public decimal ShopPoint { get; set; }


        /// <summary>
        /// 库存单位绩效积分（司机及门店绩效分) （绩效积分） 
        /// </summary>
        [DisplayName("绩效积分")]
        public decimal BasePoint { get; set; }


        /// <summary>
        ///销售退货类型
        /// </summary>
        [DisplayName("是否可退")]
        public string SaleBackFlag { get; set; }


        /// <summary>
        /// 货架号
        /// </summary>
        [DisplayName("货架号")]
        public string ShelfCode { get; set; }


        /// <summary>
        /// 商品状态
        /// </summary>
        [DisplayName("商品状态")]
        public int WStatus { get; set; }


        /// <summary>
        /// 配送单位编号
        /// </summary>
        [DisplayName("配送单位编号")]
        public int BigProductsUnitId { get; set; }

        /// <summary>
        /// 商品主供应商编号
        /// </summary>
        [DisplayName("商品主供应商编号")]
        public int VendorID { get; set; }

        public FrxsErpProductWProductsAddOrUpdateRequest.Flag AddOrEdit { get; set; }
    }
}