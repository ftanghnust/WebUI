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
    public partial class PrintSaleBack : System.Web.UI.Page
    {
        string BackID = string.Empty;

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                BackID = Request.QueryString["BackID"];
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
            var SaleBack = orderServer.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderSaleBackPreGetModelRequest()
            {
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BackID = BackID
            });


            if (SaleBack != null && SaleBack.Flag == 0)
            {
                //转DataTable
                DataTable dtSaleBack = DataTableConverter.ConvertEntityToDataRow(SaleBack.Data.order);
                dtSaleBack.TableName = "dtSaleBack";

                DataTable dtSaleBackDetail = DataTableConverter.ConvertListToDataTable(SaleBack.Data.orderdetails);
                dtSaleBackDetail.TableName = "dtSaleBackDetail";

                var sPath = Server.MapPath("/FastReportTemplets/Frx/SaleBack_3.frx"); ;

                //加载报表文件
                fReport.Load(sPath);

                //表头
                fReport.RegisterData(dtSaleBack, "Head");

                //表体
                fReport.RegisterData(dtSaleBackDetail, "Detail");

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