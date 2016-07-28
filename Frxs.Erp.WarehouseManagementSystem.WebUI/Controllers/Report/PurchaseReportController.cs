using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Frxs.Platform.Utility.Log;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Models;
using Frxs.Platform.Utility;
using System.Data;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility.Excel;


namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// 采购报表控制器
    /// </summary>
    public class PurchaseReportController : Controller
    {
        //
        // GET: /PurchaseReport/

        public ActionResult Index()
        {
            return View();
        }
        #region 页面
        /// <summary>
        /// 采购商品汇总
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyOrderProduct()
        {
            return View();
        }

        /// <summary>
        /// 采购入库采购退货汇总
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyBackProduct()
        {
            return View();
        }

        /// <summary>
        /// 采购入库采购退货明细
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyBackProductDetail()
        {
            return View();
        }


        /// <summary>
        /// 采购员库存查询
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyerStockCheck()
        {
            return View();
        }


        /// <summary>
        /// 主供应商最后一次进价查询
        /// </summary>
        /// <returns></returns>
        public ActionResult MainVendorLastBuyPrice()
        {
            return View();
        }


        /// <summary>
        ///(1)采购入库商品汇总
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetBuyOrderProductList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.ProcGetBuyOrderProductListReport);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// (2) 采购入库采购退货汇总
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetBuyBackProductList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.ProcGetBuyBackProductListReport);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }

            return Content(jsonStr);
        }



        /// <summary>
        ///(3)采购入库采购退货明细
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetBuyBackProductDetailList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.ProcGetBuyBackProductDetailListReport);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }

            return Content(jsonStr);
        }

        #endregion

        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.PRODUCT_PROCEDURE_PURCHASE);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion
        
        #region Execl导出
        ///// <summary>
        ///// 导出EXCEL-采购商品汇总
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult ExportExcelPurchaseReport1()
        //{

        //    string jsonStr = string.Empty;
        //    try
        //    {
        //        jsonStr = new ReportModel().GetReportData(false, ConstDefinition.PRODUCT_PROCEDURE_PURCHASE);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GetInstance().Fatal(ex);
        //        jsonStr = new { info = ex.Message }.ToJsonString();
        //    }

        //    var fileName = "采购商品汇总导出" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

        //    var dataTable = new DataTable();
        //    dataTable.Columns.Add("采购员姓名");
        //    dataTable.Columns.Add("单号");
        //    dataTable.Columns.Add("供应商名称");
        //    dataTable.Columns.Add("数量");
        //    dataTable.Columns.Add("配送金额");
        //    dataTable.Columns.Add("日期");
        //    dataTable.Columns.Add("含税金额");

        //    if (!jsonStr.IsNullOrEmpty())
        //    {
        //        DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);
        //        foreach (DataRow item in dt.Rows)
        //        {
        //            var newRow = dataTable.NewRow();
        //            newRow["采购员姓名"] = item["EmpName"];
        //            newRow["单号"] = item["BuyId"];
        //            newRow["供应商名称"] = item["VendorName"];
        //            newRow["数量"] = item["Qty"];
        //            newRow["配送金额"] = item["Amount"];
        //            newRow["日期"] = item["OrderDate"];
        //            newRow["含税金额"] = item["FaxAmt"];
        //            dataTable.Rows.Add(newRow);
        //        }
        //    }

        //    byte[] byteArr = NpoiExcelhelper.ExportExcel
        //            (
        //                dataTable,
        //                5000,
        //                Server.MapPath("UploadFile"),
        //                fileName
        //            );

        //    return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        //}

        ///// <summary>
        ///// 导出EXCEL-采购入库采购退货汇总
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult ExportExcelPurchaseReport2()
        //{

        //    string jsonStr = string.Empty;
        //    try
        //    {
        //        jsonStr = new ReportModel().GetReportData(false, ConstDefinition.PRODUCT_PROCEDURE_PURCHASE);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GetInstance().Fatal(ex);
        //        jsonStr = new { info = ex.Message }.ToJsonString();
        //    }

        //    var fileName = "采购入库采购退货汇总导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

        //    var dataTable = new DataTable();
        //    dataTable.Columns.Add("单号");
        //    dataTable.Columns.Add("日期");
        //    dataTable.Columns.Add("供应商");
        //    dataTable.Columns.Add("配送金额");
        //    dataTable.Columns.Add("操作员");
        //    dataTable.Columns.Add("备注");
        //    dataTable.Columns.Add("含税金额");

        //    if (!jsonStr.IsNullOrEmpty())
        //    {
        //        DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);


        //        foreach (DataRow item in dt.Rows)
        //        {
        //            var newRow = dataTable.NewRow();
        //            newRow["单号"] = item["BillId"];
        //            newRow["日期"] = item["BillDate"];
        //            newRow["供应商"] = item["VedorName"];
        //            newRow["配送金额"] = item["TotalAmt"];
        //            newRow["操作员"] = item["EmpName"];
        //            newRow["备注"] = item["Remark"];
        //            newRow["含税金额"] = item["FaxAmt"];

        //            dataTable.Rows.Add(newRow);
        //        }


        //    }

        //    byte[] byteArr = NpoiExcelhelper.ExportExcel
        //           (
        //               dataTable,
        //               5000,
        //               Server.MapPath("UploadFile"),
        //               fileName
        //           );

        //    return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        //}

        ///// <summary>
        ///// 导出EXCEL-采购入库采购退货明细
        ///// </summary>
        ///// <returns></returns>
        //public ActionResult ExportExcelPurchaseReport3()
        //{

        //    string jsonStr = string.Empty;
        //    try
        //    {
        //        jsonStr = new ReportModel().GetReportData(false, ConstDefinition.PRODUCT_PROCEDURE_PURCHASE);
        //    }
        //    catch (Exception ex)
        //    {
        //        Logger.GetInstance().Fatal(ex);
        //        jsonStr = new { info = ex.Message }.ToJsonString();
        //    }

        //    var fileName = "采购入库采购退货明细导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

        //    var dataTable = new DataTable();
        //    dataTable.Columns.Add("单号");
        //    dataTable.Columns.Add("供应商");
        //    dataTable.Columns.Add("编码");
        //    dataTable.Columns.Add("名称");
        //    dataTable.Columns.Add("采购数量");
        //    dataTable.Columns.Add("库存单位数量");
        //    dataTable.Columns.Add("单价");
        //    dataTable.Columns.Add("配送金额");
        //    dataTable.Columns.Add("单位");
        //    dataTable.Columns.Add("含税金额");
        //    dataTable.Columns.Add("采购员");

        //    if (!jsonStr.IsNullOrEmpty())
        //    {
        //        DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);


        //        foreach (DataRow item in dt.Rows)
        //        {
        //            var newRow = dataTable.NewRow();
        //            newRow["单号"] = item["BillId"];
        //            newRow["供应商"] = item["VendorName"];
        //            newRow["编码"] = item["Sku"];
        //            newRow["名称"] = item["ProductName"];
        //            newRow["采购数量"] = item["Qty"];
        //            newRow["库存单位数量"] = item["UnitQty"];
        //            newRow["单价"] = item["Price"];
        //            newRow["配送金额"] = item["SubAmt"];
        //            newRow["单位"] = item["Unit"];
        //            newRow["含税金额"] = item["FaxAmt"];
        //            if (item["EmpName"].ToString() == "null")
        //            {
        //                newRow["采购员"] = "";
        //            }
        //            else
        //            {
        //                newRow["采购员"] = item["EmpName"];
        //            }
        //            dataTable.Rows.Add(newRow);
        //        }
        //    }

        //    byte[] byteArr = NpoiExcelhelper.ExportExcel
        //          (
        //              dataTable,
        //              5000,
        //              Server.MapPath("UploadFile"),
        //              fileName
        //          );

        //    return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        //}
        #endregion


        #region
        /// <summary>
        /// 采购员缺货率分析
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleStockout()
        {
            return View();
        }

        /// <summary>
        /// 获取列表-采购员缺货率分析
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetListSaleStockout(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_GetListSaleStockout_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 采购员缺货率分析报表导出
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelSalestockout()
        {

            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.Proc_GetListSaleStockout_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            var fileName = "采购员缺货率分析报表导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var dataTable = new DataTable();
            dataTable.Columns.Add("公司机构");
            dataTable.Columns.Add("商品编码");
            dataTable.Columns.Add("商品名称");
            dataTable.Columns.Add("主条码");
            dataTable.Columns.Add("商品状态");
            dataTable.Columns.Add("商品分类");
            dataTable.Columns.Add("采购员");
            dataTable.Columns.Add("计量单位");
            dataTable.Columns.Add("销售数量");
            dataTable.Columns.Add("销售金额");
            dataTable.Columns.Add("订货数量");
            dataTable.Columns.Add("订货金额");
            dataTable.Columns.Add("缺货数量");
            dataTable.Columns.Add("缺货金额");
            dataTable.Columns.Add("缺货率");
            dataTable.Columns.Add("货区");

            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);
                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();
                    newRow["公司机构"] = item["WName"];
                    newRow["商品编码"] = item["ProductCode"];
                    newRow["商品名称"] = item["ProductName"];
                    newRow["主条码"] = item["BarCode"];
                    newRow["商品状态"] = item["StatusName"];
                    newRow["商品分类"] = item["CategoryName"];
                    newRow["采购员"] = item["BuyEmpName"];
                    newRow["计量单位"] = item["Unit"];
                    newRow["销售数量"] = item["SaleQty"];
                    newRow["销售金额"] = item["SaleAmt"];
                    newRow["订货数量"] = item["BuyQty"];
                    newRow["订货金额"] = item["BuyAmt"];
                    newRow["缺货数量"] = item["LackQty"];
                    newRow["缺货金额"] = item["LackAmt"];
                    newRow["缺货率"] = item["LackRateStr"];
                    newRow["货区"] = item["ShelfAreaName"];
                    dataTable.Rows.Add(newRow);
                }
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

        #endregion
    }
}
