using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Excel;
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.BuyOrderPre
{
    public class InsertPriceChangeController : BaseController
    {
        /// <summary>
        /// 进货价调整单
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520225)]
        public ActionResult InsertPriceOrder()
        {
            return View();
        }

        /// <summary>
        /// 进货价调整单-添加编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult InsertPriceOrderAddOrEdit()
        {
            return View();
        }

        /// <summary>
        /// 费率积分调整单
        /// </summary>
        /// <returns></returns>
        [AuthorizeMenuFilter(520226)]
        public ActionResult PlatformOrder()
        {
            return View();
        }

        /// <summary>
        /// 费率积分调整单-添加编辑
        /// </summary>
        /// <returns></returns>
        public ActionResult PlatformOrderAddOrEdit()
        {
            return View();
        }


        /// <summary>
        /// 导入 进货价
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportInsertPrice()
        {
            return View();
        }

        /// <summary>
        /// 导入 费率积分
        /// </summary>
        /// <returns></returns>
        public ActionResult ImportPlatform()
        {
            return View();
        }



        /// <summary>
        /// 导出EXCEL-费率积分调整单
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelPlatform(int ajdId)
        {
            var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjPriceGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                AdjID = ajdId
            });

            if (resp != null && resp.Flag == 0)
            {
                var details = resp.Data.Details;
                var fileName = "费率积分调整单_" + ajdId + ".xls";

                var dataTable = new DataTable();
                dataTable.Columns.Add("商品编码");
                dataTable.Columns.Add("商品名称");
                dataTable.Columns.Add("库存单位");
                dataTable.Columns.Add("包装数");
                dataTable.Columns.Add("原门店积分");
                dataTable.Columns.Add("门店积分");
                dataTable.Columns.Add("原绩效分率");
                dataTable.Columns.Add("绩效分率");
                dataTable.Columns.Add("原物流费率");
                dataTable.Columns.Add("物流费率");
                dataTable.Columns.Add("原仓储费率");
                dataTable.Columns.Add("仓储费率");

                foreach (var item in details)
                {
                    var newRow = dataTable.NewRow();
                    newRow["商品编码"] = item.SKU;
                    newRow["商品名称"] = item.ProductName;
                    newRow["库存单位"] = item.Unit;
                    newRow["包装数"] = item.PackingQty;
                    newRow["原门店积分"] = item.OldShopPoint;
                    newRow["门店积分"] = item.ShopPoint;
                    newRow["原绩效分率"] = item.OldBasePoint;
                    newRow["绩效分率"] = item.BasePoint;
                    newRow["原物流费率"] = item.OldVendorPerc1;
                    newRow["物流费率"] = item.VendorPerc1;
                    newRow["原仓储费率"] = item.OldVendorPerc2;
                    newRow["仓储费率"] = item.VendorPerc2;
                    
                    dataTable.Rows.Add(newRow);
                }

                byte[] byteArr = NpoiExcelhelper.ExportExcel
                    (
                        dataTable,
                        5000,
                        Server.MapPath("UploadFile"),
                        fileName
                    );

                return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
            }

            return Content("找不到单据信息,单据可能已被删除");
        }


        /// <summary>
        /// 导出EXCEL-进货价调整单
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelInsertPrice(int ajdId)
        {
            var resp = this.ErpProductSdkClient.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductAdjPriceGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                AdjID = ajdId
            });

            if (resp != null && resp.Flag == 0)
            {
                var details = resp.Data.Details;
                var fileName = "进货价调整单_" + ajdId + ".xls";

                var dataTable = new DataTable();
                dataTable.Columns.Add("商品编码");
                dataTable.Columns.Add("商品名称");
                dataTable.Columns.Add("库存单位");
                dataTable.Columns.Add("包装数");
                dataTable.Columns.Add("原采购价");
                dataTable.Columns.Add("采购价");
                dataTable.Columns.Add("国际条码");

                foreach (var item in details)
                {
                    var newRow = dataTable.NewRow();
                    newRow["商品编码"] = item.SKU;
                    newRow["商品名称"] = item.ProductName;
                    newRow["库存单位"] = item.Unit;
                    newRow["包装数"] = item.PackingQty;
                    newRow["原采购价"] = item.OldPrice;
                    newRow["采购价"] = item.Price;
                    newRow["国际条码"] = item.BarCode;
                    dataTable.Rows.Add(newRow);
                }

                byte[] byteArr = NpoiExcelhelper.ExportExcel
                    (
                        dataTable,
                        5000,
                        Server.MapPath("UploadFile"),
                        fileName
                    );

                return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
            }

            return Content("找不到单据信息,单据可能已被删除");
        }
        

    }
}
