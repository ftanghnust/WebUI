using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers
{
    /// <summary>
    /// View Mode 接收参数
    /// </summary>
    public class BuyOrderDetailsReportViewModel
    {
        /// <summary>
        /// 子机构仓库
        /// </summary>
        public int? SubWID { get; set; }

        /// <summary>
        /// 采购单编码
        /// </summary>
        public string BuyID { get; set; }

        /// <summary>
        /// 供应商编码
        /// </summary>
        public string VendorCode { get; set; }

        /// <summary>
        /// 供应商名称
        /// </summary>
        public string VendorName { get; set; }

        /// <summary>
        /// 创建用户
        /// </summary>
        public string CreateUserName { get; set; }

        /// <summary>
        /// SKU编码
        /// </summary>
        public string SKU { get; set; }

        /// <summary>
        /// 商品名称
        /// </summary>
        public string ProductName { get; set; }

        /// <summary>
        /// 条形码
        /// </summary>
        public string BarCode { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CreateTime1 { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public DateTime? CreateTime2 { get; set; }

        /// <summary>
        /// 过账时间
        /// </summary>
        public DateTime? PostingTime1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DateTime? PostingTime2 { get; set; }


        /// <summary>
        /// 账期开始时间
        /// </summary>
        public DateTime? SettDateStart { get; set; }

        /// <summary>
        /// 账期结束时间
        /// </summary>
        public DateTime? SettDateEnd { get; set; }

        /// <summary>
        /// 分类编号
        /// </summary>
        public int? CategoryId1 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? CategoryId2 { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public int? CategoryId3 { get; set; }
    }

    /// <summary>
    /// 
    /// </summary>
    public class BuyOrderDetailsReportController : Controller
    {
        /// <summary>
        /// 获取列表
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult GetList(BuyOrderDetailsReportViewModel model)
        {
            var serviceCenter = WorkContext.CreateOrderSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.Order.SDK.Request.FrxsErpOrderReportBuyOrderDetailsRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                BuyID = model.BuyID,
                BarCode = model.BarCode,
                CategoryId1 = model.CategoryId1,
                CategoryId2 = model.CategoryId2,
                CategoryId3 = model.CategoryId3,
                CreateTime1 = model.CreateTime1,
                CreateTime2 = model.CreateTime2,
                CreateUserName = model.CreateUserName,
                PostingTime1 = model.PostingTime1,
                PostingTime2 = model.PostingTime2,
                ProductName = model.ProductName,
                SKU = model.SKU,
                SubWID = model.SubWID,
                VendorCode = model.VendorCode,
                VendorName = model.VendorName,
                SettDateStart = model.SettDateStart,
                SettDateEnd = model.SettDateEnd
            });

            return Content(resp.Data);
        }

        /// <summary>
        /// 打开首页
        /// </summary>
        /// <returns></returns>
        public ActionResult BuyOrderDetails()
        {
            return View();
        }
    }
}
