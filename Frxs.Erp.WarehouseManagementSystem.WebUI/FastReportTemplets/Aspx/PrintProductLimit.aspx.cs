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
    public partial class PrintProductLimit : System.Web.UI.Page
    {

        string NoSaleID = string.Empty;

        /// <summary>
        /// 页面加载
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                NoSaleID = Request.QueryString["NoSaleID"];
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
            var sPath = Server.MapPath("/FastReportTemplets/Frx/WProductNoSale.frx");
            fReport.Load(sPath);

            var productServer = WorkContext.CreateProductSdkClient();
            var resp = productServer.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductWProductNoSaleGetRequest()
            {
                NoSaleID = NoSaleID
            });
            if (resp != null && resp.Flag == 0)
            {
                //转DataTable
                DataTable dtOrder = DataTableConverter.ConvertEntityToDataRow(resp.Data.wProductNoSale);
                dtOrder.TableName = "dtOrder";

                DataTable dtOrderDetail = DataTableConverter.ConvertListToDataTable(resp.Data.productdetails);
                dtOrderDetail.TableName = "dtOrderDetail";

                DataTable dtOrderDetailExt = DataTableConverter.ConvertListToDataTable(resp.Data.shopGroups);
                dtOrderDetailExt.TableName = "dtOrderDetailExt";

                //表头
                fReport.RegisterData(dtOrder, "Head");

                //表体
                fReport.RegisterData(dtOrderDetail, "Detail");

                //表体
                fReport.RegisterData(dtOrderDetailExt, "DetailExt");
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