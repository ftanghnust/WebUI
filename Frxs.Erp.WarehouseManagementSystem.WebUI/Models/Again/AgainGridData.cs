/*****************************
* Author:罗涛
*
* Date:2016/4/27 17:12:32
******************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models.Again
{
    public class AgainGridData
    {

        #region 模型
        /// <summary>
        /// 编号(仓库ID+GUID)
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// 申请编号(BuyPreApp.AppID)
        /// </summary>
        public string AppID { get; set; }

        /// <summary>
        /// 仓库ID(Warehouse.WID)
        /// </summary>
        public int WID { get; set; }

        /// <summary>
        /// 商品编号(Prouct.ProductID)
        /// </summary>
        public int ProductId { get; set; }

        /// <summary>
        /// 商品SKU(ERP编码)
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
        /// 申请单位(即采购单位)
        /// </summary>
        public string AppUnit { get; set; }

        /// <summary>
        /// 申请单位包装数(即采购单位包装数)
        /// </summary>
        public decimal PackingQty { get; set; }

        /// <summary>
        /// 申请单位包装数(即采购单位包装数)
        /// </summary>
        public decimal AppPackingQty { get; set; }

        /// <summary>
        /// 申请单位数量(采购单位数量)
        /// </summary>
        public decimal AppQty { get; set; }

        /// <summary>
        /// 采购单位价格(UnitPrice*AppPackingQty)
        /// </summary>
        public decimal Price { get; set; }

        /// <summary>
        /// 采购单位价格(UnitPrice*AppPackingQty)
        /// </summary>
        public decimal AppPrice { get; set; }

        /// <summary>
        /// 库存单位（WProducts.Unit)
        /// </summary>
        public string Unit { get; set; }

        /// <summary>
        /// 库存单位数量(=AppPackingQty*AppQty)
        /// </summary>
        public decimal UnitQty { get; set; }

        /// <summary>
        /// 库存单位价格(WProduct.BuyPrice)
        /// </summary>
        public decimal UnitPrice { get; set; }

        /// <summary>
        /// 采购的总金额(=UnitQty*UnitPrice)
        /// </summary>
        public decimal SubAmt { get; set; }

        /// <summary>
        /// 备注
        /// </summary>
        public string Remark { get; set; }

        /// <summary>
        /// 序号(输入的序号,每一个单据从1开始)
        /// </summary>
        public int SerialNumber { get; set; }

        /// <summary>
        /// 商品主供应商ID(WProductBuy.VendorID)
        /// </summary>
        public int VendorID { get; set; }

        /// <summary>
        /// 商品主供应商编号
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// 商品主供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 最后修改时间
        /// </summary>
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 最后修改用户ID
        /// </summary>
        public int? ModifyUserID { get; set; }

        /// <summary>
        /// 最后修改用户名称
        public string ModifyUserName { get; set; }

        /// <summary>
        /// 库存数（前台页面绑定用-占位字段）
        /// </summary>
        public decimal WStock { get; set; }

        #endregion

        public decimal MaxBuyPrice { get; set; }

    }
}