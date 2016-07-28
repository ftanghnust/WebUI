using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
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
    /// 库存报表控制器
    /// </summary>
    public class StoreReportController : Controller
    {
        //
        // GET: /StoreReport/

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult ReportStockProduct()
        {
            return View();
        }


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
                

                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.PROC_STOREREPORT_REPORT);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        /// <summary>
        /// (4)获取列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetBuyerStockCheckList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_BuyerStockCheck_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }

        #endregion

        public ActionResult ExportExcelStoreReport1()
        {
            
            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.PROC_STOREREPORT_REPORT);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }


            var fileName = "库存状况查询导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            var dataTable = new DataTable();
            dataTable.Columns.Add("公司机构");
            dataTable.Columns.Add("仓库");
            dataTable.Columns.Add("商品分类");
            dataTable.Columns.Add("商品编码");
            dataTable.Columns.Add("商品名称");
            dataTable.Columns.Add("条码");
            dataTable.Columns.Add("计量单位");
            dataTable.Columns.Add("规格");
            dataTable.Columns.Add("主供应商");
            dataTable.Columns.Add("库存数量");
            dataTable.Columns.Add("库存单价");
            dataTable.Columns.Add("库存金额");
            dataTable.Columns.Add("建议零售价");
            dataTable.Columns.Add("零售金额");
            dataTable.Columns.Add("销售未过账数量");
            dataTable.Columns.Add("实时库存数量");
            dataTable.Columns.Add("配送价");
            dataTable.Columns.Add("配送金额");
            dataTable.Columns.Add("总库存金额");
            dataTable.Columns.Add("总零售金额");
            dataTable.Columns.Add("总配送金额");
            dataTable.Columns.Add("商品状态");
            dataTable.Columns.Add("采购员");

            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);

                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();

                    newRow["公司机构"] = item["WName"];
                    newRow["仓库"] = item["SubWName"];
                    newRow["商品分类"] = item["CategoryName"];
                    newRow["商品编码"] = item["ProductCode"];
                    newRow["商品名称"] = item["ProductName"];
                    newRow["条码"] = item["Barcode"];
                    newRow["计量单位"] = item["Unit"];
                    newRow["规格"] = item["Spec"];
                    newRow["主供应商"] = item["VendorName"];
                    newRow["库存数量"] = item["StockQty"];
                    newRow["库存单价"] = item["Price"];
                    newRow["库存金额"] = item["Amount"];
                    newRow["建议零售价"] = item["SalePrice"];
                    newRow["零售金额"] = item["SaleAmount"];
                    newRow["销售未过账数量"] = item["NoStoreQty"];
                    newRow["实时库存数量"] = item["StoreQty"];
                    newRow["配送价"] = item["WholeSalePrice"];
                    newRow["配送金额"] = item["WholeSaleAmount"];
                    newRow["总库存金额"] = item["StockTotalAmount"];
                    newRow["总零售金额"] = item["SaleTotalAmount"];
                    newRow["总配送金额"] = item["WholeSaleTotalAmount"];
                    newRow["商品状态"] = item["Status"];
                    newRow["采购员"] = item["BuyEmpName"];

                    dataTable.Rows.Add(newRow);
                }


            }
            else
            {
                dataTable.Rows.Add(dataTable.NewRow());
            }
            byte[] byteArr = NpoiExcelhelper.ExportExcel
                     (
                         dataTable,
                         50000,
                         Server.MapPath("UploadFile"),
                         fileName
                     );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }


        //页面导出（已放弃采用前端导出）
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult ExportExcelStoreReportToPage()
        {
            var data = System.Web.HttpContext.Current.Request.Form["data"];
            var exportName = System.Web.HttpContext.Current.Request.Form["exportName"];
            data = Utils.UrlDecode(data);
            byte[] array = Encoding.UTF8.GetBytes(data);

            var fileName = exportName + "_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            return File(array, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }



        #region 盘盈盘亏分析
        /// <summary>
        /// 商品盘盈盘亏
        /// </summary>
        /// <returns></returns>
        public ActionResult StockAdj()
        {
            return View();
        }
        /// <summary>
        /// 获取列表-18I 盘赢盘亏统计报表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetListStockadj(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_GetListStockadj_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 盘盈盘亏统计报表导出
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelStockadj()
        {
            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.Proc_GetListStockadj_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            var fileName = "盘盈盘亏统计报表导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";
            
            var dataTable = new DataTable();
            dataTable.Columns.Add("单号");
            dataTable.Columns.Add("商品名称");
            dataTable.Columns.Add("采购员");
            dataTable.Columns.Add("盘盈数量");
            dataTable.Columns.Add("盘盈金额");
            dataTable.Columns.Add("盘亏数量");
            dataTable.Columns.Add("盘亏金额");
            
            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);
                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();
                    newRow["单号"] = item["AdjId"]; 
                    newRow["商品名称"] = item["ProductName"];
                    newRow["采购员"] = item["EmpName"]; 
                    newRow["盘盈数量"] = item["AdjQtyW"]; 
                    newRow["盘盈金额"] = item["AdjAmtW"]; 
                    newRow["盘亏数量"] = item["AdjQtyF"]; 
                    newRow["盘亏金额"] = item["AdjAmtF"]; 
                    
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


        #region 获取列表
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetListMainVendorByPrice(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetProductReportData(true, ConstDefinition.VENDOR_PROCEDURE_STORE);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }

            return Content(jsonStr);
        }
        #endregion
    }
}
