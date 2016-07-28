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
using Frxs.ServiceCenter.SSO.Client;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Controllers.StockCheck
{
    public class StockCheckLossController : BaseController
    {
        [AuthorizeMenuFilter(520717)]
        public ActionResult Index()
        {
            return View();
        }

        [AuthorizeMenuFilter(520717)]
        [ValidateInput(false)]
        public ActionResult StockCheckAddOrEdit(string id)
        {
            return View();
        }

        [AuthorizeMenuFilter(520717)]
        public ActionResult StockCheckProduct()
        {
            return View();
        }
    }
}