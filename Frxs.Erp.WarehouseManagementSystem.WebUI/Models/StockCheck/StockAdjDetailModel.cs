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
    public class StockAdjDetailProductQuery : BasePageModel
    {
        public string SKU { get; set; }
        public string ProductName { get; set; }
        public string BarCode { get; set; }

        public int? SubWid { get; set; }
    }

    public class StockAdjDetailProduct
    {
        // 摘要:
        //     规格属性 由3个规格属性拼接而成
        public string Attributes { get; set; }
        //
        // 摘要:
        //     条码
        public string BarCode { get; set; }
        //
        // 摘要:
        //     包装数
        public decimal BigPackingQty { get; set; }
        //
        // 摘要:
        //     大(配送)单位(冗余设计 选中配送单位时,同步该表,没有时该值为1)
        public string BigUnit { get; set; }
        //
        // 摘要:
        //     采购价格
        public decimal BuyPrice { get; set; }
        //
        // 摘要:
        //     分类
        public int CategoryId1 { get; set; }
        //
        // 摘要:
        //     分类
        public int CategoryId2 { get; set; }
        //
        // 摘要:
        //     分类
        public int CategoryId3 { get; set; }
        //
        public string CategoryName1 { get; set; }
        //
        public string CategoryName2 { get; set; }
        //
        public string CategoryName3 { get; set; }
        //
        // 摘要:
        //     库存数量 这个值需要掉专门的接口查出来，暂时取不到
        public decimal Num { get; set; }
        //
        // 摘要:
        //     价格 (最小单位配送价*包装数)
        public decimal Price { get; set; }
        //
        // 摘要:
        //     商品ID
        public int ProductId { get; set; }
        //
        // 摘要:
        //     商品名称
        public string ProductName { get; set; }
        //
        // 摘要:
        //     商品名称
        public string ProductName2 { get; set; }
        //
        // 摘要:
        //     配送价格
        public decimal SalePrice { get; set; }
        //
        // 摘要:
        //     货位编号(同一个仓库不能重复)
        public string ShelfCode { get; set; }
        //
        // 摘要:
        //     货架ID(WProducts.ShelfID、Shelf.ShelfID)
        public int? ShelfID { get; set; }
        //
        // 摘要:
        //     平台费率
        public decimal ShopAddPerc { get; set; }
        //
        // 摘要:
        //     门店库存单位原积分
        public decimal ShopPoint { get; set; }
        //
        // 摘要:
        //     ERP编码
        public string SKU { get; set; }
        //
        // 摘要:
        //     库存单位
        public string Unit { get; set; }
        //
        // 摘要:
        //     主供应商名称
        public string VendorName { get; set; }
        //
        // 摘要:
        //     仓库商品ID(WProducts.WProductID)
        public long WProductId { get; set; }
        //
        // 摘要:
        //     仓库商品状态(0:已移除1:正常;2:淘汰;3:冻结;) ;淘汰商品和冻结商品不能销售;加入或重新加入时为正常；移除后再加入时为正常
        public int WStatus { get; set; }
        //
        // 摘要:
        //     仓库商品状态的文本描述
        public string WStatusStr { get; set; }
    }

    public class StockAdjDetailQuery : BasePageModel
    {
        public string AdjID { get; set; }
        public string SearchValue { get; set; }
    }

    /// <summary>
    /// 明细统计类
    /// </summary>
    public class StockAdjDetailCalculate : BaseModel
    {
        /// <summary>
        /// 统计总数量
        /// </summary>
        public decimal AdjQtySum { get; set; }
        /// <summary>
        /// 统计总金额
        /// </summary>
        public decimal AdjAmtSum { get; set; }

    }


    public class StockAdjDetailModel : BaseModel
    {
        #region Model
        /// <summary>
        /// 标志位 0表示新增，1表示修改
        /// </summary>
        public int Flag { get; set; }
        /// <summary>
        /// 主键(仓库ID+GUID)
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        public int WID { get; set; }
        /// <summary>
        /// 盘亏盘赢调整编号(StockAdj.AdjID)
        /// </summary>
        public string AdjID { get; set; }
        /// <summary>
        /// 商品编号(Prouct.ProductID)
        /// </summary>
        public int ProductId { get; set; }
        /// <summary>
        /// 商品SKU(商品编码)
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// 描述商品名称(Product.ProductName)
        /// </summary>
        public string ProductName { get; set; }
        /// <summary>
        /// 商品的国际条码
        /// </summary>
        public string BarCode { get; set; }
        /// <summary>
        /// 商品图片用于移动端(Products.ImageUrl200*200)
        /// </summary>
        public string ProductImageUrl200 { get; set; }
        /// <summary>
        /// 商品图片用于PC端(Products.ImageUrl400*400)
        /// </summary>
        public string ProductImageUrl400 { get; set; }
        /// <summary>
        /// 调整单位(j库存单位,预留)
        /// </summary>
        public string AdjUnit { get; set; }
        /// <summary>
        /// 调整单位包装数(固定为1) 2016-6-20 按照会议要求，类型统一改成decimal
        /// </summary>
        public decimal AdjPackingQty { get; set; }
        /// <summary>
        /// 调整数量
        /// </summary>
        public decimal AdjQty { get; set; }
        /// <summary>
        /// 库存单位(WProducts.Unit)
        /// </summary>
        public string Unit { get; set; }
        /// <summary>
        /// 库存单位数量(=AdjQty*AdjPackingQty)
        /// </summary>
        public decimal UnitQty { get; set; }
        /// <summary>
        /// (库存单位)采购单价(=WProducts.SalePrice)
        /// </summary>
        public decimal BuyPrice { get; set; }
        /// <summary>
        /// (库存单位)配送单价(=WProductsBuy.BuyPrice)
        /// </summary>
        public decimal SalePrice { get; set; }
        /// <summary>
        /// 调整金额(=UnitQty*BuyPrice)
        /// </summary>
        public decimal AdjAmt { get; set; }
        /// <summary>
        /// 商品主供应商(=WProductsBuy.VendorID)
        /// </summary>
        public int VendorID { get; set; }
        /// <summary>
        /// 主供应商编号(Vendor.VendorCode)
        /// </summary>
        public string VendorCode { get; set; }
        /// <summary>
        /// 主供应商名称(Vendor.VendorName)
        /// </summary>
        public string VendorName { get; set; }
        /// <summary>
        /// 盘点时该单位时的子机构的库存(StockQty.StockQty)
        /// </summary>
        public decimal StockQty { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
        /// <summary>
        /// 序号(输入的序号,每一个单据从1开始)
        /// </summary>
        public int SerialNumber { get; set; }
        /// <summary>
        /// 盘点单号(StockCheck.StockCheckID)
        /// </summary>
        public string StockCheckID { get; set; }
        /// <summary>
        /// 盘点明细ID(StockCheckDetails.ID)
        /// </summary>
        public string StockCheckDetailsID { get; set; }
        /// <summary>
        /// 盘点人员ID
        /// </summary>
        public int? CheckUserID { get; set; }
        /// <summary>
        /// 盘点人员姓名
        /// </summary>
        public string CheckUserName { get; set; }
        /// <summary>
        /// 盘点库存数量(StockCheckDetails.CheckUnitQty)
        /// </summary>
        public decimal? CheckUnitQty { get; set; }
        /// <summary>
        /// 基本分类一级分类ID(Category.CategoryId)
        /// </summary>
        public int CategoryId1 { get; set; }
        /// <summary>
        /// 基本分类一级分类名称
        /// </summary>
        public string CategoryId1Name { get; set; }
        /// <summary>
        /// 基本分类二级分类ID(Category.CategoryId)
        /// </summary>
        public int CategoryId2 { get; set; }
        /// <summary>
        /// 基本分类二级分类名称
        /// </summary>
        public string CategoryId2Name { get; set; }
        /// <summary>
        /// 基本分类三级分类ID(Category.CategoryId)
        /// </summary>
        public int CategoryId3 { get; set; }
        /// <summary>
        /// 基本分类三级分类名称
        /// </summary>
        public string CategoryId3Name { get; set; }
        /// <summary>
        /// 运营一级分类ID(ShopCategory.ShopCategoryId)
        /// </summary>
        public int ShopCategoryId1 { get; set; }
        /// <summary>
        /// 运营一级分类名称
        /// </summary>
        public string ShopCategoryId1Name { get; set; }
        /// <summary>
        /// 运营二级分类ID(ShopCategory.ShopCategoryId)
        /// </summary>
        public int ShopCategoryId2 { get; set; }
        /// <summary>
        /// 运营二级分类名称
        /// </summary>
        public string ShopCategoryId2Name { get; set; }
        /// <summary>
        /// 运营三级分类ID(ShopCategory.ShopCategoryId)
        /// </summary>
        public int ShopCategoryId3 { get; set; }
        /// <summary>
        /// 运营三级分类名称
        /// </summary>
        public string ShopCategoryId3Name { get; set; }
        /// <summary>
        /// 品牌ID
        /// </summary>
        public int? BrandId1 { get; set; }
        /// <summary>
        /// 品牌ID名称
        /// </summary>
        public string BrandId1Name { get; set; }
        /// <summary>
        /// 子品牌ID
        /// </summary>
        public int? BrandId2 { get; set; }
        /// <summary>
        /// 子品牌名称
        /// </summary>
        public string BrandId2Name { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int WarehouseId { get; set; }
        #endregion Model

        public string GetStockAdjDetailPageData(StockAdjDetailQuery cpm)
        {
            string jsonStr = "[]";
            try
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailListGetRequest()
                {
                    PageIndex = cpm.page,
                    PageSize = cpm.rows,
                    SortBy = cpm.sort,
                    AdjID = cpm.AdjID,
                    SearchValue = cpm.SearchValue,
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







        public int DeleteStockAdjDetail(string ids)
        {
            var IdList = ids.Split(',').ToList();
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailDelRequest()
            {
                Ids = IdList,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                result = resp.Data;
            }
            return result;
        }

        public int ClearStockAdjDetail(string id)
        {
            var ServiceCenter = WorkContext.CreateOrderSdkClient();
            var resp = ServiceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailDelRequest()
            {
                AdjID = id,
                Ids = new List<string>(),
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });
            int result = 0;
            if (resp != null && resp.Flag == 0)
            {
                result = resp.Data;
            }
            return result;
        }

        public StockAdjDetailModel GetStockAdjDetail(string id)
        {
            StockAdjDetailModel model;
            if (!String.IsNullOrEmpty(id))
            {
                var serviceCenter = WorkContext.CreateOrderSdkClient();
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailGetRequest()
                {
                    ID = id.ToString(),
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
                });
                if (resp != null && resp.Flag == 0)
                {
                    model = AutoMapperHelper.MapTo<StockAdjDetailModel>(resp.Data);
                }
                else
                {
                    model = null;
                }
            }
            else
            {
                model = null;
            }
            return model;
        }

        public ResultData SaveStockAdjDetail(StockAdjDetailModel model, int UserId, string UserName)
        {
            ResultData result;
            var serviceCenter = WorkContext.CreateOrderSdkClient();
            if (!String.IsNullOrEmpty(model.ID))
            {
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailSaveRequest()
                {
                    Flag = 1,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    ID = model.ID,
                    AdjID = model.AdjID,
                    ProductId = model.ProductId,
                    SKU = model.SKU,
                    ProductName = model.ProductName,
                    BarCode = model.BarCode,
                    AdjUnit = model.AdjUnit,
                    AdjPackingQty = model.AdjPackingQty,
                    AdjQty = model.AdjQty,
                    BuyPrice = model.BuyPrice,
                    UnitQty = model.UnitQty,
                    AdjAmt = model.AdjAmt,
                    StockQty = model.StockQty,
                    SalePrice = model.SalePrice,
                    Remark = model.Remark,
                    Unit = model.AdjUnit,
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    VendorID = model.VendorID,
                    VendorCode = model.VendorCode,
                    VendorName = model.VendorName,
                    //StockCheckID = String.Empty,
                    //StockCheckDetailsID = String.Empty,
                    //CheckUserID = null,
                    //CheckUserName = null,
                    //CheckUnitQty = null,
                    CategoryId1 = model.CategoryId1,
                    CategoryId2 = model.CategoryId2,
                    CategoryId3 = model.CategoryId3,
                    CategoryId1Name = model.CategoryId1Name,
                    CategoryId2Name = model.CategoryId2Name,
                    CategoryId3Name = model.CategoryId3Name
                });
                if (resp == null)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "保存数据失败"
                    };

                }
                else if (resp.Flag == 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功"
                    };
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    };
                }
            }
            else
            {
                var id = GetStockAdjDetailId();
                if (id.Equals(String.Empty))
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "获取ID失败",
                        Data = null
                    };
                    return result;
                }
                var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderStockAdjDetailSaveRequest()
                {
                    Flag = 0,
                    ID = id,
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    AdjID = model.AdjID,
                    ProductId = model.ProductId,
                    SKU = model.SKU,
                    ProductName = model.ProductName,
                    BarCode = model.BarCode,
                    AdjUnit = model.AdjUnit,
                    AdjPackingQty = model.AdjPackingQty,
                    AdjQty = model.AdjQty,
                    BuyPrice = model.BuyPrice,
                    UnitQty = model.UnitQty,
                    AdjAmt = model.AdjAmt,
                    StockQty = model.StockQty,
                    SalePrice = model.SalePrice,
                    Remark = model.Remark,
                    Unit = model.AdjUnit,
                    WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    VendorID = model.VendorID,
                    VendorCode = model.VendorCode,
                    VendorName = model.VendorName,
                    //StockCheckID = String.Empty,
                    //StockCheckDetailsID = String.Empty,
                    //CheckUserID = null,
                    //CheckUserName = null,
                    //CheckUnitQty = null,
                    CategoryId1 = model.CategoryId1,
                    CategoryId2 = model.CategoryId2,
                    CategoryId3 = model.CategoryId3,
                    CategoryId1Name = model.CategoryId1Name,
                    CategoryId2Name = model.CategoryId2Name,
                    CategoryId3Name = model.CategoryId3Name
                });

                if (resp == null)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = "数据读取错误"
                    };

                }
                else if (resp.Flag == 0)
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_SUCCESS,
                        Info = "操作成功"
                    };
                }
                else
                {
                    result = new ResultData
                    {
                        Flag = ConstDefinition.FLAG_FAIL,
                        Info = resp.Info
                    };
                }
            }
            return result;
        }

        public string GetStockAdjDetailId()
        {
            var adjDetailId = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString() + Guid.NewGuid().ToString("N");
            return adjDetailId;
        }
    }

    public class StockAdjDetailImportModel
    {
        /// <summary>
        /// 明细表和扩展表 主键(仓库ID+GUID)
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// Erp编码/商品编码(手工输入)
        /// </summary>
        public string SKU { get; set; }
        /// <summary>
        /// 调整数量
        /// </summary>
        public decimal AdjQty { get; set; }
        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }
    }

}