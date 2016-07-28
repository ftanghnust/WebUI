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
    public partial class PrintPurChaseOut : System.Web.UI.Page
    {

        string BackID = string.Empty;

        string Type = string.Empty;

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
                Type = Request.QueryString["Type"];
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
            var BackOrder = orderServer.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyBackPreGetModelRequest()
            {
                BackID = BackID,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });


            if (BackOrder != null && BackOrder.Flag == 0)
            {
                //转DataTable
                DataTable dtBackOrder = DataTableConverter.ConvertEntityToDataRow(BackOrder.Data.order);
                dtBackOrder.TableName = "dtBackOrder";

                DataTable dtBackOrderDetail = DataTableConverter.ConvertListToDataTable(BackOrder.Data.orderdetails);
                dtBackOrderDetail.TableName = "dtBackOrderDetail";

                var sPath = "";
                switch (Type)
                {
                    case "A4No":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseBack_NoPrice.frx");
                        break;
                    case "A4Yes":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseBack.frx");
                        break;
                    case "ThreeNo":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseBack_NoPrice_3.frx");
                        break;
                    case "ThreeYes":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseBack_3.frx");
                        break;
                }

                //加载报表文件
                fReport.Load(sPath);

                //表头
                fReport.RegisterData(dtBackOrder, "Head");

                //表体
                fReport.RegisterData(dtBackOrderDetail, "Detail");

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