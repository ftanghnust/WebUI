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
using Frxs.Erp.ServiceCenter.Order.SDK.Request;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.FastReportTemplets.Aspx
{
    public partial class PrintRepairApply : System.Web.UI.Page
    {

        string AppID = string.Empty;

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                AppID = Request.QueryString["AppID"];
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

            //加载报表文件
            var sPath = Server.MapPath("/FastReportTemplets/Frx/RepairApply.frx");
            fReport.Load(sPath);

            var orderServer = WorkContext.CreateOrderSdkClient();

            var resp = orderServer.Execute(new FrxsErpOrderBuyPreAppGetModelRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                AppID = AppID,
                UserId = WorkContext.UserIdentity.UserId,
                UserName = WorkContext.UserIdentity.UserName
            });

            if (resp != null && resp.Flag == 0)
            {

                //转DataTable
                DataTable dtOrder = DataTableConverter.ConvertEntityToDataRow(resp.Data.BuyPreApp);
                dtOrder.TableName = "dtOrder";

                DataTable dtOrderDetail = DataTableConverter.ConvertListToDataTable(resp.Data.BuyPreAppDetailsList);
                dtOrderDetail.TableName = "dtOrderDetail";

                //表头
                fReport.RegisterData(dtOrder, "Head");

                //表体
                fReport.RegisterData(dtOrderDetail, "Detail");
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