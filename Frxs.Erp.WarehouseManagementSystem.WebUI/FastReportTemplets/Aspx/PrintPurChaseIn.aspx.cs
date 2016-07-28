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
    public partial class PrintPurChaseIn : System.Web.UI.Page
    {

        string BuyID = string.Empty;

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
                BuyID = Request.QueryString["BuyID"];
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
            var BuyOrder = orderServer.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderBuyOrderPreGetModelRequest()
            {
                BuyID = BuyID,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });


            if (BuyOrder != null && BuyOrder.Flag == 0)
            {
                //转DataTable
                DataTable dtBuyOrder = DataTableConverter.ConvertEntityToDataRow(BuyOrder.Data.order);
                dtBuyOrder.TableName = "dtBuyOrder";

                DataTable dtBuyOrderDetail = DataTableConverter.ConvertListToDataTable(BuyOrder.Data.orderdetails);
                dtBuyOrderDetail.TableName = "dtBuyOrderDetail";

                var sPath = "";
                switch (Type)
                {
                    case "A4No":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseIn_NoPrice.frx");
                        break;
                    case "A4Yes":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseIn.frx");
                        break;
                    case "ThreeNo":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseIn_NoPrice_3.frx");
                        break;
                    case "ThreeYes":
                        sPath = Server.MapPath("/FastReportTemplets/Frx/PurchaseIn_3.frx");
                        break;
                }

                //加载报表文件
                fReport.Load(sPath);

                //表头
                fReport.RegisterData(dtBuyOrder, "Head");

                //表体
                fReport.RegisterData(dtBuyOrderDetail, "Detail");

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