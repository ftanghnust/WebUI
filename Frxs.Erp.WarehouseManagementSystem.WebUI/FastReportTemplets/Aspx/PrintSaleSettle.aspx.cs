using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FastReport.Web;
using System.ComponentModel;
using System.Data;
using Frxs.Platform.Utility.Map;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.FastReportTemplets.Aspx
{
    public partial class PrintSaleSettle : System.Web.UI.Page
    {

        string OrderID = string.Empty;

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                OrderID = Request.QueryString["OrderID"];
                WebFastReport.Prepare();
            }
        }

        /// <summary>
        /// Web Report开始加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void WebFastReport_StartReport(object sender, EventArgs e)
        {
            var webReport = sender as WebReport;
            if (webReport == null) return;
            var fReport = webReport.Report;

        

            var orderServer = WorkContext.CreateOrderSdkClient();
            var SaleSettle = orderServer.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleSettlePrinteGetModelRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                OrderId = OrderID
            });


            if (SaleSettle != null && SaleSettle.Flag == 0)
            {
                //转DataTable
                DataTable dtSaleSettle = DataTableConverter.ConvertEntityToDataRow(SaleSettle.Data.SaleSettle);
                dtSaleSettle.TableName = "dtSaleSettle";

                DataTable dtSaleSettleDetail = DataTableConverter.ConvertListToDataTable(SaleSettle.Data.SaleSettleDetailList);
                dtSaleSettleDetail.TableName = "dtSaleSettleDetail";

                DataTable dtSaleOrderDetail = DataTableConverter.ConvertListToDataTable(SaleSettle.Data.BackOrderDetails);
                dtSaleOrderDetail.TableName = "dtSaleOrderDetail";
                var sPath = Server.MapPath("/FastReportTemplets/Frx/SaleSettle.frx");
                if (dtSaleOrderDetail.Rows.Count>0)
                {
                     sPath = Server.MapPath("/FastReportTemplets/Frx/SaleSettles.frx");
                }
                //加载报表文件
                fReport.Load(sPath);

                //表头
                fReport.RegisterData(dtSaleSettle, "Head");

                //表体
                fReport.RegisterData(dtSaleSettleDetail, "Detail");

                if (dtSaleOrderDetail.Rows.Count > 0)
                {
                    //表体
                    fReport.RegisterData(dtSaleOrderDetail, "BackDetail");
                }
            }
        }

        /// <summary>
        /// 报表打印
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnPrint_Click(object sender, EventArgs e)
        {
            WebFastReport.Report.Print();
        }

        /// <summary>
        /// 导出报表到Execl
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnExecl_Click(object sender, EventArgs e)
        {
            WebFastReport.ExportExcel2007();
        }

        /// <summary>
        /// 导出报表到PDF
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void BtnPdf_Click(object sender, EventArgs e)
        {
            WebFastReport.ExportPdf();
        }

    }
}