using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using FastReport.Web;
using System.ComponentModel;
using System.Data;
using Frxs.Erp.ServiceCenter.Order.SDK.Resp;
using Frxs.Platform.Utility.Map;
using Frxs.Platform.Utility;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.FastReportTemplets.Aspx
{
    public partial class PrintSaleOut : System.Web.UI.Page
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

            //加载报表文件
            var sPath = Server.MapPath("/FastReportTemplets/Frx/SaleOut.frx");
            fReport.Load(sPath);

            var orderServer = WorkContext.CreateOrderSdkClient();
            var resp = orderServer.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrdervSaleOrderGetRequest()
            {
                OrderId = OrderID,
                WarehouseId = WorkContext.CurrentWarehouse.Parent.WarehouseId
            });


            //商品详情信息  GetDeliverProductInfoAction
            var deliverProduct = new List<FrxsErpOrderGetDeliverProductInfoExtResp.ProductDetailExt>();
            var respDeliverProduct = orderServer.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderGetDeliverProductInfoExtRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId.ToString(),
                OrderId = OrderID
            });

            if (respDeliverProduct != null && respDeliverProduct.Flag == 0)
            {
                deliverProduct = respDeliverProduct.Data.ProductData.OrderBy(o => o.ShelfCode).ToList();
            }

            if (resp != null && resp.Flag == 0)
            {
                //转DataTable
                DataTable dtOrder = DataTableConverter.ConvertEntityToDataRow(resp.Data.Order);
                dtOrder.TableName = "dtOrder";

                DataTable orderDetail = DataTableConverter.ConvertListToDataTable(resp.Data.Details);
                DataTable dtOrderDetail = DataTableConverter.ConvertListToDataTable(deliverProduct);


                //组装DataTable-解决根据货区排序打印
                DataTable tblDatas = new DataTable("Datas");
                tblDatas.Columns.Add("SKU", Type.GetType("System.String"));
                tblDatas.Columns.Add("ProductName", Type.GetType("System.String"));
                tblDatas.Columns.Add("SaleUnit", Type.GetType("System.String"));
                tblDatas.Columns.Add("SalePrice", Type.GetType("System.String"));
                tblDatas.Columns.Add("SaleQty", Type.GetType("System.String"));
                tblDatas.Columns.Add("SubAmt", Type.GetType("System.String"));
                tblDatas.Columns.Add("ShopPoint", Type.GetType("System.String"));
                tblDatas.Columns.Add("Remark", Type.GetType("System.String"));
                tblDatas.Columns.Add("SubAddAmt", Type.GetType("System.String"));

                for (var i = 0; i < dtOrderDetail.Rows.Count; i++)
                {
                    for (var j = 0; j < orderDetail.Rows.Count; j++)
                    {
                        if (dtOrderDetail.Rows[i]["ProductId"].ToString() == orderDetail.Rows[j]["ProductId"].ToString())
                        {
                            DataRow newRow = tblDatas.NewRow();
                            newRow["SKU"] = orderDetail.Rows[j]["SKU"].ToString();
                            newRow["ProductName"] = orderDetail.Rows[j]["ProductName"].ToString();
                            newRow["SaleUnit"] = orderDetail.Rows[j]["SaleUnit"].ToString();
                            newRow["SalePrice"] = orderDetail.Rows[j]["SalePrice"].ToString();
                            newRow["SaleQty"] = orderDetail.Rows[j]["SaleQty"].ToString();
                            newRow["SubAmt"] = orderDetail.Rows[j]["SubAmt"].ToString();
                            newRow["ShopPoint"] = orderDetail.Rows[j]["ShopPoint"].ToString();
                            newRow["Remark"] = orderDetail.Rows[j]["Remark"].ToString();
                            newRow["SubAddAmt"] = orderDetail.Rows[j]["SubAddAmt"].ToString();
                            tblDatas.Rows.Add(newRow);

                        }
                    }
                }
                
                tblDatas.TableName = "dtOrderDetail";

                //表头
                fReport.RegisterData(dtOrder, "Head");

                //表体
                fReport.RegisterData(tblDatas, "Detail");

                var productServer = WorkContext.CreateProductSdkClient();
                var respAppSettings = productServer.Execute(new Frxs.Erp.ServiceCenter.Product.SDK.Request.FrxsErpProductSysAppSettingsGetRequest()
                {
                    WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                    SKey = ConstDefinition.SYS_APPSETTIONS_KEY_SALEORDERBILL
                });
                if (respAppSettings != null && respAppSettings.Flag == 0)
                {
                    DataTable dtOrderAppSettings = DataTableConverter.ConvertEntityToDataRow(respAppSettings.Data);
                    dtOrderAppSettings.TableName = "dtOrderAppSettings";

                    //表体
                    fReport.RegisterData(dtOrderAppSettings, "AppSettings");
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