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
    /// 销售报表控制器
    /// </summary>
    public class SalesReportController : Controller
    {
        //
        // GET: /SalesReport/

        public ActionResult Index()
        {
            return View();
        }
        #region 页面
        /// <summary>
        /// 收款单汇总
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleSettle()
        {
            return View();
        }

        /// <summary>
        /// 客户费用单汇总
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleFeeShop()
        {
            return View();
        }

        /// <summary>
        /// 客户销售情况汇总表
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleOrderShop()
        {
            return View();
        }

        /// <summary>
        /// 批发价调整金额差异
        /// </summary>
        /// <returns></returns>
        public ActionResult ProductPriceDiff()
        {
            return View();
        }


        /// <summary>
        /// 客户销售情况汇总表(按照商品编码,商品名称,采购员,商品分类编码,商品分类名称,供应商做一个报表)
        /// </summary>
        /// <returns></returns>
        public ActionResult SaleOrderByProduct()
        {
            return View();
        }




        #endregion
        #region 获取列表
        /// <summary>
        ///(4)获取列表-采购员缺货率分析
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetListSaleStockoutList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_GetListSaleStockout_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// (1)客户销售情况汇总表
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetGetCustomerSaleReportList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_GetCustomerSaleReport_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }


        /// <summary>
        /// (2)门店费用单汇总
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetCustomerExpSumList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_GetCustomerExpSum_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// (3)收款单汇总
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetCollectionSummaryList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_CollectionSummary_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                //jsonStr = new { info = ex.Message }.ToJsonString();
            }
            return Content(jsonStr);
        }

        /// <summary>
        ///(5) 供应商销售情况汇总表导出（通过商品编号）
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetSalesReportByProductList(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_SalesReportByProduct_Report);
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
        /// <summary>
        /// 导出EXCEL-客户销售情况汇总表导出
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelSalesReport1()
        {

            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.Proc_GetCustomerSaleReport_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }


            var fileName = "客户销售情况汇总表导出" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var dataTable = new DataTable();
            dataTable.Columns.Add("门店编码");
            dataTable.Columns.Add("门店名称");
            dataTable.Columns.Add("销售数量");
            dataTable.Columns.Add("销售金额");
            dataTable.Columns.Add("销售平台费用");
            dataTable.Columns.Add("退货数量");
            dataTable.Columns.Add("退货金额");
            dataTable.Columns.Add("退货平台费用");
            dataTable.Columns.Add("合计平台费用");
            dataTable.Columns.Add("合计销售金额");
            dataTable.Columns.Add("合计退货金额");
            dataTable.Columns.Add("线路名称");
            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);


                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();
                    newRow["门店编码"] = item["ShopCode"];
                    newRow["门店名称"] = item["ShopName"];
                    newRow["销售数量"] = item["SaleQty"];
                    newRow["销售金额"] = item["SaleAmount"];
                    newRow["销售平台费用"] = item["SalePoint"];

                    newRow["退货数量"] = item["BackQty"];
                    newRow["退货金额"] = item["BackTotalAmount"];
                    newRow["退货平台费用"] = item["BackPoint"];
                    newRow["合计平台费用"] = item["TotalPoint"];
                    newRow["合计销售金额"] = item["TotalAmount"];
                    newRow["合计退货金额"] = item["TotalAmt"];
                    newRow["线路名称"] = item["LineName"];

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

        /// <summary>
        /// 供应商销售情况汇总表导出（通过商品编号）
        /// </summary>
        /// <returns></returns>
        public ActionResult ExprotExcelDSalesReportByProduct()
        {
            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.Proc_SalesReportByProduct_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            var fileName = "供应商销售情况汇总表导出" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var dataTable = new DataTable();
            dataTable.Columns.Add("商品编码");
            dataTable.Columns.Add("商品名称");
            dataTable.Columns.Add("采购员");
            dataTable.Columns.Add("商品分类");
            dataTable.Columns.Add("供应商");
            dataTable.Columns.Add("销售数量");
            dataTable.Columns.Add("销售金额");
            dataTable.Columns.Add("销售平台费用");
            dataTable.Columns.Add("退货数量");
            dataTable.Columns.Add("退货金额");
            dataTable.Columns.Add("退货平台费用");
            dataTable.Columns.Add("合计平台费用");
            dataTable.Columns.Add("合计销售金额");
            dataTable.Columns.Add("合计退货金额");

            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);


                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();
                    newRow["商品编码"] = item["SKU"];
                    newRow["商品名称"] = item["ProductName"];
                    newRow["采购员"] = item["BuyEmpName"];
                    newRow["商品分类"] = item["CategoryName"];
                    newRow["供应商"] = item["VendorName"];
                    newRow["销售数量"] = item["SaleQty"];
                    newRow["销售金额"] = item["SaleAmount"];
                    newRow["销售平台费用"] = item["SalePoint"];
                    newRow["退货数量"] = item["BackQty"];
                    newRow["退货金额"] = item["BackTotalAmount"];
                    newRow["退货平台费用"] = item["BackPoint"];
                    newRow["合计平台费用"] = item["TotalPoint"];
                    newRow["合计销售金额"] = item["TotalAmount"];
                    newRow["合计退货金额"] = item["TotalAmt"];
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

        /// <summary>
        /// 导出EXCEL-客户费用单汇总
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelSalesReport2()
        {

            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.Proc_GetCustomerExpSum_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }


            var fileName = "门店费用单汇总导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var dataTable = new DataTable();
            dataTable.Columns.Add("单号");
            dataTable.Columns.Add("公司机构");
            dataTable.Columns.Add("开单日期");
            dataTable.Columns.Add("门店编号");
            dataTable.Columns.Add("门店名称");
            dataTable.Columns.Add("录单人员");
            dataTable.Columns.Add("确认人员");

            dataTable.Columns.Add("过帐日期");
            dataTable.Columns.Add("过帐人员");
            dataTable.Columns.Add("项目名称");
            dataTable.Columns.Add("金额");
            dataTable.Columns.Add("备注");
            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);


                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();
                    newRow["单号"] = item["BillId"];
                    newRow["公司机构"] = item["WName"];
                    newRow["开单日期"] = item["FeeDate"];
                    newRow["门店编号"] = item["ShopCode"];
                    newRow["门店名称"] = item["ShopName"];
                    newRow["录单人员"] = item["CreateUserName"];
                    newRow["确认人员"] = item["ConfUserName"];
                    newRow["过帐日期"] = item["PostingTime"];
                    newRow["过帐人员"] = item["ConfUserName"];
                    newRow["项目名称"] = item["FeeName"];
                    newRow["金额"] = item["FeeAmt"];
                    newRow["备注"] = item["Remark"];


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
                       5000,
                       Server.MapPath("UploadFile"),
                       fileName
                   );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }

        /// <summary>
        /// 导出EXCEL-(3)收款单汇总
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelSalesReport3()
        {
            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.Proc_CollectionSummary_Report);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            var fileName = "收款单汇总表导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var dataTable = new DataTable();
            dataTable.Columns.Add("单号");
            dataTable.Columns.Add("公司机构");
            dataTable.Columns.Add("日期");
            dataTable.Columns.Add("过帐日");
            dataTable.Columns.Add("门店编号");
            dataTable.Columns.Add("门店名称");
            dataTable.Columns.Add("录单人员");
            dataTable.Columns.Add("确认人员");
            dataTable.Columns.Add("过帐人员");
            dataTable.Columns.Add("结算单据类型");
            dataTable.Columns.Add("结算单号");
            dataTable.Columns.Add("金额");
            dataTable.Columns.Add("备注");
            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);

                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();
                    newRow["单号"] = item["SettleID"];
                    newRow["公司机构"] = item["WName"];
                    newRow["日期"] = item["SettleTime"];
                    newRow["过帐日"] = item["PostingTime"];
                    newRow["门店编号"] = item["ShopCode"];
                    newRow["门店名称"] = item["ShopName"];
                    newRow["录单人员"] = item["CreateUserName"];
                    newRow["确认人员"] = item["ConfUserName"];
                    newRow["过帐人员"] = item["PostingUserName"];
                    newRow["结算单据类型"] = item["BillTypeName"];
                    newRow["结算单号"] = item["BillID"];
                    newRow["金额"] = item["BillPayAmt"];
                    newRow["备注"] = item["Remark"];

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
                        5000,
                        Server.MapPath("UploadFile"),
                        fileName
                    );

            return File(byteArr, ConstDefinition.EXCEL_EXPORT_CONTEXT_TYPE, fileName);
        }
        #endregion



        /// <summary>
        /// 获取列表-批发价调整金额差异
        /// </summary>
        /// <param name="cpm">客户端分页对象</param>
        /// <returns>分页集合的json字符串</returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetListProductPriceDiff(BasePageModel cpm)
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.PROC_GETLISTPRODUCTPRICEDIFF_REPORT);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(jsonStr);
        }

        /// <summary>
        /// 批发价调整金额差异报表导出
        /// </summary>
        /// <returns></returns>
        public ActionResult ExportExcelProductPriceDiff()
        {
            string jsonStr = string.Empty;
            try
            {
                jsonStr = new ReportModel().GetReportData(false, ConstDefinition.PROC_GETLISTPRODUCTPRICEDIFF_REPORT);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
                jsonStr = new { info = ex.Message }.ToJsonString();
            }

            var fileName = "批发价调整金额差异报表导出_" + DateTime.Now.ToString("yyyyMMdd") + ".xls";

            var dataTable = new DataTable();
            dataTable.Columns.Add("单号");
            dataTable.Columns.Add("公司机构");
            dataTable.Columns.Add("单据类型");
            dataTable.Columns.Add("日期");
            dataTable.Columns.Add("过帐日");
            dataTable.Columns.Add("操作员");
            dataTable.Columns.Add("商品编码");
            dataTable.Columns.Add("商品名称");
            dataTable.Columns.Add("商品分类1");
            dataTable.Columns.Add("商品分类2");
            dataTable.Columns.Add("商品分类3");
            dataTable.Columns.Add("计量单位");
            dataTable.Columns.Add("采购员");
            dataTable.Columns.Add("旧价格");
            dataTable.Columns.Add("新价格");
            dataTable.Columns.Add("价格调整差异");
            dataTable.Columns.Add("库存数量");
            dataTable.Columns.Add("批发金额差异");

            if (!jsonStr.IsNullOrEmpty())
            {
                DataTable dt = DataTableConverter.JsonToDataTable(jsonStr);
                foreach (DataRow item in dt.Rows)
                {
                    var newRow = dataTable.NewRow();
                    newRow["单号"] = item["AdjId"];
                    newRow["公司机构"] = item["WName"];
                    newRow["单据类型"] = item["BillType"];
                    newRow["日期"] = item["BeginTime"];
                    newRow["过帐日"] = item["PostingTime"];
                    newRow["操作员"] = item["CreateUserName"];
                    newRow["商品编码"] = item["SKU"];
                    newRow["商品名称"] = item["ProductName"];
                    newRow["商品分类1"] = item["Category1Name"];
                    newRow["商品分类2"] = item["Category2Name"];
                    newRow["商品分类3"] = item["Category3Name"];
                    newRow["计量单位"] = item["Unit"];
                    newRow["采购员"] = item["BuyEmpName"];
                    newRow["旧价格"] = item["OldPrice"];
                    newRow["新价格"] = item["NewPrice"]; ;
                    newRow["价格调整差异"] = item["DifPrice"];
                    newRow["库存数量"] = item["StoreQty"];
                    newRow["批发金额差异"] = item["DifAmt"];
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


        /// <summary>
        /// 周转箱页面
        /// </summary>
        /// <returns></returns>
        public ActionResult PackageQty()
        {
            return View();
        }

        /// <summary>
        /// 周转箱数据
        /// </summary>
        /// <returns></returns>
        public ActionResult GetListPackageQtyData()
        {
            string jsonStr = "[]";
            try
            {
                jsonStr = new ReportModel().GetReportData(true, ConstDefinition.Proc_GetPackageQty);
            }
            catch (Exception ex)
            {
                Logger.GetInstance().Fatal(ex);
            }
            return Content(jsonStr);
        }

    }
}
