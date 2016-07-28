using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Web;
using Frxs.Erp.WarehouseManagementSystem.WebUI;
using Frxs.Platform.Utility;
using Frxs.Platform.Utility.Web;
using Frxs.Platform.Utility.Json;
using System.Web.Mvc;
using Frxs.Platform.Utility.Map;
using System.Runtime.Serialization;
using Frxs.Erp.WarehouseManagementSystem.WebUI.Infrastructure;

namespace Frxs.Erp.WarehouseManagementSystem.WebUI.Models
{
    public class StockCheckPlanQuery : BasePageModel
    {
        
    }

    public class StockCheckPlanModel : BaseModel
    {
        public StockCheckPlanModel GetStockCheckPlan(string id)
        {
            var model = new StockCheckPlanModel();
            
            return model;
        }

        public string GetStockCheckPlanPageData(StockCheckPlanQuery cpm)
        {
            string jsonStr = "[]";
            
            return jsonStr;
        }

        public int DeleteStockCheckPlan(string ids)
        {
            return 0;
        }

        public ResultData AddStockCheckPlan(StockCheckPlanModel model)
        {
            return new ResultData();
        }

        public ResultData EditStockCheckPlan(StockCheckPlanModel model)
        {
            return new ResultData();
        }

        public string GetStockCheckPlanId()
        {
            var serviceCenter = WorkContext.CreateIDSdkClient();
            var resp = serviceCenter.Execute(new Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest()
            {
                WID = WorkContext.CurrentWarehouse.Parent.WarehouseId,
                Type = Frxs.Erp.ServiceCenter.ID.SDK.Request.FrxsErpIdIdsGetRequest.IDTypes.CheckStockPlan
            });
            if (resp != null && resp.Flag == 0)
            {
                return resp.Data;
            }
            else
            {
                return String.Empty;
            }
        }
    }
}